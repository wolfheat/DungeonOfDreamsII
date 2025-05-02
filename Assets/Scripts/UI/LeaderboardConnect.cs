using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Internal.Models;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

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
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        var scoreMetadata = new ScoreMetadata { perc = percent };

        string metadataJson = JsonConvert.SerializeObject(scoreMetadata);

        try {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(percent == 100f ? leaderboardCompletionistID : leaderboardSpeedID, playerScore, new AddPlayerScoreOptions { Metadata = metadataJson });
        }
        catch (LeaderboardsException e){

            Debug.Log("Could not add player score: "+e.Message);    
        }
    }
    
    // Only update when first loaded
    public async Task<LeaderboardScoresPage> UpdateLeaderboard(int leaderboardType)
    {
        Debug.Log("** Updating Leaderboard");
        return await LeaderboardsService.Instance.GetScoresAsync(leaderboardType == 0 ? leaderboardCompletionistID : leaderboardSpeedID);
    }

}
