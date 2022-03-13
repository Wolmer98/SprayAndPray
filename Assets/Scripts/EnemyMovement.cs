using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed;

    private Transform m_playerTransform;

    void Start()
    {
        // TODO: Replace with a get from some game-manager.
        m_playerTransform = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        var direction = (m_playerTransform.position - transform.position).normalized;
        transform.position += direction * m_movementSpeed * Time.deltaTime;
    }
}
