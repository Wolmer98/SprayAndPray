using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Damage;
    public float LifeTime = 3.0f;
    public int PierceTimes = 0;

    private int m_pierceCounter = 0;

    private void Start()
    {
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
        if(m_pierceCounter > PierceTimes)
            Destroy(gameObject);
    }
}
