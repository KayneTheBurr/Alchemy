using UnityEngine;

namespace KE
{
    public interface IDamageable
    {
        void TakeDamage(float damage, Element element);

        Transform GetTransform();
    }
}