using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class LeaderboardListEntry : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerTimeText;
    [SerializeField] private TextMeshProUGUI playerPercentText;

    internal void SetData(Unity.Services.Leaderboards.Models.LeaderboardEntry leaderboardItems)
    {
        playerNameText.text = Convert.CutHashtagAndEnding(leaderboardItems.PlayerName);
        Debug.Log("Converting ms "+leaderboardItems.Score + " = "+Convert.MStoTimeString(leaderboardItems.Score));
        playerTimeText.text = Convert.MStoTimeString(leaderboardItems.Score);

        ScoreMetadata scoreMetadata = JsonConvert.DeserializeObject<ScoreMetadata>(leaderboardItems.Metadata);

        playerPercentText.text = ((int)scoreMetadata.perc).ToString();   
    }

}
