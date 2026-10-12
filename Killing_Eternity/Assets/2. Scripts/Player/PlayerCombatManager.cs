using UnityEngine;
namespace KE
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        [Header("Testing")]
        public Skill testFireballSO;

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
                // 1. Drop the SO into Tree Socket 1 to generate the ActiveSkillInstance
                player.skillTreeManager.treeSockets[0].EquipPrimarySkill(testFireballSO);

                // 2. Map the generated instance to the South Button (Hotbar Index 0)
                AssignSkillToHotbar(player.skillTreeManager.treeSockets[0].primarySkill, 0);
            }
        }
    }
}
