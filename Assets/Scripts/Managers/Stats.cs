using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Wolfheat.StartMenu;
using Debug = UnityEngine.Debug;
public class Stats : MonoBehaviour
{
    [SerializeField] SledgeHammerFlicker sledgeHammerFlicker;
    [SerializeField] GameObject itemsHolder;

    public float MiningSpeed { get => miningSpeed; }
    private float miningSpeed;
    public const float MiningSpeedDefault = 3f;
    public const float MiningSpeedSpeedUp = 6f;

    public int Minerals { get => minerals; }
    public int minerals = 0;

    public int Damage { get => damage; }
    private int damage;
    public const int DamageDefault = 1;
    public const int DamageBoosted = 30;
    public const int MineralsToGetSeeThrough = 100;
    //public const float MiningSpeedSpeedUp = 12f;

    public const int MaxHealth = 10;
    public int CurrentMaxHealth { get; private set; } = 1;
    public int Health { get; private set; } = 1;
    public int Bombs { get; private set; } = 0;
    public int DeathCount { get; private set; } = 0;
    public bool IsInRegainArea { get; set; } = false;

    public bool IsDead { get; set; } = false;

    public static Stats Instance { get; private set; }
    public bool HasSledgeHammer { get; private set; } = false;
    public bool[] MineralsOwned { get; private set; } = new bool[4];

    private bool[] MineralsSeeThroughActivated = new bool[4];

    public Vector3 SavedStartPosition { get; private set; } = new Vector3();
    public float MovingSpeedMultiplier { get; internal set; } = 1f;
    public bool HasTeleported { get; internal set; } = false;

    private Stopwatch stopwatch = new();
    private TimeSpan bossStartTime;

    [SerializeField] GameObject[] ActivationMinerals;

