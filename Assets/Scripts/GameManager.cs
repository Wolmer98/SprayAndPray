using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    [SerializeField] private AudioMixerGroup m_sfxGroup;
    private Dictionary<string, float> m_sfxPlayTimes = new Dictionary<string, float>();

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

    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        float minAudioClipCooldown = UnityEngine.Random.Range(0.05f, 0.2f);

        if (m_sfxPlayTimes.ContainsKey(audioClip.name))
        {
            if (Time.timeSinceLevelLoad < m_sfxPlayTimes[audioClip.name] + minAudioClipCooldown)
                return;
            
            m_sfxPlayTimes[audioClip.name] = Time.timeSinceLevelLoad;
        }
        else
        {
            m_sfxPlayTimes.Add(audioClip.name, Time.timeSinceLevelLoad);
        }

        var audioSourceGo = new GameObject();
        var audioSource = audioSourceGo.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = m_sfxGroup;
        audioSource.PlayOneShot(audioClip);

        Destroy(audioSourceGo, audioClip.length + 0.1f);
    }
}
