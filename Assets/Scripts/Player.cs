using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int m_xp;
    private int m_level;
    private int m_xpRequired = 15;

    private AudioSource m_audioSource;

    [SerializeField] private AudioClip m_pickupAudioClip;
    [SerializeField] private AudioClip m_levelupAudioClip;

    [SerializeField] private float m_fireChainSlotChance = 0.25f;
    [SerializeField] private List<Item> m_possibleLoot = new List<Item>();

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void AddXP(int delta)
    {
        m_xp += delta;
        if (m_xp >= m_xpRequired)
        {
            LevelUp();
            m_xp = 0;

            m_audioSource.PlayOneShot(m_levelupAudioClip);

        }
        else
        {
            m_audioSource.PlayOneShot(m_pickupAudioClip);
        }
    }

    private void LevelUp()
    {
        float rng = Random.Range(0.0f, 1.0f);
        m_level++;
        if (m_level < 3)
            rng = 2.0f; // Remove chance for slots the first levels.

        if (rng <= m_fireChainSlotChance && !GameManager.Instance.Inventory.MaximumFireslotsReached())
        {
            GameManager.Instance.Inventory.AddFireChainSlot();
            FloatingTextManager.Instance.SpawnFloatingText("NEW FIRE SLOT [TAB]", transform.position, transform);
            return;
        }
        else
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

                FloatingTextManager.Instance.SpawnFloatingText("NEW ITEM [TAB]", transform.position, transform);
                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LevelUp();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < 50; i++)
            {
                LevelUp();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale = 10.0f;
        if (Input.GetKeyUp(KeyCode.T))
            Time.timeScale = 1.0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<HealthComponent>().TakeDamage(1.0f);
        }
    }
}
