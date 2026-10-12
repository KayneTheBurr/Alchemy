using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Node")]
public class SkillNode : ScriptableObject
{
    [Header("Tree Location")]
    [Tooltip("Set to 1-6 if it belongs to a specific slot's patterns. Set to 0 if it's a Global/Arch node.")]
    public int connectedSocketID = 0;

    public string skillID = "";
    public bool isAllocated = false;

    public List<SkillNode> requiredPreviousNodes;
    public List<SkillNode> mutuallyExclusiveNodes;

    public List<SkillNodeEffect> effects;
}
