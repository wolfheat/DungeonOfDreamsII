using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Wolfheat.Inputs;
using Wolfheat.StartMenu;

public enum GameStates { Running, Paused }

public class UIController : MonoBehaviour
{
    [SerializeField] InteractableUI interactableUI;
    [SerializeField] TransitionScreen transitionScreen;
    [SerializeField] DeathScreenController deathScreen;
    [SerializeField] WinScreenScroll winScreen;
    [SerializeField] GameObject helpScreen;
    [SerializeField] OxygenController oxygenPanel;
    [SerializeField] Image mapMask;
    [SerializeField] GameObject bossHealthBar;

	public static UIController Instance { get; private set; }

    [SerializeField] PauseController pauseScreen;

    private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

    }
    public void OnEnable()
    {
        Inputs.Instance.Controls.Player.Esc.started += Pause;
        TransitionScreen.AnimationComplete += TransitionComplete;
        Inputs.Instance.Controls.Player.Tilde.performed += Tilde;

        Pause(false);
        // Initialize Helpscreen as deactivated
        //helpScreen.gameObject.SetActive(false);
    }

    private void Tilde(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR        
        helpScreen.gameObject.SetActive(!helpScreen.gameObject.activeSelf);
#endif
    }

    public void OnDisable()
    {

        Inputs.Instance.Controls.Player.Esc.started -= Pause;
        TransitionScreen.AnimationComplete -= TransitionComplete;
        Inputs.Instance.Controls.Player.Tilde.performed -= Tilde;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        // Player can not toggle pause when dead
        if (Stats.Instance.IsDead) return;

        // If buy menu is open close it instead of pause
        if (Shop.Instance.CloseIfOpen())
            return;

        bool doPause = GameState.state == GameStates.Running;
        Pause(doPause);
        pauseScreen.SetActive(doPause);
    }

    public void Pause(bool pause = true)
    {
        GameState.state = pause ? GameStates.Paused : GameStates.Running;
        Time.timeScale = pause ? 0f : 1f;
    }
    //
    //public void UpdateShownItemsUI(List<ItemData> data,bool resetList = false)
	//{
	//	interactableUI.UpdateItems(data,resetList);
	//}
	
	public void AddPickedUp(ItemData data)
	{
		interactableUI.AddPickedUp(data);
	}

    public void ShowDeathScreenInstant()
    {
        HideDarkening();
        deathScreen.Show();
    }

    public void ShowDeathScreen()
	{
        // Transition to Dark
        open = UIActions.DeathScreen;
        transitionScreen.Darken();


        SoundMaster.Instance.FadeMusic();
    }
    
    public void ShowWinScreen()
	{
        // Stop timer
        Stats.Instance.StopGameTimer();

        // Send Analytics for Game Complete
        UGS_Analytics.Instance.GameCompletedEvent();

        int completePercent = Stats.Instance.GetCompletePercent();
        if (completePercent== 100) {
            Debug.Log("Completionist!");
        }
        winScreen.ShowCompletionist(completePercent == 100);
        
        string completePercentText = completePercent + "%";



        winScreen.SetCompleteTimeText(Stats.Instance.GetElapsedTimeString());
        winScreen.SetCompletePercentText(completePercentText);
        winScreen.SetDeathsText(Stats.Instance.DeathCount);
        

        //SoundMaster.Instance.PlaySpeech(SoundName.ExitSpeech,true);

        // Pausing makes scroll not active
        Pause(true);

        // Transition to Dark
		transitionScreen.Darken();
        open = UIActions.WinScreen;

        // This wont happen if player has teleported
        SavingUtility.gameSettingsData.SendScoreToLeaderboard(Stats.Instance.GetElapsedTimeMS(),completePercent);
	}


    private UIActions open = UIActions.None;
    //private UIActions close = UIActions.None;

    public enum UIActions {None,DeathScreen,WinScreen}

    public void TransitionComplete()
	{
        switch (open)
        {
            case UIActions.None:
                break;
            case UIActions.DeathScreen:
                deathScreen.Show();
                break;
            case UIActions.WinScreen:
                winScreen.Show();
                break;
        }
        open = UIActions.None;
        /*
        switch (close)
        {
            case UIActions.None:
                break;
            case UIActions.DeathScreen:
                deathScreen.Hide();
                break;
        }
        close = UIActions.None;
        */
    }

    [SerializeField] GameObject compass;
    internal void ActivateCompass()
    {
        compass.SetActive(true);
    }

    internal void ToMainMenu()
    {
        SoundMaster.Instance.ResetMusic();
        SceneChanger.Instance.ChangeScene("StartMenu");
    }

    // Screen darkening image
    [SerializeField] Image image;

    // Post processing volume for darkening effect
    [SerializeField] Volume volume;

    internal void UpdateScreenDarkening(float percent)
    {
        // Setting darkening effect to specific percent
        image.color = new Color() { a = percent };
        volume.weight = percent;
        //Debug.Log("Updating screen darkening");
    }

    internal void HideDarkening()
    {
        image.color = new Color() { a = 0f };
    }

    internal void HideOxygen()
    {
        throw new NotImplementedException();
    }

    internal void ShowOxygen()
    {
        throw new NotImplementedException();
    }

    internal void ActivateMap(bool activate = true) => mapMask.enabled = !activate;

    internal bool MapIsActive() => mapMask != null && mapMask.gameObject.activeSelf;

    internal void ShowBossHealth(bool doShow = true) => bossHealthBar.SetActive(doShow);
}
