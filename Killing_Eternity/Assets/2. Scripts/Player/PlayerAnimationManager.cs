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


        

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }
    }
}
