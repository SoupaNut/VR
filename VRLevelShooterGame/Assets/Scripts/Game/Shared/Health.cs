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

        public UnityAction onDamage;
        public UnityAction onDie;

        // Start is called before the first frame update
        private void Start()
        {
            m_CurrentHealth = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (Invincible)
            {
                return;
            }

            m_CurrentHealth -= damage;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, MaxHealth);

            onDamage?.Invoke();
            if (m_CurrentHealth <= 0f)
            {
                onDie?.Invoke();
            }
        }
    }
}

