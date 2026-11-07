using UnityEngine;

namespace KE
{
    public interface IDamageable
    {
        public Faction MyFaction { get; }
        bool IsDead();

        void TakeDamage(float damage, Element element, IDamageable damageSource = null, bool isDoT = false);

        Transform GetTransform();

        void ApplyBuildup(Element element, float buildupAmount, float dotAmount, IDamageable source = null);
    }
}