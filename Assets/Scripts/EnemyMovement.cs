using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed;

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (GameManager.Instance.Player == null)
            return;

        var direction = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        transform.position += direction * m_movementSpeed * Time.deltaTime;
    }
}
