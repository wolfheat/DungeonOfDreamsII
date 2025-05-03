using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wolfheat.Inputs;

namespace Wolfheat.StartMenu
{
    public struct AnimationRequest
    {
        public Animator animator;
        public string animationName;
        public bool disable;
    }
    public enum MenuOption {MainMenu, Settings, Credits, StartGame, Leaderboards ,Exit}

    public class StartMenuController : MonoBehaviour
    {
        public static StartMenuController Instance { get; private set; }
        public static bool PlayerUsingMouse { get; internal set; }

        public MenuState menuState = MenuState.Idle;
        [SerializeField] WinScreenScroll creditsScroll;
        [SerializeField] StartMenuPanel credits;
        [SerializeField] StartMenuPanel settings;
        [SerializeField] StartMenuPanel leaderboards;
        [SerializeField] StartMenuPanel startMenu;
        [SerializeField] private MenuOption nextMenu;
        [SerializeField] GameObject[] menuDefaultSelect;

        public static MenuButton lastButton;
        private Controls actions;
        private StartMenuPanel currentOption;

        public void SetNextMenu(int nextMenuindex)
        {
            //Debug.Log("Set Next: " + Time.realtimeSinceStartup);
            if (menuState == MenuState.Transitioning) return;
            nextMenu = (MenuOption)nextMenuindex;
            SoundMaster.Instance.PlaySound(SoundName.MenuClick);
            CloseCurrent();
        }

        private void CloseCurrent()
        {
            currentOption.animator.CrossFade("Close", 0.1f);
            menuState = MenuState.Transitioning;
        }

        private void Start()
        {
            Debug.Log("Start Menu Controller, set Current to StartMenu as initiation");
            currentOption = startMenu;
            InitiateStartMenu();
            SoundMaster.Instance.PlayMusic(MusicName.MenuMusic);
        
            actions = new Controls();
            actions.Enable();
            actions.Player.M.performed += SoundMaster.Instance.ToggleMusic;
        }
        private void OnEnable()
        {
            // Leave this
            Time.timeScale = 1f;

            Debug.Log("StartMenu On Enable");
            if (Instance != null) Destroy(gameObject);
            Instance = this;

            settings.gameObject.SetActive(false);
            credits.gameObject.SetActive(false);

            Debug.Log("Soundmaster "+SoundMaster.Instance);

            WinScreenScroll.Completed += CreditsShownComplete;
            StartMenuInputs.Instance.Controls.UI.UpArrow.performed += PlayerUsedKeyboard;
        }

        private void OnDisable()
        {
            actions.Player.M.performed -= SoundMaster.Instance.ToggleMusic;
            WinScreenScroll.Completed -= CreditsShownComplete;
            StartMenuInputs.Instance.Controls.UI.UpArrow.performed -= PlayerUsedKeyboard;
        }

        public void PlayerUsedMouse()
        {
            Debug.Log("Player are using mouse, de-activate current.");
            PlayerUsingMouse = true;
        }
        
        private void PlayerUsedKeyboard(InputAction.CallbackContext context)
        {
            Debug.Log("Player are using keyboard, activate default.");
            PlayerUsingMouse = false;
            ActivateDefaultSelectedForCurrentMenu();
        }


        public void ShowMenu(MenuOption menu)
        {
                Debug.Log("Showing Menu "+menu);
            switch (menu)
            {
                case MenuOption.MainMenu:
                    InitiateStartMenu();
                    break;
                case MenuOption.Settings:
                    ShowSettings();
                    break;
                case MenuOption.Credits:
                    ShowCredits();
                    break;
                case MenuOption.StartGame:
                    StartGame();
                    break;
                case MenuOption.Leaderboards:
                    ShowLeaderboards();
                    break;
                case MenuOption.Exit:
                    ExitGame();
                    break;
            }
        }


        public void AnimationComplete()
        {
            currentOption.gameObject.SetActive(false);        
            ShowMenu(nextMenu);

            // Maybe to early to enable this
            menuState = MenuState.Idle;

        }

        private void ActivateDefaultSelectedForCurrentMenu()
        {
            if (!PlayerUsingMouse) {
                menuDefaultSelect[(int)nextMenu]?.GetComponent<Selectable>().Select();
                Debug.Log("Selected: "+ EventSystem.current.currentSelectedGameObject);
            }
        }

        private void InitiateStartMenu()
        {        
            startMenu.gameObject.SetActive(true);
            startMenu.animator.CrossFade("Initiate",0.1f);
            //startMenu.animator.Play("Initiate");
            currentOption = startMenu;
            ActivateDefaultSelectedForCurrentMenu();
        }

        private void StartGame()
        {
            Debug.Log("Start Game Pressed");
            //SceneManager.UnloadSceneAsync("StartMenu");
            SceneChanger.Instance.ChangeScene("DreamsDungeon2");
        }

        private void ShowLeaderboards()
        {
            Debug.Log("Leaderboards Pressed");
            menuState = MenuState.Transitioning;
            leaderboards.gameObject.SetActive(true);
            currentOption = leaderboards;
            ActivateDefaultSelectedForCurrentMenu();
        }
        private void ShowSettings()
        {
            Debug.Log("Settings Pressed");
            menuState = MenuState.Transitioning;
            settings.gameObject.SetActive(true);
            currentOption = settings;
            ActivateDefaultSelectedForCurrentMenu();
        }

        private void CreditsShownComplete()
        {
            Debug.Log("Shown Credits Complete");
            ShowMenu(MenuOption.MainMenu);
        }

        private void ShowCredits()
        {
            Debug.Log("Credits Pressed");
            menuState = MenuState.Transitioning;
            creditsScroll.gameObject.SetActive(true);
            creditsScroll.ShowFromStartMenu();

            currentOption = credits;
        }
        
        public void ClearSave()
        {
            Debug.Log("Clear Save file requested");
            SavingUtility.Instance.ClearGameData();
        }

        private void ExitGame()
        {
            SavingUtility.Instance.SavePlayerDataToFile();
            Debug.Log("Exit Pressed");
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
            Application.Quit();
        }
    }
    public enum MenuNames {StartGame,Settings,Credits,Exit,
        CloseMenuOption
    }
    public enum MenuState {Idle,Transitioning}
}   
