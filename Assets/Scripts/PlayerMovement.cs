using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed;
    private Vector3 m_direction;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            m_direction += Vector3.up;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            m_direction += Vector3.down;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            m_direction += Vector3.left;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            m_direction += Vector3.right;

        m_direction.Normalize();

        Move();

        m_direction = Vector3.zero;
    }

    void Move()
    {
        m_direction.z = 0; // Never move in z-axis.
        transform.position += m_direction * m_movementSpeed * Time.deltaTime;
    }
}
