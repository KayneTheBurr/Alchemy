using ECM2;
using UnityEngine;

namespace KE
{
    public class CharacterManager : MonoBehaviour
    {
        [HideInInspector] public Character characterController;
        [HideInInspector] public Animator animator;

        protected virtual void Awake()
        {
            characterController = GetComponent<Character>();
            animator = GetComponentInChildren<Animator>();
        }

        protected virtual void Update()
        {

        }



    }
}
