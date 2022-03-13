using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float MovementSpeed;

    void Update()
    {
        Move();   
    }

    void Move()
    {
        transform.position += transform.up * MovementSpeed * Time.deltaTime;
    }
}
