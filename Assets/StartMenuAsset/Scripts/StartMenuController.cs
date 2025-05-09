using System;
using System.Collections;
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
    public enum MenuOption {MainMenu, Settings, Credits, Leaderboards , StartGame, Exit }

    public class StartMenuController : MonoBehaviour, IPointerMoveHandler
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
        [SerializeField] private MenuOption currentMenu;
        [SerializeField] private MenuOption menuBeforeMain = MenuOption.MainMenu;
        [SerializeField] GameObject[] menuDefaultSelect;
        [SerializeField] GameObject[] mainMenuButtons;

        public static MenuButton lastButton;
        private Controls actions;
        private StartMenuPanel currentOption;

        public void SetNextMenu(int nextMenuindex) => SetNextMenu((MenuOption)nextMenuindex);
        public void SetNextMenu(MenuOption nextMenuOption)
        {
            //Debug.Log("Set Next: " + Time.realtimeSinceStartup);
            if (menuState == MenuState.Transitioning) return;
            nextMenu = nextMenuOption;
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
            //currentOption = startMenu;
            //currentMenu = MenuOption.MainMenu;
            //ActivateDefaultSelectedForCurrentMenu();

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
            StartMenuInputs.Instance.Controls.UI.ActivateKeyboard.performed += PlayerUsedKeyboard;
            StartMenuInputs.Instance.Controls.UI.ESC.performed += ESC;
            //StartMenuInputs.Instance.Controls.UI.ESC.performed += PlayerUsedKeyboard;
        }

        private void ESC(InputAction.CallbackContext context)
        {
            switch (currentMenu) {
                case MenuOption.MainMenu:
                    Debug.Log("ESC on Main Menu do Nothing, or jump to Quit?");
                    ActivateDefaultSelectedForCurrentMenu();
                    break;
                case MenuOption.Settings:
                    Debug.Log("ESC on Setting Menu, close it if not entering name?");
                    SetNextMenu(MenuOption.MainMenu);
                    break;
                case MenuOption.Credits:
                    Debug.Log("ESC on Credits, already got function for that?");
                    break;
                case MenuOption.Leaderboards:
                    Debug.Log("ESC on Leaderboards, close it?");
                    SetNextMenu(MenuOption.MainMenu);
                    break;
                default:
                    break;
            }
        }

        private void OnDisable()
        {
            actions.Player.M.performed -= SoundMaster.Instance.ToggleMusic;
            WinScreenScroll.Completed -= CreditsShownComplete;
            StartMenuInputs.Instance.Controls.UI.ActivateKeyboard.performed -= PlayerUsedKeyboard;
            StartMenuInputs.Instance.Controls.UI.ESC.performed -= ESC;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (EnteringDataInInputField()) {
                Debug.Log("Moveing pointer but player is editing inputfield");
                return;
            }
            EventSystem.current.SetSelectedGameObject(null);
            PlayerUsingMouse = true;
        }

        private bool EnteringDataInInputField() => NameChanger.Instance != null && NameChanger.Instance.IsEditingName;

        private void PlayerUsedKeyboard(InputAction.CallbackContext context)
        {
            if (NameChanger.Instance != null && NameChanger.Instance.IsEditingName) {
                Debug.Log("Player using keyboard yes, but editing inputfield so dismiss change to keybard controls");
                return;
            }

            if (PlayerUsingMouse) {
                Debug.Log("Player switched to using keyboard, activate default.");
                PlayerUsingMouse = false;
                ActivateDefaultSelectedForCurrentMenu();
            }
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
                case MenuOption.Leaderboards:
                    ShowLeaderboards();
                    break;
                case MenuOption.StartGame:
                    StartGame();
                    break;
                case MenuOption.Exit:
                    ExitGame();
                    break;
            }
            //currentMenu = menu;
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
                Debug.Log("next Menu = "+ currentMenu + " trying to activate default selectable from it, came from "+menuBeforeMain);
                if (menuDefaultSelect[(int)currentMenu] != null) {
                    if(currentMenu == MenuOption.MainMenu && menuBeforeMain != MenuOption.MainMenu) {
                        Debug.Log(" -- - |Selecting menu button "+menuBeforeMain);
                        mainMenuButtons[(int)menuBeforeMain]?.GetComponent<Selectable>().Select();
                    }
                    else {  
                        Debug.Log(" -- - -Selecting menu button "+currentMenu);
                        menuDefaultSelect[(int)currentMenu]?.GetComponent<Selectable>().Select();
                    }
                }
                Debug.Log("Selected: "+ EventSystem.current.currentSelectedGameObject);
            }
        }

        private void InitiateStartMenu()
        {
            Debug.Log("Initiating start Menu");
            Debug.Log("null? = "+startMenu == null);
            startMenu.gameObject.SetActive(true);

            Debug.Log("Initiating start Menu B");
            startMenu.animator.CrossFade("Initiate",0.1f);
            //startMenu.animator.Play("Initiate");
            currentOption = startMenu;
            menuBeforeMain = currentMenu;
            currentMenu = MenuOption.MainMenu;
            Debug.Log("Initiating start Menu C");
            ActivateDefaultSelectedForCurrentMenu();
            Debug.Log("Initiating start Menu D");


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
            currentMenu = MenuOption.Leaderboards;
            ActivateDefaultSelectedForCurrentMenu();
        }
        private void ShowSettings()
        {
            Debug.Log("Settings Pressed");
            menuState = MenuState.Transitioning;
            settings.gameObject.SetActive(true);
            currentOption = settings;
            currentMenu = MenuOption.Settings;
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
            currentMenu = MenuOption.Credits;
            ActivateDefaultSelectedForCurrentMenu();
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
