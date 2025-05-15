
using System.Collections;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Wolfheat.Inputs;

public class LeaderboardTableManager : MonoBehaviour
{
    [SerializeField] private LeaderboardListEntry leaderboardEntryPrefab;  
    [SerializeField] private Transform leaderboardHolder;  
    [SerializeField] private string[] leaderboardNames;  
    [SerializeField] private TextMeshProUGUI leaderboardNameText;  
    [SerializeField] private GameObject leftArrow;  
    [SerializeField] private GameObject rightArrow;  
    [SerializeField] private ScrollRect scrollRect;


    LeaderboardScoresPage[] leaderboardScoresPages = new LeaderboardScoresPage[TotalLeaderboards];

    // Manual Scrolling with keyboard
    private const float ManualScrollSpeed = 1.15f;
    private const float ManualScrollDampening = 0.1f;
    private const float ScrollTime = 0.3f;
    private float currentSpeed = 0;

    private int currentLeaderboard = 0;
    private const int TotalLeaderboards = 2;


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

    private void DirectionInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.x < -0.5f) {
            ShowPreviousLeaderboard();            
        }else if(input.x > 0.5f)
            ShowNextLeaderboard();
    }


    private void OnDisable() => StartMenuInputs.Instance.Controls.Player.Move.performed -= DirectionInput;

    private void Start()
    {
        UpdateLeaderboard(0);        
    }


    private void Update()
    {
        // Read move input from player
        Vector2 movementDetected = StartMenuInputs.Instance.Controls.Player.Move.ReadValue<Vector2>();
        if (movementDetected.y < -0.1f) 
            currentSpeed = -ManualScrollSpeed;
        else if (movementDetected.y > 0.1f) 
            currentSpeed = ManualScrollSpeed;
        

        if (currentSpeed == 0)
            return;

        currentSpeed *= Mathf.Pow(ManualScrollDampening, Time.unscaledDeltaTime/ScrollTime);

        if (currentSpeed < 0.05 && currentSpeed > -0.05) 
            currentSpeed = 0;
        scrollRect.verticalNormalizedPosition += currentSpeed * Time.unscaledDeltaTime;
    }

    public void ShowNextLeaderboard()
    {
        Debug.Log("NEXT");
        if (currentLeaderboard == TotalLeaderboards - 1) return;
        currentLeaderboard++;
        UpdateWithLeaderboard(currentLeaderboard);
    }
    
    public void ShowPreviousLeaderboard()
    {
        Debug.Log("PREV");
        if (currentLeaderboard == 0) return;
        currentLeaderboard--;
        UpdateWithLeaderboard(currentLeaderboard);
    }

    
    // Only update when first loaded
    private async void UpdateLeaderboard(int leaderboardType)
    {
        Debug.Log("** Updating Leaderboard");
        LeaderboardScoresPage page = await LeaderboardConnect.Instance.UpdateLeaderboard(1);
        LeaderboardScoresPage page2 = await LeaderboardConnect.Instance.UpdateLeaderboard(0);

        leaderboardScoresPages[0] = page;
        leaderboardScoresPages[1] = page2;
        UpdateWithLeaderboard(0);

        UpdateArrows(leaderboardType);
    }

    private void UpdateArrows(int leaderboardType)
    {
        Debug.Log("UpdateArrows "+leaderboardType+ " leaderboardType < TotalLeaderboards-1 => "+(leaderboardType < TotalLeaderboards - 1));
        leftArrow.SetActive(leaderboardType > 0);
        rightArrow.SetActive(leaderboardType < TotalLeaderboards-1);
    }

    private void UpdateWithLeaderboard(int pageIndex)
    {
        LeaderboardScoresPage page = leaderboardScoresPages[pageIndex];

        UpdateArrows(pageIndex);

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
