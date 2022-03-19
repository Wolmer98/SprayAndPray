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
        var healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDie.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));

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
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<HealthComponent>().TakeDamage(1.0f);
        }
    }
}
