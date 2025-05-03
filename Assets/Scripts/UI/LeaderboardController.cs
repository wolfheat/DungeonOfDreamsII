
using System;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.Inputs;

public class LeaderboardTableManager : MonoBehaviour
{
    [SerializeField] private LeaderboardListEntry leaderboardEntryPrefab;  
    [SerializeField] private Transform leaderboardHolder;  
    [SerializeField] private string[] leaderboardNames;  
    [SerializeField] private TextMeshProUGUI leaderboardNameText;  
    public static LeaderboardTableManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable() => StartMenuInputs.Instance.Controls.Player.Move.performed += DirectionInput;

    private void DirectionInput(InputAction.CallbackContext context) => ShowNextLeaderboard();

    private void OnDisable() => StartMenuInputs.Instance.Controls.Player.Move.performed -= DirectionInput;

    private void Start()
    {
        UpdateLeaderboard(0);        
    }

    private int currentLeaderboard = 0;
    private const int TotalLeaderboards = 2;

    public void ShowNextLeaderboard()
    {
        currentLeaderboard = (currentLeaderboard + 1) % TotalLeaderboards;
        UpdateWithLeaderboard(currentLeaderboard);
    }

    LeaderboardScoresPage[] leaderboardScoresPages = new LeaderboardScoresPage[TotalLeaderboards];

    // Only update when first loaded
    private async void UpdateLeaderboard(int leaderboardType)
    {
        Debug.Log("** Updating Leaderboard");
        LeaderboardScoresPage page = await LeaderboardConnect.Instance.UpdateLeaderboard(1);
        LeaderboardScoresPage page2 = await LeaderboardConnect.Instance.UpdateLeaderboard(0);

        leaderboardScoresPages[0] = page;
        leaderboardScoresPages[1] = page2;
        UpdateWithLeaderboard(0);

    }

    private void UpdateWithLeaderboard(int pageIndex)
    {
        LeaderboardScoresPage page = leaderboardScoresPages[pageIndex];

        // Set Header
        leaderboardNameText.text = leaderboardNames[pageIndex];

        // Remove all present items
        foreach (Transform leaderboardEntry in leaderboardHolder.transform) {
            Destroy(leaderboardEntry.gameObject);
        }

        if (page.Results.Count == 0) {
            Debug.Log("** Results are empty can not create any entries in the leaderboard list");
            return;
        }
            
        // Create all new entries
        for (int i = 0; i < page.Results.Count; i++) {
            LeaderboardEntry leaderboardItems = page.Results[i];
            LeaderboardListEntry listEntry = Instantiate(leaderboardEntryPrefab, leaderboardHolder, false);
            listEntry.SetData(leaderboardItems,i+1);
        }
    }
}
