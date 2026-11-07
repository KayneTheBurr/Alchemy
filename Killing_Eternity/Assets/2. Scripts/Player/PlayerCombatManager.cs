using UnityEngine;
namespace KE
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        #region Variables
        [HideInInspector] public PlayerManager player;

        [Header("Testing")]
        public Skill testFireballSO;
        public Skill testSmiteSO;
        public Skill testIceLanceSO;
        public SkillNode testIceConversionNode;
        public SkillNode testIceDamageNode;

        public bool hasFireSkill = false;
        public bool hasIceSkill = false;

        [Header("Targeting Logic")]
        [Tooltip("Radius to scan for enemy targets. 18 is ~about the size of what is visible on screen")]
        public float enemyTargetingDistance = 15f;

        [Tooltip("Radius to scan for targetable environment objects, shorter than enemy distance to prevent revealing hidden objects")]
        public float environmentTargetDistance = 3f;

        [Tooltip("Bonus score added to the currently locked target. Prevents the highlight from rapidly flickering back and forth between two closely grouped enemies when aiming between them.")]
        public float targetStickiness = 30f;

        [Tooltip("Multiplier for aiming accuracy. Higher values force the lock-on to follow the stick/facing direction; lower values allow closer enemies to win out even if they are off-center.")]
        public float dotProductMult = 50f;

        [Tooltip("Flat score bonus given to enemies to ensure they always take targeting priority over destructible environment objects at the same distance/alignment.")]
        public float enemyTargetScoreAdditive = 50f;

        [Tooltip("Penalty applied per unit of distance. Increase this to make the system aggressively prioritize closer threats over more aligned distant ones.")]
        public float distanceMultiplier = 1.0f;

        public float mouseForgivenessRadius = 1.0f;
        private Vector3 losOffset = new Vector3(0, 1f, 0);
        private IDamageable previousTarget;
        #endregion

        #region Core Functions
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        protected override void Start()
        {
            base.Start();

            //if (testFireballSO != null && player.skillTreeManager != null)
            //{
            //    player.skillTreeManager.treeSockets[0].EquipPrimarySkill(testFireballSO);
            //    AssignSkillToHotbar(player.skillTreeManager.treeSockets[0].primarySkill, 0);

            //    // 2. Force the booleans to false so the Allocate method doesn't abort
            //    if (testIceConversionNode != null) testIceConversionNode.isAllocated = false;
            //    if (testIceDamageNode != null) testIceDamageNode.isAllocated = false;

            //    // 3. Allocate the nodes AFTER the skill is equipped. 
            //    // This pushes them into the activeNodes list and triggers RecalculateEntireTree!
            //    if (testIceConversionNode != null) player.skillTreeManager.AllocateNode(testIceConversionNode);
            //    if (testIceDamageNode != null) player.skillTreeManager.AllocateNode(testIceDamageNode);
            //}
            if(testSmiteSO != null && player.skillTreeManager != null)
            {
                player.skillTreeManager.treeSockets[1].EquipPrimarySkill(testSmiteSO);
                AssignSkillToHotbar(player.skillTreeManager.treeSockets[1].primarySkill, 1);
            }
            if(testIceLanceSO != null && player.skillTreeManager != null)
            {
                player.skillTreeManager.treeSockets[0].EquipPrimarySkill(testIceLanceSO);
                AssignSkillToHotbar(player.skillTreeManager.treeSockets[0].primarySkill, 0);
            }
        }
        protected override void Update()
        {
            base.Update();
            HandleSmartTargeting();
        }
        #endregion

        #region Skill Assignmetn and Use
        public void AssignSkillToHotbar(ActiveSkillInstance skillInstance, int hotbarIndex)
        {
            if (hotbarIndex >= 0 && hotbarIndex < activeSkills.Length)
            {
                activeSkills[hotbarIndex] = skillInstance;
            }
        }
        public void HandleSkillInput(int hotbarIndex)
        {
            //DEBUG TEMP ONLY
            if (hotbarIndex == 0 && hasIceSkill == false) return;
            if (hotbarIndex == 1 && hasFireSkill == false) return;


            ExecuteSkill(hotbarIndex);
        }
        public override void ExecuteSkill(int index)
        {
            base.ExecuteSkill(index);
        }
        #endregion

        #region Targeting
        private void HandleSmartTargeting()
        {
            if (!player.IsTargetValid(currentTarget))
            {
                currentTarget = null;
            }

            LayerMask enemyLayer = WorldUtilityManager.instance.GetCharacterLayers();
            LayerMask envLayer = WorldUtilityManager.instance.GetEnviroLayers();
            LayerMask damageLayer = WorldUtilityManager.instance.GetDamageLayers();

            Vector3 aimVector = player.transform.forward;

            // --- MNK ---
            if (!InputManager.instance.isGamepadActive)
            {
                Ray ray = PlayerCamera.instance.realCam.ScreenPointToRay(InputManager.instance.lookInput);

                //hard lock to target 
                if (Physics.SphereCast(ray, mouseForgivenessRadius, out RaycastHit mouseHit, 150f, damageLayer))
                {
                    IDamageable mouseTarget = mouseHit.collider.GetComponent<IDamageable>();

                    //make sure its a valid target and that it isnt ourselves
                    if (player.IsTargetValid(mouseTarget) && mouseTarget.GetTransform() != transform)
                    {
                        SetNewTarget(mouseTarget);
                    }
                }

                if (Physics.Raycast(ray, out RaycastHit envHit, 150f, envLayer))
                {
                    Vector3 flatMousePoint = envHit.point;
                    flatMousePoint.y = transform.position.y;
                    aimVector = (flatMousePoint - transform.position).normalized;
                }
            }

            // --- CONTROLLER ---
            else
            {
                //right stick manually sets the facing direction and aim
                if (InputManager.instance.lookInputActive)
                {
                    aimVector = InputManager.instance.lookDirection;
                }
                else if (currentTarget != null && !currentTarget.IsDead())
                {
                    Vector3 flatTarget = currentTarget.GetTransform().position;
                    flatTarget.y = transform.position.y;
                    aimVector = (flatTarget - transform.position).normalized;
                }
                else if (InputManager.instance.moveDirection.sqrMagnitude > 0.1f)
                {
                    aimVector = InputManager.instance.moveDirection.normalized;
                }
            }

            // ----- SCORING TARGETS ----- //

            // the | here means that it can be either/both of the two layer options
            //layermasks use binary -> get read different 
            //is much more efficient than running a for loop through all 32 possible layers
            LayerMask scanMask = enemyLayer | envLayer;

            Collider[] hits = Physics.OverlapSphere(transform.position, enemyTargetingDistance, scanMask);

            IDamageable bestTarget = null;
            float highestScore = -float.MaxValue;
            Vector3 raycastOrigin = transform.position + losOffset;

            float dynamicStickiness = targetStickiness;
            if (InputManager.instance.isGamepadActive && InputManager.instance.lookInputActive)
            {
                //remove the target stickiness on controller when using input, and stick to the target only when passively moving 
                dynamicStickiness = 0f;
            }

            foreach (Collider hit in hits)
            {
                IDamageable potential = hit.GetComponent<IDamageable>();
                if (!player.IsTargetValid(potential)) continue;

                //dont lock onto ourselves
                if (potential.GetTransform() == transform) continue;

                //factor/mulitpier for character class targets vs enviro targets

                float distance = Vector3.Distance(transform.position, hit.transform.position);

                // << operator is like normalizing but for binary
                // comparing to see if the hit is included in the environment layer
                bool isEnvironment = (envLayer == (envLayer | (1 << hit.gameObject.layer)));

                // ignore environment objects outside of thier range
                if (isEnvironment && distance > environmentTargetDistance) continue;

                // LOS check
                Vector3 targetCenter = hit.transform.position + losOffset;
                Vector3 dirToTarget3D = (targetCenter - raycastOrigin).normalized;

                if (Physics.Raycast(raycastOrigin, dirToTarget3D, out RaycastHit losHit, distance, envLayer))
                {
                    if (losHit.collider.gameObject != hit.gameObject)
                    {
                        continue; // if blocked by a wall that is NOT this target
                    }
                }

                Vector3 flatHit = hit.transform.position;
                flatHit.y = transform.position.y;
                Vector3 dirToTargetFlat = (flatHit - transform.position).normalized;

                // 1.0 is right in front, -1.0 is exactly behind
                float alignment = Vector3.Dot(aimVector, dirToTargetFlat);

                // score each target with alignment and distance
                float score = (alignment * dotProductMult) - (distance * distanceMultiplier);

                // Enemies inherently score much higher so destructible barrels don't steal aggro
                if (!isEnvironment) score += enemyTargetScoreAdditive;

                // stickiness modifier for our current target
                if (potential == currentTarget) score += dynamicStickiness;

                if (score > highestScore)
                {
                    highestScore = score;
                    bestTarget = potential;
                }
            }

            if (bestTarget != null)
            {
                SetNewTarget(bestTarget);
            }
            else if (!player.isPerformingAction)
            {
                SetNewTarget(null);
            }
        }

        private void SetNewTarget(IDamageable newTarget)
        {
            currentTarget = newTarget;

            if (currentTarget != previousTarget)
            {
                // Turn off old target
                if (previousTarget != null && !previousTarget.IsDead())
                {
                    TargetHighlight oldHighlight = previousTarget.GetTransform().GetComponent<TargetHighlight>();
                    if (oldHighlight != null) oldHighlight.SetHighlight(false);
                }

                // Turn on new target
                if (currentTarget != null)
                {
                    TargetHighlight newHighlight = currentTarget.GetTransform().GetComponent<TargetHighlight>();
                    if (newHighlight != null) newHighlight.SetHighlight(true);
                }

                PlayerUIManager.instance.playerHUDManager.SetTarget(currentTarget);

                previousTarget = currentTarget;
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyTargetingDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, environmentTargetDistance);
        }
    }
}
