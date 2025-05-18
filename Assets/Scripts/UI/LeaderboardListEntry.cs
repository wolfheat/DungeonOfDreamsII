using System;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LeaderboardListEntry : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerTimeText;
    [SerializeField] private TextMeshProUGUI playerPercentText;
    [SerializeField] private LeaderboardSystemSetter systemSetter;

    internal void SetData(Unity.Services.Leaderboards.Models.LeaderboardEntry leaderboardItems, int index = 0)
    {
        indexText.text = index.ToString();
        //Debug.Log("Converting ms "+leaderboardItems.Score + " = "+Convert.MStoTimeString(leaderboardItems.Score));
        playerTimeText.text = Convert.MStoTimeString(leaderboardItems.Score);

        string sanitized = Convert.CutHashtagAndEnding(leaderboardItems.PlayerName);
        string unSanitized = sanitized.Replace("_", " ");

        playerNameText.text = unSanitized;

        //Debug.Log("Metadata string = "+leaderboardItems.Metadata);

        // Percent completed metadata handeling
        if (leaderboardItems.Metadata == null || leaderboardItems.Metadata.Length == 0)
            playerPercentText.text = "XX%";
        else {
            ScoreMetadata scoreMetadata = JsonConvert.DeserializeObject<ScoreMetadata>(leaderboardItems.Metadata);
            playerPercentText.text = ((int)scoreMetadata.perc).ToString()+"%";

            systemSetter.SetAsSystem(scoreMetadata.systemID,scoreMetadata.versionString);
            //playerNameText.text += AddSystemText(); 
        }

    }

    // Adding system text with sprite Asset
    private string AddSystemText(int systemIndex)
    {
        Debug.Log(this.name + " set to system type "+systemIndex);
        switch (systemIndex) {
            case 0:
                return "  <sprite=0>";
            case 1:
                return "  <sprite=1>";
            case 2:
                return "  <sprite=2>";
        }
        return "";
    }
}
