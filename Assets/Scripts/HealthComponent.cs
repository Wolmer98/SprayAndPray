using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float m_maxHealth;

    private float m_currentHealth;

    public UnityEvent OnDie;

    void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (m_currentHealth <= 0)
            return;

        m_currentHealth -= damage;

        if (m_currentHealth <= 0)
            Die();
    }

    void Die()
    {
        OnDie.Invoke();
        Destroy(gameObject);
    }
}
