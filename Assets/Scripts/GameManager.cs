using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<GameManager>();

            return m_instance;
        }
    }

    public GameObject Player;
    public Inventory Inventory;
    public WeaponItem StartWeapon;

    private void Start()
    {
        Inventory.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Inventory.gameObject.SetActive(!Inventory.gameObject.activeSelf);

            if (Inventory.gameObject.activeSelf)
                PauseGame();
            else
                UnpauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
