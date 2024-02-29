using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.Shared
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")]
        public float MaxHealth = 100f;

        private float m_CurrentHealth;
        public float CurrentHealth { get => m_CurrentHealth; set => m_CurrentHealth = value; }

        private bool m_Invincible = false;
        public bool Invincible { get => m_Invincible; set => m_Invincible = value; }

        public UnityAction<float, GameObject> onDamage;
        public UnityAction onDie;

        // Start is called before the first frame update
        private void Start()
        {
            m_CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
            {
                return;
            }

            float healthBefore = m_CurrentHealth;
            m_CurrentHealth -= damage;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, MaxHealth);

            // Call onDamage action
            float trueDamageTaken = healthBefore - m_CurrentHealth;
            if(trueDamageTaken > 0)
            {
                onDamage?.Invoke(trueDamageTaken, damageSource);
            }

            HandleDeath();
        }

        private void HandleDeath()
        {
            if (m_CurrentHealth <= 0f)
            {
                onDie?.Invoke();
            }
        }
    }
}

