using System;
using System.Collections;

//using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Wolfheat.Inputs;
using Wolfheat.StartMenu;

public class WinScreenScroll : MonoBehaviour
{
    [SerializeField] UIController UIController;
    [SerializeField] TextMeshProUGUI winTimeText;
    [SerializeField] TextMeshProUGUI winPercentText;
    [SerializeField] TextMeshProUGUI deathsText;
    [SerializeField] TextMeshProUGUI successText;

    [SerializeField] GameObject completionist;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject panelHider;
    [SerializeField] GameObject allText;
    [SerializeField] GameObject scroll;

    [SerializeField] private LayerMask UIInvisibleLayer;
    [SerializeField] private LayerMask UILayer;

    private float StartPosition = 0f;
    private float EndPosition = 5600;
    private float TopScrollPadding = 200f;
    private float ScrollPadding = 250f;

    private float speed = 80f;
    private const float Speedup = 6f;
    private bool startMenuView = false;

    public static Action Completed;

    private void Start()
    {
        Hide();
        //StartMenuInputs.Instance.Controls.Player.Esc.performed += ESCAPE;
    }
    private void OnEnable()
    {        
        if(StartMenuInputs.Instance != null)
            StartMenuInputs.Instance.Controls.UI.ESC.performed += ESCAPE;
        if(Inputs.Instance != null)
        Inputs.Instance.Controls.Player.Esc.performed += ESCAPE;
    }

    private void OnDisable()
    {
        if (StartMenuInputs.Instance != null)
            StartMenuInputs.Instance.Controls.UI.ESC.performed -= ESCAPE;
        if (Inputs.Instance != null)
            Inputs.Instance.Controls.Player.Esc.performed -= ESCAPE;
    }

    public void ShowFromStartMenu()
    {
        startMenuView = true;
        //panel.SetActive(true);

        StartCoroutine(Animate());

        //SoundMaster.Instance.PlayMusic(MusicName.CreditsMusic);
    }

    private void ESCAPE(InputAction.CallbackContext context)
    {
        if (!panel.gameObject.activeSelf) return;
        // Player tries to escape the credits screen
        Debug.Log("SKIP ESC");
        ExitWinScrollMenu();
    }
    public void Show()
    {
        startMenuView = false;
        
        Debug.Log("Win screen Active Pause game");
        StartCoroutine(Animate());
        SoundMaster.Instance.PlayMusic(MusicName.CreditsMusic);
    }

    private IEnumerator Animate()
    {
        panelHider.SetActive(true);
        RectTransform rect = scroll.GetComponent<RectTransform>();
        yield return null;
        panel.SetActive(true);
        yield return null;
        rect.anchoredPosition = new Vector3(0, StartPosition);

        yield return null;

        float textBoxHeight = rect.rect.height;
        float screenHeight = panel.GetComponent<RectTransform>().rect.height;

        StartPosition = -screenHeight - TopScrollPadding;
        Debug.Log("Start position = " + StartPosition);

        Vector2 pos = new Vector3(0, StartPosition);
        rect.anchoredPosition = pos;

        //StartPosition = - screenHeight - ScrollPadding;
        EndPosition = textBoxHeight + ScrollPadding;
        Debug.Log("End position should be 6400 ish = " + EndPosition + " = [" + textBoxHeight + "]+[" + ScrollPadding + "]");

        yield return null;

        panelHider.SetActive(false);

        Debug.Log("Text box height = " + textBoxHeight);
        Debug.Log("Text box anchored pos = " + rect.anchoredPosition);
        Debug.Log("End position should be 6400 ish = " + EndPosition);
        while (pos.y < EndPosition) {
            yield return null;
            // Include keyboars speed up
            float animationSpeed = (Mouse.current.leftButton.isPressed
                || SpeedUpWinScreenButtonsPressed()
                ) ? speed * Speedup : speed;
            //float animationSpeed = Mouse.current.leftButton.isPressed  ? speed * Speedup : speed;
            pos += Vector2.up * animationSpeed * Time.unscaledDeltaTime;
            rect.anchoredPosition = pos;
        }
        Debug.Log("Animation of End Credits complete");
        //Hide();


        rect.anchoredPosition = new Vector3(0, StartPosition);
        ExitWinScrollMenu();
    }

    private static bool SpeedUpWinScreenButtonsPressed()
    {
        return   (Inputs.Instance != null && (Inputs.Instance.Controls.UI.Enter.IsPressed() || Inputs.Instance.Controls.UI.Space.IsPressed() || Inputs.Instance.Controls.UI.UpArrow.IsPressed())) 
              || (StartMenuInputs.Instance != null && (StartMenuInputs.Instance.Controls.UI.Enter.IsPressed() || StartMenuInputs.Instance.Controls.UI.Space.IsPressed() || StartMenuInputs.Instance.Controls.UI.UpArrow.IsPressed()));
    }

    private void ExitWinScrollMenu()
    {
        if (startMenuView)
            Hide();
        else {
            Debug.Log("TO MAIN MENU - ONLY FROM GAME NOT CREDITS VIEW");
            UIController.Instance.ToMainMenu();
        }
        StopAllCoroutines();

        panelHider.SetActive(false);
    }

    public void Hide()
    {
        if(startMenuView)
            Completed?.Invoke();
        panel.SetActive(false);
    }

    internal void SetCompleteTimeText(string winTime) => winTimeText.text = winTime;

    internal void SetCompletePercentText(string winPercent) => winPercentText.text = winPercent;

    internal void SetSuccessText(string success) => successText.text = success;
    internal void SetDeathsText(int deaths)
    {
        deathsText.transform.parent.gameObject.SetActive(deaths > 0);
            deathsText.text = deaths.ToString();
    }

    internal void ShowCompletionist(bool isComplete) => completionist.SetActive(isComplete);

    /*

<b>Credits
</b>

A game by wolfheat


A variety of assets were used to create this game.


<b>Graphics
</b>

3D
Leartes Studios - Will's Room Environment
RRFreelance - Tools Pack 1
Polygon  - Farm Pack
Pure Poly - Ultimate Low Poly Mining

2D
Vector Dividers Collection
upklyak (freepik) - Ground cracks on land isolated
GarryKillian(freepik) - Wallpaper
masadepan(freepik) - Vector Compass
Zooperdan - Some logos
João Baltieri - Mini Simple Characters | Skeleton

VFX
Unity Technologies - Particle Pack
Vefects - Item Pickup VFX


<b>Audio
</b>

Freesound - Various Sounds
Uppbeat - Music

Vasyl Sakal - Celestial Lullaby 
Brock Hewitt - In The Silence


<b>Fonts
</b>

Prida01 - gluk


<b>AI Tools
</b>

Genny - Voices - Cloe, Matthew

Playground AI - 2D art


<b>Other Tools Used
</b>

Audacity

Gimp

Blender

Unity


<b>Game Jam
</b>

This game was created for the
Dungeon Crawler Game Jam 2024


Thank you for playing!



*/
}
