using System;
using TMPro;
using UnityEngine;

public class LeaderboardListEntry : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerTimeText;

    internal void SetData(Unity.Services.Leaderboards.Models.LeaderboardEntry leaderboardItems)
    {
        playerNameText.text = leaderboardItems.PlayerName;
        playerTimeText.text = ((float)leaderboardItems.Score).ToString();
    }

}
