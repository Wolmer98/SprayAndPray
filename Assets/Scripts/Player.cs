using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        GetComponent<HealthComponent>().OnDie.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<HealthComponent>().TakeDamage(1.0f);
        }
    }
}
