using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed;

    private float m_swayPeriod;
    private float m_swayDegrees;

    private void Start()
    {
        m_swayPeriod = Random.Range(0.5f, 5.0f);
        m_swayDegrees = Random.Range(-60.0f, 60.0f);
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (GameManager.Instance.Player == null)
            return;

        var direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        direction = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.time * m_swayPeriod) * m_swayDegrees) * direction;
        
        transform.position += direction * m_movementSpeed * Time.deltaTime;
    }
}
