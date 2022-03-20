using System;
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

    public Player Player;
    public WeaponItem StartWeapon;
    public Inventory Inventory;
    public HoverPopup HoverPopup;
    public GameObject DeathScreen;

    [SerializeField] TMPro.TMP_Text m_timerText;

    private void Start()
    {
        Inventory.Init();
        Player.GetComponent<HealthComponent>().OnDie.AddListener(GameOver);
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

        int seconds = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        m_timerText.text = time.ToString(@"m\:ss");
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        PauseGame();
        DeathScreen.SetActive(true);
    }

    public void Restart()
    {
        UnpauseGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
