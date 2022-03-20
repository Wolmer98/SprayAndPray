using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostStatusEffect : MonoBehaviour
{
    void Start()
    {
        var comp = GetComponent<EnemyMovement>();
        if (comp != null)
        {
            comp.m_movementSpeed *= 0.35f;
        }
    }
}
