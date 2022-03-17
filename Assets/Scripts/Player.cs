using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int m_xp;
    private int m_level;
    private int m_xpRequired = 15;

    [SerializeField] private List<Item> m_possibleLoot = new List<Item>();

    private void Start()
    {
        GetComponent<HealthComponent>().OnDie.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
    }

    public void AddXP(int delta)
    {
        m_xp += delta;
        if (m_xp >= m_xpRequired)
        {
            LevelUp();
            m_xp = 0;
        }
    }

    private void LevelUp()
    {
        m_possibleLoot.Shuffle();
        Item selectedItem = null;
        foreach (var item in m_possibleLoot)
        {
            if (GameManager.Instance.Inventory.m_items.Contains(item))
                continue;
            else
            {
                selectedItem = item;
                break;
            }
        }

        if (selectedItem != null)
        {
            GameManager.Instance.Inventory.AddItem(selectedItem);
            m_possibleLoot.Remove(selectedItem);

            var messagePos = transform.position + (Vector3.up * 5);
            FloatingTextManager.Instance.SpawnFloatingText("NEW ITEM [TAB]", transform.position, transform);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LevelUp();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<HealthComponent>().TakeDamage(1.0f);
        }
    }
}
