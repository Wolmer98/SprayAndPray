using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStatusEffect : MonoBehaviour
{
    private HealthComponent m_healthComponent;

    private float m_tickDamage = 1.0f;
    private float m_tickInterval = 1.0f;
    private float m_tickTimer;

    void Start()
    {
        m_healthComponent = GetComponent<HealthComponent>();
    }

    void Update()
    {
        m_tickTimer += Time.deltaTime;

        if (m_tickTimer >= m_tickInterval)
        {
            m_tickTimer = 0.0f;
            m_healthComponent.TakeDamage(m_tickDamage);
        }
    }
}
