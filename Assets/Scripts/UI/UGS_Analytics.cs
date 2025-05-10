using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;


public class UGS_Analytics : MonoBehaviour
{
    private string GameCompleted = "GameCompleted";
    private string GameAbandoned = "AbandonGameToMenu";
    private string BossDefeated = "BossDefeated";
    private string ReachedShop = "ReachedShop";
    private string BlownUp = "BlownUp";



    private string levelID = "LevelID";
    private string defeatAtBossAfterSeconds = "DefeatAtBossAfterSeconds";
    private string bombsHeld = "BombsHeld";
    private string bombsUsed = "BombsUsed";
    private string deathCount = "DeathCount";
    private string maxBombs = "MaxBombsUsedAtSameTime";
    private string playTime = "PlayTime";

    public static UGS_Analytics Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    async void Start()
    {
        try {
            await UnityServices.InitializeAsync();
            GiveConsent(); //Get user consent according to various legislations
        }
        catch {
            SpecialInfo.Instance.ShowInfo("UGS consent failed");
        }
    }

    public void GiveConsent()
    {
        // Call if consent has been given by the user
        AnalyticsService.Instance.StartDataCollection();
    }

    // EVENTS
    public void GameCompletedEvent()
    {
        // Define Custom Event Parameters
        var customEvent = new CustomEvent(GameCompleted);
        customEvent[levelID] = 0;
        customEvent[bombsHeld] = Inventory.Instance.BombsHeld;
        customEvent[bombsUsed] = Inventory.Instance.BombsUsed;
        customEvent[deathCount] = Stats.Instance.DeathCount;
        customEvent[playTime] = Stats.Instance.GetElapsedTimeS();

        SendEvent(customEvent);        
    }

    public void GameAbandonedEvent()
    {
        // Define Custom Event Parameters
        var customEvent = new CustomEvent(GameAbandoned);
        customEvent[levelID] = 0;
        customEvent[bombsHeld] = Inventory.Instance.BombsHeld;
        customEvent[bombsUsed] = Inventory.Instance.BombsUsed;
        customEvent[deathCount] = Stats.Instance.DeathCount;
        customEvent[playTime] = Stats.Instance.GetElapsedTimeS();

        SendEvent(customEvent);        
    }
    
    public void ReachedShopEvent()
    {
        // Define Custom Event Parameters
        var customEvent = new CustomEvent(ReachedShop);
        customEvent[levelID] = 0;
        customEvent[playTime] = Stats.Instance.GetElapsedTimeS();
        SendEvent(customEvent);        
    }
     public void BossDefeatedEvent()
    {
        // Define Custom Event Parameters
        var customEvent = new CustomEvent(BossDefeated);
        customEvent[levelID] = 0;
        customEvent[defeatAtBossAfterSeconds] = Stats.Instance.GetBossElapsedTimeS();
        customEvent[bombsHeld] = Inventory.Instance.BombsHeld;
        customEvent[bombsUsed] = Inventory.Instance.BombsUsed;
        customEvent[deathCount] = Stats.Instance.DeathCount;
        customEvent[playTime] = Stats.Instance.GetElapsedTimeS();

        SendEvent(customEvent);        
    }

    public void BlownUpEvent()
    {
        // Define Custom Event Parameters
        var customEvent = new CustomEvent(BlownUp);
        customEvent[levelID] = 0;
        customEvent[bombsHeld] = Inventory.Instance.BombsHeld;
        customEvent[bombsUsed] = Inventory.Instance.BombsUsed;
        customEvent[deathCount] = Stats.Instance.DeathCount;
        customEvent[playTime] = Stats.Instance.GetElapsedTimeS();

        SendEvent(customEvent);        
    }


    private void SendEvent(CustomEvent customEvent)
    {
        if (Stats.Instance.HasTeleported) {
            Debug.Log("Wont send analytics event when player has teleported during the playthrough");
            return;
        }

        // Record the event
        AnalyticsService.Instance.RecordEvent(customEvent);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
    }
}
