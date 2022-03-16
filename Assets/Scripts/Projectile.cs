using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Damage;
    public float LifeTime = 3.0f;
    public int PierceTimes = 0;
    public int ProjectilesOnDestroy = 0;

    private int m_pierceCounter = 0;

    private void Start()
    {
        Debug.Log("DAMAGE: " + Damage);
        Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent)
            {
                healthComponent.TakeDamage(Damage);
            }
        }

        m_pierceCounter++;
        if (m_pierceCounter > PierceTimes)
            Die();
    }

    private void Die()
    {
        for (int i = 0; i < ProjectilesOnDestroy; i++)
        {
            var newProjectile = Instantiate(gameObject, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Projectile>();
            newProjectile.ProjectilesOnDestroy = 0; // Prevent infinite spawning.
            newProjectile.PierceTimes = 0;
        }
     
        Destroy(gameObject, 0.05f); // Small delay to ensure ondestroy projectiles are spawned first.
    }
}
