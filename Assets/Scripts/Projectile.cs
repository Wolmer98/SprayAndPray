using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public FireSystem.FireRequest FireRequest;

    [SerializeField] private float m_lifeTime = 3.0f;
    private int m_pierceCounter = 0;

    [SerializeField] private GameObject m_hitEffect;
    [SerializeField] private AudioClip m_hitAudioClip;

    private AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, m_lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent)
            {
                healthComponent.TakeDamage(FireRequest.Damage);
            }
        }

        if (m_hitEffect != null)
            Instantiate(m_hitEffect, transform.position, Quaternion.identity);

        if (m_audioSource != null && m_hitAudioClip != null)
            m_audioSource.PlayOneShot(m_hitAudioClip);

        m_pierceCounter++;
        if (m_pierceCounter > FireRequest.PierceTimes)
            Die();
    }

    private void Die()
    {
        for (int i = 0; i < FireRequest.ProjectilesOnDestroy; i++)
        {
            var newProjectile = Instantiate(gameObject, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Projectile>();
            newProjectile.FireRequest = FireRequest;
            newProjectile.FireRequest.ProjectilesOnDestroy = 0;// Prevent infinite spawning.
            newProjectile.m_pierceCounter = 0;
        }

        Destroy(gameObject, 0.05f); // Small delay to ensure ondestroy projectiles are spawned first.
    }
}
