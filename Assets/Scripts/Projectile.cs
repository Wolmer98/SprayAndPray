using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Damage;
    public float LifeTime = 3.0f;

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

        Destroy(gameObject);
    }
}
