using UnityEngine;
using TMPro;

public class TestWaveSpawnManager : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    // [SerializeField] private Transform[] enemySpawnPoints = null;
    [SerializeField] private TextMeshProUGUI timeBetweenWavesText = null;
    [SerializeField] private TextMeshProUGUI numWaveText = null;

    [Header("Attributes")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int maxNumWaves = 5;
    #endregion

    #region Private Variables
    private float timeBetweenWavesTracker = 0f;
    private int numWave = 1;
    #endregion

    #region Unity Functions
    void Update()
    {
        if (numWave > maxNumWaves) return;

        bool isReadyForWave = timeBetweenWavesTracker <= 0f;
        if (isReadyForWave)
        {
            spawnWave();
            timeBetweenWavesTracker = timeBetweenWaves;
        }

        timeBetweenWavesTracker -= Time.deltaTime;
        timeBetweenWavesTracker = Mathf.Clamp(timeBetweenWavesTracker, 0f, Mathf.Infinity);
        string countdownText = string.Format("{0:00.00}", timeBetweenWavesTracker);

        if (numWave > maxNumWaves)
        {
            timeBetweenWavesText.text = numWave > maxNumWaves ? "X" : countdownText;
        }
        else
        {
            timeBetweenWavesText.text = countdownText;
        }
    }

    void OnEnable()
    {
        timeBetweenWavesText.gameObject.transform.parent.gameObject.SetActive(true);
        numWaveText.gameObject.transform.parent.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        timeBetweenWavesText.gameObject.transform.parent.gameObject.SetActive(false);
        numWaveText.gameObject.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    #region Private Functions
    private void spawnWave()
    {
        numWaveText.text = numWave.ToString();
        numWave++;

        spawnEnemies();
    }

    private void spawnEnemies()
    {
        // for (int i = 0; i < enemySpawnPoints.Length; i++)
        // {
        //     Transform spawnPoint = enemySpawnPoints[i];
        //     if (i < 7)
        //     {
        //         enemyManager.SpawnEnemy(spawnPoint.position, spawnPoint.rotation, EnemyManager.EnemyType.PawnLickquidator);
        //     }
        //     else if (i >= 6 && i < 9)
        //     {
        //         enemyManager.SpawnEnemy(spawnPoint.position, spawnPoint.rotation, EnemyManager.EnemyType.AerialLickquidator);
        //     }
        //     else
        //     {
        //         enemyManager.SpawnEnemy(spawnPoint.position, spawnPoint.rotation, EnemyManager.EnemyType.BossLickquidator);
        //     }
        // }
    }
    #endregion
}
