using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int level
    {
        get { return GameData.Instance.currentLevel; }
        set
        {
            GameData.Instance.currentLevel = value;
            CurrentLevel = value;
            UIManager.Instance.UpdateLevel(value);
            GameData.Instance.Save();
            // Do all other UI update shit here in the future
        }
    }
    public int lives
    {
        get { return CurrencyManager.Instance.GetCurrency(CurrencyTypeEnum.Life); }
        set
        {
            CurrencyManager.Instance.SetCurrency(CurrencyTypeEnum.Life, value);
            UIManager.Instance.UpdateUI(CurrencyTypeEnum.Life);
        }
    }

    public GameStateEnum gameState
    {
        get { return GameState; }
        set
        {
            if (GameState != value)
            {
                PreviousGameState = GameState;
                GameState = value;
            }
        }
    }
    [Header("Game Setting Properties")]
    /*public int MaxSpotCount = 32;
    public int MaxHoleIntake = 16;
    public int MaxHoleSearch = 32;*/
    public PrefabsSO Prefabs;
    public LevelsSO Maps;
    public int reviveCost = 900;
    public int lifeRefillCost = 900;
    public int maxLives = 5;
    public float reviveTimer = 20f;
    public float speedFactor = 1f;

    [Header("In Game Properties")]
    public GameStateEnum GameState = GameStateEnum.NONE;
    public GameStateEnum PreviousGameState = GameStateEnum.NONE;
    public int CurrentLevel = 0;
    public float currentTimer = 0f;
    public MapController currentMap;
    
    [HideInInspector] public float gameSpeed = 1f;
    [HideInInspector] public bool inTermination = false;

    private bool canRevive = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        gameSpeed = 1f / speedFactor;

        // Initialize everything here
        SettingData.Instance.Load();
        CurrencyData.Instance.Load();
        GameData.Instance.Load();
        TutorialData.Instance.Load();

        level = GameData.Instance.currentLevel;
        if (level > Maps.Levels.Count - 1) 
            level = Maps.Levels.Count - 1;

        CurrentLevel = level;
        CurrencyManager.Instance.INIT();
        UIManager.Instance.INIT();
        // ==========================
        
        AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_BG, transform);
        MapLoad();
    }

    private void Update()
    {
        if (GameState != GameStateEnum.PLAYING || currentMap == null) { return; }
        if (currentTimer > 0f)
        {
            currentTimer -= Time.deltaTime;
            UIManager.Instance.UpdateGameTimer(currentTimer);
        }
        else
        {
            if (canRevive)
                GameTimeout();
            else
                GameLose();
        }
    }

    public void MapLoad()
    {
        if (inTermination) return;
        // Clean up
        if (currentMap != null)
        {
            inTermination = true;
            Destroy(currentMap.gameObject);
        }

        gameState = GameStateEnum.NONE;
        GameObject map = Instantiate(Maps.Levels[CurrentLevel]);
        currentMap = map.GetComponent<MapController>();
        if (currentMap != null)
        {
            currentMap.INIT();
            FunctionManager.Instance.DelayFunction(1f, () => { inTermination = false; });
            currentTimer = currentMap.Timer;
            UIManager.Instance.UpdateLevel(CurrentLevel);
            UIManager.Instance.UpdateGameTimer(currentTimer);
        }
    }

    public void GameWin()
    {
        //Debug.Log("Game Win");
        gameState = GameStateEnum.WIN;
        FunctionManager.Instance.DelayFunction(0.5f, () =>
        {
            AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_WIN, transform);
            UIManager.Instance.OpenWin();
        });
        
    }

    public void GameTimeout()
    {
        //Debug.Log("Game Lose");
        gameState = GameStateEnum.TIMEOUT;
        UIManager.Instance.OpenLose();
    }

    public void GameLose()
    {
        gameState = GameStateEnum.LOSE;
        UIManager.Instance.OpenRetry();
    }

    public void GameRevive(bool free)
    {
        currentTimer = 20f;
        gameState = GameStateEnum.NONE;
        canRevive = false;
        UIManager.Instance.UpdateGameTimer(currentTimer);
    }

    public void GameRetry()
    {
        // Lose life is counted here
        if (PreviousGameState == GameStateEnum.PLAYING)
        {
            lives--;
        }
        MapLoad();
        PreviousGameState = GameStateEnum.NONE;
    }

    public void NextLevel() 
    {
        if (level < Maps.Levels.Count - 1) level++;
        // This is assuming the game is in testing phase
        if (CurrentLevel != level) CurrentLevel++;
        MapLoad();
    }

    public void ReceiveReward(CurrencyTypeEnum type, RewardTypeEnum reward, float mult)
    {
        var value = 0;
        switch (reward)
        {
            case RewardTypeEnum.WIN:
                value = (int)currentMap.RewardCoin;
                break;
        }
        switch (type)
        {
            case CurrencyTypeEnum.Coin:
                CurrencyManager.Instance.AddCurrency(type, value * mult);
                break;
            case CurrencyTypeEnum.Life:
                break;
        }
        UIManager.Instance.UpdateUI(type);
    }
}
