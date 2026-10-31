using UnityEngine;

namespace KE
{
    public interface IDamageable
    {
        public Faction MyFaction { get; }
        bool IsDead();

        void TakeDamage(float damage, Element element, IDamageable damageSource = null);

        Transform GetTransform();

        void ApplyBuildup(Element element, float buildupAmount, float dotAmount, IDamageable source = null);
    }
}