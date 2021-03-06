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

    [SerializeField] private Bounds m_spawnBounds;
    [SerializeField] private float m_spawnDistance;
    [SerializeField] public float m_maxTimeDifficultyScaling;
    [SerializeField] AnimationCurve m_waveAmount;

    [SerializeField] AnimationCurve m_healthAmount;

    [SerializeField] GameObject[] m_enemyPrefabs;

    private float m_difficultyTimeScale = 1.0f; // Used for cheats/skipping time

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

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_difficultyTimeScale = Mathf.Infinity;
            if (GameManager.Instance.Player == null)
                return;
            var playerTransform = GameManager.Instance.Player.transform;
            FloatingTextManager.Instance.SpawnFloatingText("DIFFICULTY MAXED", playerTransform.position, playerTransform);
        }
    }

    private async void SpawnEnemyWave()
    {
        var enemyPrefab = GetRandomEnemyPrefab();
        var waveOffset = Random.insideUnitCircle.normalized * m_spawnDistance;

        int spawnAmount = (int)m_waveAmount.Evaluate(GetDifficultyT());

        for (int i = 0; i < spawnAmount; i++)
        {
            if (GameManager.Instance.Player == null)
                return;

            var randomOffset = (Random.insideUnitCircle.normalized * 10.0f) + waveOffset;
            var spawnPosition = GameManager.Instance.Player.transform.position.AddVec2(randomOffset);

            SpawnEnemy(enemyPrefab, spawnPosition);

            await System.Threading.Tasks.Task.Delay((int)(m_spawnCooldown * 10));
        }
    }

    private float GetDifficultyT()
    {
        return Mathf.Clamp01((Time.timeSinceLevelLoad * m_difficultyTimeScale) / m_maxTimeDifficultyScaling);
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return m_enemyPrefabs[Random.Range(0, m_enemyPrefabs.Length)];
    }

    private void SpawnRandomEnemy()
    {
        if (GameManager.Instance.Player == null)
            return;

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
        
        spawnPosition = m_spawnBounds.ClosestPoint(spawnPosition);
        if (Vector3.Distance(GameManager.Instance.Player.transform.position, spawnPosition) < 20.0f)
            return;

        var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        float maxHealth = m_healthAmount.Evaluate(GetDifficultyT());
        enemy.GetComponent<HealthComponent>().SetMaxHealth(maxHealth);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1.0f, 0.92f, 0.016f, 0.7f);
        Gizmos.DrawCube(m_spawnBounds.center, m_spawnBounds.size);
    }
}
