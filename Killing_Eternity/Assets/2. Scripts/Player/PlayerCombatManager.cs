using UnityEngine;
namespace KE
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        [HideInInspector] public PlayerManager player;

        [Header("Testing")]
        public Skill testFireballSO;
        public SkillNode testIceConversionNode;
        public SkillNode testIceDamageNode;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void AssignSkillToHotbar(ActiveSkillInstance skillInstance, int hotbarIndex)
        {
            if (hotbarIndex >= 0 && hotbarIndex < activeSkills.Length)
            {
                activeSkills[hotbarIndex] = skillInstance;
            }
        }
        public void HandleSkillInput(int hotbarIndex)
        {
            ExecuteSkill(hotbarIndex);
        }
        public override void ExecuteSkill(int index)
        {
            base.ExecuteSkill(index);
        }

        protected override void Start()
        {
            base.Start();

            if (testFireballSO != null && player.skillTreeManager != null)
            {
                player.skillTreeManager.treeSockets[0].EquipPrimarySkill(testFireballSO);
                AssignSkillToHotbar(player.skillTreeManager.treeSockets[0].primarySkill, 0);

                // 2. Force the booleans to false so the Allocate method doesn't abort
                if (testIceConversionNode != null) testIceConversionNode.isAllocated = false;
                if (testIceDamageNode != null) testIceDamageNode.isAllocated = false;

                // 3. Allocate the nodes AFTER the skill is equipped. 
                // This pushes them into the activeNodes list and triggers RecalculateEntireTree!
                if (testIceConversionNode != null) player.skillTreeManager.AllocateNode(testIceConversionNode);
                if (testIceDamageNode != null) player.skillTreeManager.AllocateNode(testIceDamageNode);
            }
        }
    }
}
