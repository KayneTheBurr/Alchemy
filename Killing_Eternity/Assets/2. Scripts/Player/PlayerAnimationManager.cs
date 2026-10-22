using System;
using UnityEngine;



namespace KE
{
    public class PlayerAnimationManager : CharacterAnimationManager
    {
        PlayerManager player;

        [Header("Player Animation States")]
        public string dodgeForward = "Dodge_Forward";
        public string dodgeBackward = "Dodge_Back";
        public string catalystRaise = "Catalyst_Raise_01";





        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }
    }
}
