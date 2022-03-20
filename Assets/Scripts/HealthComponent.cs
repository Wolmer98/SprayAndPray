using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float m_maxHealth;
    [SerializeField] private XPDrop m_xpDropPrefab;
    [SerializeField] private float m_invincibilityTime;
    float m_lastTimeHit;

    [SerializeField] private GameObject m_damagedEffect;
    [SerializeField] private AudioClip m_damagedAudioClip;

    private float m_currentHealth;
    private AudioSource m_audioSource;

    public UnityEvent OnTakeDamage;
    public UnityEvent OnDie;

    void Start()
    {
        m_currentHealth = m_maxHealth;

        m_audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damage)
    {
        if (m_currentHealth <= 0)
            return;

        if (m_lastTimeHit + m_invincibilityTime > Time.timeSinceLevelLoad)
            return;
        m_lastTimeHit = Time.timeSinceLevelLoad;

        m_currentHealth -= damage;
        OnTakeDamage.Invoke();

        if (m_damagedEffect != null)
            Instantiate(m_damagedEffect, transform.position, Quaternion.identity);
        if (m_damagedAudioClip != null && m_audioSource != null)
            m_audioSource.PlayOneShot(m_damagedAudioClip);

        var message = Mathf.CeilToInt(damage).ToString();
        FloatingTextManager.Instance.SpawnFloatingText(message, transform.position, null);

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
        var chance = Mathf.Lerp(0.0f, 1.0f, Time.timeSinceLevelLoad / EnemyManager.Instance.m_maxTimeDifficultyScaling);
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
