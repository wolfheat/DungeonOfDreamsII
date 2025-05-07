using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DisplayOptionsController : MonoBehaviour
{

    [SerializeField] private DisplayOption displayOptionPrefab; 
    [SerializeField] private TextMeshProUGUI disabledText;

    [SerializeField] private Selectable overrideNavUP;
    [SerializeField] private Selectable overrideNavDOWN;

    public static DisplayOptionsController Instance { get; private set; }

    private List<DisplayOption> displayOptions = new List<DisplayOption>();

    private void OnEnable()
    {
        SpecialInfo.Instance?.ShowInfo("Enable: DisplayOptionsController");
        ResetDisplayButtons();
    }

    public void ResetDisplayButtons()
    {
        Debug.Log("RESET DISPLAY BUTTONS");

        int amt = 0;
        foreach (Transform t in transform.GetComponentInChildren<Transform>()) {
            if (t == this.transform) continue;
            Destroy(t.gameObject);
            amt++;
        }
        SpecialInfo.Instance?.ShowInfo("Destroyed "+amt+" old buttons");
        displayOptions.Clear();

        // Display option disabled for WebGL
        if(Application.platform == RuntimePlatform.WebGLPlayer) {
            disabledText.gameObject.SetActive(true);
            SpecialInfo.Instance?.ShowInfo("WebGL return");
            return;
        }       

        // This needs to happen after the removal, cause webGL cant handle this info
        List<DisplayInfo> displaysInfos = new List<DisplayInfo>();
        Screen.GetDisplayLayout(displaysInfos);

        int activeScreen = displaysInfos.IndexOf(Screen.mainWindowDisplayInfo);
        SpecialInfo.Instance?.ShowInfo("Create new buttons");

        for (int i = 0; i < displaysInfos.Count; i++) {
            DisplayOption option = Instantiate(displayOptionPrefab, this.transform);
            option.SetIndexAndTexts(i, displaysInfos[i].name);
            displayOptions.Add(option);
            option.SetAsSelected(i == activeScreen);

        }
        OverrideNaviagations();

    }

    private void OverrideNaviagations()
    {
        Debug.Log("Overriding navigations");
        // Generate list of Selectables and Navigation
        List<Selectable> selectables = new List<Selectable>();
        List<Navigation> navigations = new List<Navigation>();
        for (int i = 0; i < displayOptions.Count; i++) {
            selectables.Add(displayOptions[i].GetComponent<Selectable>());
        }


        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = overrideNavUP;
        nav.selectOnDown = overrideNavDOWN;

        for (int i = 0; i < displayOptions.Count; i++) {            
            nav.selectOnLeft = i > 0 ? selectables[i-1] : null;
            nav.selectOnRight= i < displayOptions.Count - 1 ? selectables[i+1] : null;
            selectables[i].navigation = nav;
        }
    }

    internal void SetDisplayOptionTo(int index)
    {
        Debug.Log("CHANGE MONITOR");
        if (!moveWindowInProgress) {
            StartCoroutine(MoveToDisplay(FullScreenMode.FullScreenWindow, index));
        }

        // Update visuals to only show the new active button as avtive
        OnlyEnableButton(index);

    }

    private void OnlyEnableButton(int index)
    {
        for (int i = 0; i < displayOptions.Count; i++) {
            DisplayOption displayOption = displayOptions[i];
            displayOption.SetAsSelected(i == index);
        }
    }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private int activedisplay = 0;

    private bool moveWindowInProgress;

    private IEnumerator MoveToDisplay(FullScreenMode mode,int index = 1)
    {
        SpecialInfo.Instance?.ShowInfo("Run Move Display, in progress: " + moveWindowInProgress);



        moveWindowInProgress = true;
        try {

            List<DisplayInfo> displaysInfos = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displaysInfos);

            activedisplay = index == -1 ? ((activedisplay + 1) % displaysInfos.Count) : index;
            SpecialInfo.Instance?.ShowInfo("New Display Index: " + activedisplay);

            if (activedisplay < displaysInfos.Count) {

                SpecialInfo.Instance?.ShowInfo("Displays: " + displaysInfos.Count);


                SpecialInfo.Instance?.ShowInfo("Active Display: " + activedisplay);


                var display = displaysInfos[activedisplay];

                Vector2Int targetCoordinates = new Vector2Int(0, 0);

                if (Screen.fullScreenMode != FullScreenMode.Windowed) {
                    // Target the center of the display. Doing it this way shows off
                    // that MoveMainWindow snaps the window to the top left corner
                    // of the display when running in fullscreen mode.
                    targetCoordinates.x += Screen.width / 2;
                    targetCoordinates.y += Screen.height / 2;
                }

                Screen.fullScreenMode = FullScreenMode.Windowed;

                yield return new WaitForSeconds(0.2f);

                var moveOperation = Screen.MoveMainWindowTo(display, targetCoordinates);
                yield return new WaitForSeconds(0.2f);
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

                SpecialInfo.Instance?.ShowInfo("Active: " + display.name);

                yield return moveOperation;
            }
        }
        finally {
            moveWindowInProgress = false;
        }
    }



}
