using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

// WIN, WEBL, UNITY
public enum SystemIndexes { Win,WebGL,Unity,Linux,Android}

public partial class LeaderboardConnect : MonoBehaviour
{
    private string leaderboardCompletionistID = "100percent";
    private string leaderboardSpeedID = "speed";
    public static LeaderboardConnect Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private async void Start()
    {
        Debug.Log("** Initializing Unity Services");
        await UnityServices.InitializeAsync();
        Debug.Log("** Signing in Anonomously");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }



    // Add player score
    public async void AddPlayerScoreAsync(string playerName, float playerScore, float percent)
    {
        string sanitized = playerName.Replace(" ", "_");

        await AuthenticationService.Instance.UpdatePlayerNameAsync(sanitized);

        int systemUsed = SystemIDController.Instance.SystemID.GetSystemID();

        var scoreMetadata = new ScoreMetadata { perc = percent, systemID = systemUsed, versionString = Application.version};

        string metadataJson = JsonConvert.SerializeObject(scoreMetadata);


        int maxRetries = 8;

        for (int attempt = 0; attempt < maxRetries; attempt++) {

            try {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(percent == 100f ? leaderboardCompletionistID : leaderboardSpeedID, playerScore, new AddPlayerScoreOptions { Metadata = scoreMetadata });
                FindFirstObjectByType<WinScreenScroll>()?.SetSuccessText("Success");
                return;
            }
            catch (LeaderboardsException e) {

                Debug.Log("Could not add player score: " + e.Message);
                FindFirstObjectByType<WinScreenScroll>()?.SetSuccessText("Failed\nAttempt "+attempt+" Error! " + e.Message + " trying to send score again in 1 second.");
                if (attempt == maxRetries) {
                    Debug.LogError("All retry attempts failed.");
                    FindFirstObjectByType<WinScreenScroll>()?.SetSuccessText("Failed\nFinal Error: Could not save score after " + maxRetries+" attempts. Cause:" + e.Message);
                    return;
                }
                else {
                    await Task.Delay(1000); // Wait before next retry 
                }
            }
        }
    }
    
    // Only update when first loaded
    public async Task<LeaderboardScoresPage> UpdateLeaderboard(int leaderboardType)
    {
        Debug.Log("** Updating Leaderboard");
        return await LeaderboardsService.Instance.GetScoresAsync(leaderboardType == 0 ? leaderboardCompletionistID : leaderboardSpeedID, new GetScoresOptions { IncludeMetadata = true });
        //return await LeaderboardsService.Instance.GetScoresAsync(leaderboardType == 0 ? leaderboardCompletionistID : leaderboardSpeedID);
    }

}
