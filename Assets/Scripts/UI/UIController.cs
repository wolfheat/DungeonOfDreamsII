using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
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
    [SerializeField] GameObject map;

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
        Debug.Log("UI CONTROLLER INSTANCE SET");

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
        Debug.Log("Tilde");
        helpScreen.gameObject.SetActive(!helpScreen.gameObject.activeSelf);
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
        //if (playerStats.IsDead) return;

        bool doPause = GameState.state == GameStates.Running;
        Pause(doPause);
        pauseScreen.SetActive(doPause);
    }

    public void Pause(bool pause = true)
    {
        GameState.state = pause ? GameStates.Paused : GameStates.Running;
        Debug.Log("Gamestate set to " + GameState.state);
        Time.timeScale = pause ? 0f : 1f;
    }

    public void UpdateShownItemsUI(List<ItemData> data,bool resetList = false)
	{
		interactableUI.UpdateItems(data,resetList);
	}
	
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
    }
    
    public void ShowWinScreen()
	{

        string winTime = Stats.Instance.GetElapsedTime();

        winScreen.SetCompleteTimeText(winTime);

        SoundMaster.Instance.PlaySpeech(SoundName.ExitSpeech,true);

        // Pausing makes scroll not active
        Pause(true);

        Debug.Log("Show End Screen after transition");
        // Transition to Dark
		transitionScreen.Darken();
        open = UIActions.WinScreen;
	}

    private UIActions open = UIActions.None;
    //private UIActions close = UIActions.None;

    public enum UIActions {None,DeathScreen,WinScreen}

    public void TransitionComplete()
	{
        Debug.Log("Transition Complete");        
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
        Debug.Log("Go to Main menu, unload Dungeon, load start menu");
        SoundMaster.Instance.ResetMusic();

        //SceneManager.UnloadSceneAsync("Dungeon");
        //SceneManager.UnloadSceneAsync("DreamsDungeon2");
        if (SceneManager.GetSceneByName("DreamsDungeon2").IsValid()) {
            SceneManager.UnloadSceneAsync("DreamsDungeon2");
            //SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("DreamsDungeon2"));
            Debug.Log("Unloaded Scene DreamsDungeon2");
        }

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

    internal void ActivateMap(bool activate = true) => map.SetActive(activate);
    internal bool MapIsActive() => map.activeSelf;
}
