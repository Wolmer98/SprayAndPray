using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager m_instance;
    public static EnemyManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<EnemyManager>();

            return m_instance;
        }
    }

    [SerializeField] private float m_spawnCooldown;
    [SerializeField] private float m_spawnWaveCooldown;
    private float m_spawnTimer;
    private float m_spawnWaveTimer;

    [SerializeField] private float m_spawnDistance;
    [SerializeField] private float m_maxTimeDifficultyScaling;
    [SerializeField] AnimationCurve m_waveAmount;

    [SerializeField] GameObject[] m_enemyPrefabs;

    void Start()
    {
        
    }

    void Update()
    {
        if (m_spawnTimer >= m_spawnCooldown)
        {
            m_spawnTimer = 0;
            SpawnRandomEnemy();
        }
        else
        {
            m_spawnTimer += Time.deltaTime;
        }

        if (m_spawnWaveTimer >= m_spawnWaveCooldown)
        {
            m_spawnWaveTimer = 0;
            SpawnEnemyWave();
        }
        else
        {
            m_spawnWaveTimer += Time.deltaTime;
        }
    }

    private async void SpawnEnemyWave()
    {
        var enemyPrefab = GetRandomEnemyPrefab();
        var waveOffset = Random.insideUnitCircle.normalized * m_spawnDistance;

        var t = Mathf.Clamp01(Time.timeSinceLevelLoad / m_maxTimeDifficultyScaling);
        int spawnAmount = (int)m_waveAmount.Evaluate(t);

        for (int i = 0; i < spawnAmount; i++)
        {
            var randomOffset = (Random.insideUnitCircle.normalized * 10.0f) + waveOffset;
            var spawnPosition = GameManager.Instance.Player.transform.position.AddVec2(randomOffset);

            SpawnEnemy(enemyPrefab, spawnPosition);

            await System.Threading.Tasks.Task.Delay((int)(m_spawnCooldown * 10));
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return m_enemyPrefabs[Random.Range(0, m_enemyPrefabs.Length)];
    }

    private void SpawnRandomEnemy()
    {
        var enemyPrefab = GetRandomEnemyPrefab();
        var playerOffset = Random.insideUnitCircle.normalized * m_spawnDistance;
        var spawnPosition = GameManager.Instance.Player.transform.position.AddVec2(playerOffset);
        SpawnEnemy(enemyPrefab, spawnPosition);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition)
    {
        // Since we do async spawning, make sure this doesn't run in edit-mode
        if (this == null)
            return;

       Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
