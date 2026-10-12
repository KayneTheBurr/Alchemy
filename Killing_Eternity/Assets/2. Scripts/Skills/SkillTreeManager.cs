using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    public class SkillTreeManager : MonoBehaviour
    {
        public PlayerManager player;

        [Header("The Tree Sockets")]
        public SkillSocket[] treeSockets = new SkillSocket[6];

        [Header("Allocated Progression")]
        public List<SkillNode> activeNodes = new List<SkillNode>();

        protected virtual void Awake()
        {
            player = GetComponent<PlayerManager>();

            // Initialize the 6 sockets with their IDs
            for (int i = 0; i < 6; i++)
            {
                treeSockets[i] = new SkillSocket(i + 1);
            }
        }

        public void AllocateNode(SkillNode targetNode)
        {
            if (targetNode.isAllocated) return;

            // verify requirements here (skill points, mutually exclusive checks, pathing)

            targetNode.isAllocated = true;
            activeNodes.Add(targetNode);

            RecalculateEntireTree();
        }
        public void RecalculateEntireTree()
        {
            // 6 part array for each of the different sockets, able to hold all effects that apply to them 
            List<SkillNodeEffect>[] effectsPerSocket = new List<SkillNodeEffect>[6];
            for (int i = 0; i < 6; i++)
            {
                effectsPerSocket[i] = new List<SkillNodeEffect>();
            }

            List<SkillNodeEffect> globalEffects = new List<SkillNodeEffect>();

            
            // sort the skill trees node effects into global, skill1, skill2, etc. 
            foreach (SkillNode node in activeNodes)
            {
                foreach (SkillNodeEffect effect in node.effects)
                {
                    if (effect.scope == NodeScope.Global)
                    {
                        globalEffects.Add(effect);
                    }
                    else if (effect.scope == NodeScope.SkillSocket)
                    {
                        //Get the socket that this skill node applies too
                        int arrayIndex = node.connectedSocketID - 1;

                        if (arrayIndex >= 0 && arrayIndex < 6)
                        {
                            effectsPerSocket[arrayIndex].Add(effect);
                        }
                    }
                }
            }

            // send the list of effects into the sockets
            for (int i = 0; i < 6; i++)
            {
                treeSockets[i].RecalculateSocketMath(effectsPerSocket[i]);
            }

            // send the list of global effects into the player stats manager
            ApplyGlobalEffects(globalEffects);
        }

        private void ApplyGlobalEffects(List<SkillNodeEffect> globalEffects)
        {
            //blood mage, etc.
        }
    }
}
