using UnityEngine;

namespace Unity.Game.Shared
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")]
        public float MaxHealth = 100f;

        [Tooltip("Object does not take damage")]
        public bool Invincible;

        public float CurrentHealth { get; private set; }

        public UnityAction<float, GameObject> onDamage;
        public UnityAction onDie;

        // Start is called before the first frame update
        void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
            {
                return;
            }

            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            // Call onDamage action
            float trueDamageTaken = healthBefore - CurrentHealth;
            if (trueDamageTaken > 0)
            {
                onDamage?.Invoke(trueDamageTaken, damageSource);
            }

            HandleDeath();
        }

        void HandleDeath()
        {
            if (CurrentHealth <= 0f)
            {
                onDie?.Invoke();
            }
        }
    }
}