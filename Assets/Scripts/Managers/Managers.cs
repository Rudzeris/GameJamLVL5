using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(StatisticsManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(MissionManager))]
[RequireComponent(typeof(DataManager))]
[AddComponentMenu("Managers/Managers")]
public class Managers : MonoBehaviour
{
    [SerializeField] private float totalStartupTimeout = 5f;
    public static AudioManager Audio { get; private set; }
    public static StatisticsManager Statistics { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static PlayerManager Player { get; private set; }
    public static MissionManager Mission { get; private set; }
    public static DataManager Data { get; private set; }
    private List<IGameManager> _startSequence;

    private static Managers managers;

    private void Awake()
    {
        if (managers != null && !managers.gameObject.IsDestroyed())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        managers = this;
        Audio = GetComponent<AudioManager>();
        Statistics = GetComponent<StatisticsManager>();
        Inventory = GetComponent<InventoryManager>();
        Player = GetComponent<PlayerManager>();
        Mission = GetComponent<MissionManager>();
        Data = GetComponent<DataManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Audio);
        _startSequence.Add(Statistics);
        _startSequence.Add(Inventory);
        _startSequence.Add(Player);
        _startSequence.Add(Mission);
        _startSequence.Add(Data);
    }
    private void Start()
    {
        StartCoroutine(StartupManagers());
    }
    private IEnumerator StartupManagers()
    {
        bool allStarted = _startSequence.TrueForAll(manager => manager.Status == ManagerStatus.Started);

        foreach (IGameManager manager in _startSequence)
            manager.Startup();

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        float seconds = 0;

        while (numReady < numModules && seconds <= totalStartupTimeout)
        {
            int lastReady = numReady;
            numReady = 0;
            seconds += Time.deltaTime;

            foreach (IGameManager manager in _startSequence)
                if (manager.Status == ManagerStatus.Started)
                    numReady++;
            if (numReady > lastReady)
            {
                Debug.Log($"Progress: {numReady}/{numModules}");
                try
                {
                    Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Managers Startup: {exc.Message}");
                }
            }
        }
        if (numReady == numModules)
        {
            Debug.Log("All managers started up");
            try
            {
                if (!allStarted)
                    Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
            }
            catch (Exception exc)
            {
                Debug.LogError($"Managers Startup: {exc.Message}");
            }
        }
        else
            Debug.LogError("Error started managers");
    }
}