    public static Action<int> HealthUpdate;
    public static Action<int> BombUpdate;
    public static Action MineralsUpdate;
    public static Action MineralsAmountUpdate;
    public static Action NoMoreMineralsReached;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        miningSpeed = MiningSpeedDefault;
        damage = DamageDefault;
        DeathCount = 0;

    }
    int startAmountItems = 0;
    private void Start()
    {
        if (MineralsOwned.Length != ActivationMinerals.Length)
            Debug.LogWarning("Place all Minerals references in Stats/ActivationMinerals, need " + MineralsOwned.Length);

        
        // Start Timer
        stopwatch.Start();

        startAmountItems = CountItems();
        Debug.Log("Start Children Items = "+startAmountItems+" This is the amount the player need to collect to achieve the Completionist");
    }
    internal void StoreBossStartTime() => bossStartTime = stopwatch.Elapsed;

    internal int GetCompletePercent()
    {
        int currentItems = CountItems();
        float percent = 100*(((float)startAmountItems - currentItems) / startAmountItems);
        Debug.Log("End Children Items "+currentItems+" percent = > "+percent);
        return (int)percent;
        //return percent == 100 ? "100%" : (percent.ToString("F1")+ "%");
    }

    // coins, bombs, keys, gems, maps
    private int CountItems() => itemsHolder.transform.GetComponentsInChildren<Usable>(false).ToArray().Length 
        + itemsHolder.transform.GetComponentsInChildren<PowerUp>(false).ToArray().Length;
    
    //private int CountItems() => itemsHolder.transform.GetComponentsInChildren<Usable>().Where(x => x.gameObject.activeSelf).ToArray().Length 
    //    + itemsHolder.transform.GetComponentsInChildren<PowerUp>().Where(x => x.gameObject.activeSelf).ToArray().Length;

    [SerializeField] Transform[] levelStartPositions;
    private int activeLevelStartPosition = 0;

    public void SetSpecificPosition(int newStartPosition)
    {
        activeLevelStartPosition = newStartPosition;
        SavedStartPosition = levelStartPositions[activeLevelStartPosition].position;
    }

    public bool GetNextStartPosition()
    {
        activeLevelStartPosition++;
        if (activeLevelStartPosition >= levelStartPositions.Length) {
            // Scene Completed
            Debug.Log("Scene Completed");
            return true;
        }
        SavedStartPosition = levelStartPositions[activeLevelStartPosition].position;
        return false;
    }

    internal void StopGameTimer()
    {
        stopwatch.Stop();
    }

    internal int GetElapsedTimeMS()
    {
        TimeSpan ts = stopwatch.Elapsed;
        return (int)ts.TotalMilliseconds;
    }
    internal int GetElapsedTimeS()
    {
        TimeSpan ts = stopwatch.Elapsed;
        return (int)ts.TotalSeconds;
    }
    internal int GetBossElapsedTimeS()
    {        
        TimeSpan ts = stopwatch.Elapsed - bossStartTime;
        return (int)ts.TotalSeconds;
    }

    public string GetElapsedTimeString() {

        stopwatch.Stop();

        TimeSpan ts = stopwatch.Elapsed;

        if (ts.Hours > 0)
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        return String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

    }

    public void SetMovemenSpeedMultiplier(float multiplier) => MovingSpeedMultiplier = multiplier;

    public void SetDefaultSledgeHammer()
    {
        sledgeHammerFlicker.SetFlicker(false);
        miningSpeed = MiningSpeedDefault;
        damage = DamageDefault;

    }

    public void SetBoostSledgeHammer()
    {
        sledgeHammerFlicker.SetFlicker(true);
        miningSpeed = MiningSpeedSpeedUp;
        damage = DamageBoosted;
    }

    public void DefineGameDataForSave()
    {
        // Player position and looking direction (Tilt is disregarder, looking direction is good enough)
        //SavingUtility.playerGameData.PlayerPosition = SavingUtility.Vector3AsV3(rb.transform.position);
        //SavingUtility.playerGameData.PlayerRotation = SavingUtility.Vector3AsV3(rb.transform.forward);

        // Inventory

        // Health, Oxygen
        //SavingUtility.playerGameData.PlayerHealth = health;
        //SavingUtility.playerGameData.PlayerOxygen = oxygen;

    }

    public void OxygenHealthRemoval() => Health = 0;

    public bool TakeDamage(int amt)
    {
        Health -= amt;
        Health = Math.Max(Health, 0);
        Debug.Log("Player lose health " + Health);

        HealthUpdate?.Invoke(Health);

        if (Health <= 0) {
            Debug.Log("Player is dead");

            IsDead = true;
            return true;
        }
        return false;
    }

    internal void Revive()
    {
        Debug.Log("Revive");
        if (savedValues.valuesSet) {
            Debug.Log("Revive With Saved Values");
            // Boss revive sets health to max along with all saved values from entering Boss area
            LoadBossValues();
            // Reset Shops
            Shop.Instance.ResetShop();
        }
        else {
            // Normal revive only sets health to max
            Health = CurrentMaxHealth;
        }
        DeathCount++;
        IsDead = false;
        HealthUpdate?.Invoke(Health);
        SoundMaster.Instance.AddRestartSpeech();
    }

    internal bool Heal()
    {
        if (Health == CurrentMaxHealth)
            return false;
        //SoundMaster.Instance.PlaySound(SoundName.YourWoundsAreHealed);
        Health = CurrentMaxHealth;
        HealthUpdate?.Invoke(Health);
        return true;
    }

    internal void AddHealth(int value)
    {
        CurrentMaxHealth = Math.Min(CurrentMaxHealth + value, MaxHealth);
        Health = CurrentMaxHealth;
        HealthUpdate?.Invoke(Health);
    }


    internal void AddBomb(int amount)
    {
        Bombs += amount;
        BombUpdate?.Invoke(Bombs);
    }
    internal void RemoveBombs(int amount)
    {
        Bombs = Math.Max(0, Bombs - amount);
        BombUpdate?.Invoke(Bombs);
    }

    [SerializeField] GameObject sledgeHammerCamera;
    internal void ActivateSledgeHammer()
    {
        sledgeHammerCamera.GetComponent<Camera>().enabled = true;
        HasSledgeHammer = true;
    }

    internal void ActivateCompass()
    {
        UIController.Instance.ActivateCompass();
    }

    internal void SetNewRespawnPoint(Vector3 point)
    {
        SavedStartPosition = new Vector3(Mathf.RoundToInt(point.x), 0, Mathf.RoundToInt(point.z));
    }

    internal void ActivateMap() => UIController.Instance.ActivateMap();

    private BossSaveValues savedValues = new BossSaveValues();

    internal void LoadBossValues()
    {
        Debug.Log("Player Died Reset Boss Save Values");

        CurrentMaxHealth = savedValues.playerCurrentMaxHealth;
        Health = CurrentMaxHealth;

        Inventory.Instance.SetBombs(savedValues.bombsHeld);
        Inventory.Instance.SetCoins(savedValues.coinsHeld);

        MovingSpeedMultiplier = savedValues.movingSpeedMult;
        Debug.Log("Moving speed multiplier is now "+MovingSpeedMultiplier);

        // Updates the graphics to correct health
        HealthUpdate?.Invoke(Health);
    }
    internal void SaveBossValues()
    {
        Debug.Log("Set Boss Save Values");
        // Player Hearts
        // Player Speed
        // No Chicken, No firespell, no Banana
        // Bombs
        savedValues = new BossSaveValues(Health, CurrentMaxHealth, Inventory.Instance.BombsHeld, Inventory.Instance.CoinsHeld, MovingSpeedMultiplier, true);

        // Also reset Enemy when player dies
    }

    public struct BossSaveValues
    {
        public int playerCurrentHealth;
        public int playerCurrentMaxHealth;
        public int bombsHeld;
        public int coinsHeld;
        public float movingSpeedMult;
        public bool valuesSet;

        public BossSaveValues(int playerHealth = 0, int playerMaxHealth = 0, int bombs=0, int coins=0, float moveSpeed = 1, bool isSet = false)
        {
            playerCurrentHealth = playerHealth;
            playerCurrentMaxHealth = playerMaxHealth;
            bombsHeld = bombs;
            coinsHeld = coins;
            movingSpeedMult = moveSpeed;
            valuesSet = isSet;
        }
    }

}
