using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private LeaderboardListEntry leaderboardEntryPrefab;  
    [SerializeField] private Transform leaderboardHolder;  

    private string leaderboardCompletionistID = "100percent";
    private string leaderboardSpeedID = "speed";



    private async void Start()
    {
        Debug.Log("** Initializing Unity Services");
        await UnityServices.InitializeAsync();
        Debug.Log("** Signing in Anonomously");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        UpdateLeaderboard(0);
        //await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardCompletionistID, 0d);
    }

    // Add player score
    private async void AddPlayerScoreAsync(string playerName, float playerScore, float percent)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        try {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(percent == 100f ? leaderboardCompletionistID : leaderboardSpeedID, playerScore);
        }
        catch (LeaderboardsException e){

            Debug.Log("Could not add player score: "+e.Message);    
        }
    }
    
    // Only update when first loaded
    private async void UpdateLeaderboard(int leaderboardType)
    {
        Debug.Log("** Updating Leaderboard");
        LeaderboardScoresPage page = await LeaderboardsService.Instance.GetScoresAsync(leaderboardType == 0 ? leaderboardCompletionistID : leaderboardSpeedID);

        // Remove all present items
        foreach (Transform leaderboardEntry in leaderboardHolder.transform) {
            Destroy(leaderboardEntry.gameObject);
        }

        if(page.Results.Count == 0) {
            Debug.Log("** Results are empty can not create any entries in the leaderboard list");
            AddPlayerScoreAsync("Testplayer",100f,100f); 
            UpdateLeaderboard(0);
            return;
        }

        // Create all new entries
        foreach (LeaderboardEntry leaderboardItems in page.Results) {
            LeaderboardListEntry listEntry = Instantiate(leaderboardEntryPrefab, leaderboardHolder,false);
            listEntry.SetData(leaderboardItems);
        }


    }

}
