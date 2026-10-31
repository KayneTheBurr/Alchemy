using UnityEngine;
namespace KE
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;

        [Header("Layer Masks")]
        [SerializeField] LayerMask characterLayers;
        [SerializeField] LayerMask damageLayers;
        [SerializeField] LayerMask enviroLayers;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            
        }
        private void Update()
        {

        }
        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }
        public LayerMask GetDamageLayers()
        {
            return damageLayers;
        }
        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }


        //public enum Faction
        //{
        //    Player,
        //    Enemy,
        //    Ally,
        //    Neutral
        //}
        public bool CanIDamageThisTarget(Faction attacker, Faction target)
        {
            if (attacker == Faction.Player || attacker == Faction.Ally)
            {
                switch (target)
                {
                    case Faction.Player:
                    case Faction.Ally:
                        return false;

                    case Faction.Enemy:
                    case Faction.Neutral:
                        return true;
                }
            }

            else if (attacker == Faction.Enemy)
            {
                switch (target)
                {
                    case Faction.Player:
                    case Faction.Ally:
                        return true;

                    case Faction.Enemy:
                    case Faction.Neutral:
                        return false;
                }
            }

            else if(attacker == Faction.Neutral)
            {
                return true;
            }

            return false;
        }
    }
}