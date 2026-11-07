
using KE;
using UnityEngine;

public class TEST_TriggerSkillPickup : MonoBehaviour
{
    public Element myElement;
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerManager>();

        if( player != null )
        {
            switch (myElement)
            {
                case Element.Fire:
                    player.playerCombatManager.hasFireSkill = true;
                    break;
                case Element.Ice:
                    player.playerCombatManager.hasIceSkill = true;
                    break;

                default:
                    break;
            }
        }

        Destroy(this.gameObject);
    }
}
