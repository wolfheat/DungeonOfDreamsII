using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class LeaderboardListEntry : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerTimeText;
    [SerializeField] private TextMeshProUGUI playerPercentText;

    internal void SetData(Unity.Services.Leaderboards.Models.LeaderboardEntry leaderboardItems, int index = 0)
    {
        indexText.text = index.ToString();
        playerNameText.text = Convert.CutHashtagAndEnding(leaderboardItems.PlayerName);
        Debug.Log("Converting ms "+leaderboardItems.Score + " = "+Convert.MStoTimeString(leaderboardItems.Score));
        playerTimeText.text = Convert.MStoTimeString(leaderboardItems.Score);

        Debug.Log("Metadata string = "+leaderboardItems.Metadata);

        // Percent completed metadata handeling
        if(leaderboardItems.Metadata == null || leaderboardItems.Metadata.Length == 0)
            playerPercentText.text = "XX%";
        else {
            ScoreMetadata scoreMetadata = JsonConvert.DeserializeObject<ScoreMetadata>(leaderboardItems.Metadata);
            playerPercentText.text = ((int)scoreMetadata.perc).ToString()+"%";
        }
    }

}
