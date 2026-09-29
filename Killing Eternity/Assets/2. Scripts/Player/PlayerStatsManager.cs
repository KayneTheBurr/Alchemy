using UnityEngine;

namespace KE
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        [Header("Speed Stats")]
        public ObservableVariable moveSpeedMultiplier = new ObservableVariable(1.0f);
    }
}
