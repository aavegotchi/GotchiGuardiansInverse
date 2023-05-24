using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gotchi.Player.Presenter;
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

    public List<GameObject> Gotchis { get { return spawnedGotchis; } }

    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject gotchiPrefab = null;

    [SerializeField] private GameObject botGotchiPrefab = null;

    [Header("Attributes")]
    [SerializeField] private Transform spawn = null;
    #endregion

    #region Private Variables
    private List<GameObject> spawnedGotchis = new List<GameObject>();
    private List<GameObject> spawnedBots = new List<GameObject>();
    private Dictionary<int, GotchiPresenter> gotchiLookup = new Dictionary<int, GotchiPresenter>();
    private Dictionary<int, GotchiBotPresenter> botLookup = new Dictionary<int, GotchiBotPresenter>();
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
            botLookup.Add(bot.GetInstanceID(), presenter);
        }
    }

    public void RemoveBot(int botId)
    {
        int botIndex = spawnedBots.FindIndex(go => go.GetInstanceID() == botId);

        if (botIndex != -1) {
            spawnedBots.RemoveAt(botIndex);
            botLookup.Remove(botId);
        }
    }

    public void RemoveBot()
    {
        GameObject removedBot = spawnedBots[spawnedBots.Count - 1];
        spawnedBots.RemoveAt(spawnedBots.Count - 1);
        botLookup.Remove(removedBot.GetInstanceID());
        Destroy(removedBot);
    }

    public void RegisterGotchi(GotchiPresenter NewGotchi)
    {
        int id = NewGotchi.gameObject.GetInstanceID();
        spawnedGotchis.Add(NewGotchi.gameObject);
        gotchiLookup.Add(id, NewGotchi);
    }

    public void RemoveGotchi(GotchiPresenter Gotchi)
    {
        spawnedGotchis.Remove(Gotchi.gameObject);
        gotchiLookup.Remove(Gotchi.gameObject.GetInstanceID());
    }

    public int GetLiveGotchiCount()
    {
        int totalLiveGotchis = 0;
        spawnedGotchis.ForEach(delegate(GameObject go) {
            if (!gotchiLookup[go.GetInstanceID()].IsDead())
            {
                totalLiveGotchis++;
            }
        });
        
        return totalLiveGotchis;
    }

    public int GetLiveBotCount()
    {
        int totalLiveBots = 0;
        spawnedBots.ForEach(delegate(GameObject go) {
        if (!botLookup[go.GetInstanceID()].IsDead())
        {
            totalLiveBots++;
        }
        });
        
        return totalLiveBots;
    }
    #endregion
}
