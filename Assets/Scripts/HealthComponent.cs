using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float m_maxHealth;
    [SerializeField] private XPDrop m_xpDropPrefab;

    private float m_currentHealth;

    public UnityEvent OnTakeDamage;
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
        OnTakeDamage.Invoke();

        // Temp.
        //FloatingTextManager.Instance.SpawnFloatingText(damage.ToString(), transform.position);

        if (m_currentHealth <= 0)
            Die();
    }

    void Die()
    {
        OnDie.Invoke();

        if (ShouldDropXP() && m_xpDropPrefab != null)
            Instantiate(m_xpDropPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private bool ShouldDropXP()
    {
        float rng = Random.Range(0.0f, 1.0f);
        var chance = Mathf.Lerp(0.0f, 1.0f, Time.timeSinceLevelLoad / 300.0f);
        return rng > chance;
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public float GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        m_maxHealth = newMaxHealth;
    }
}
