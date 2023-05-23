using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gotchi.Bot.Presenter;

public class GotchiManager : MonoBehaviour
{
    #region Public Variables
    public static GotchiManager Instance = null;

    public enum AttackType
    {
        Basic,
        Spin,
    }

    public Transform Spawn
    {
        get { return spawn; }
    }

    public GameObject GotchiPrefab
    {
        get { return gotchiPrefab; }
    }

    public List<GameObject> Bots { get { return spawnedBots; } }

    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject gotchiPrefab = null;

    [SerializeField] private GameObject botGotchiPrefab = null;

    [Header("Attributes")]
    [SerializeField] private Transform spawn = null;
    #endregion

    #region Private Variables
    private List<GameObject> spawnedBots = new List<GameObject>();
    private Dictionary<int, GotchiBotPresenter> BotLookup = new Dictionary<int, GotchiBotPresenter>();
    #endregion

    #region Unity Functions
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Functions
    public void SpawnBots(int numBots)
    {
        for (int i = 0; i < numBots; i++)
        {
            GameObject bot = Instantiate(botGotchiPrefab, Vector3.zero, Quaternion.identity, transform);
            GotchiBotPresenter presenter = bot.GetComponent<GotchiBotPresenter>();
            presenter.SetUsername("bot_" + (i + 1));
            
            spawnedBots.Add(bot);
            BotLookup.Add(bot.GetInstanceID(), presenter);
        }
    }

    public void RemoveBot()
    {
        GameObject removedBot = spawnedBots[spawnedBots.Count - 1];
        spawnedBots.RemoveAt(spawnedBots.Count - 1);
        Destroy(removedBot);
    }

    #endregion
}
