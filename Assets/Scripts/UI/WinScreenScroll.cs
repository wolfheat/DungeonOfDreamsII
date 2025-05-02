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
    [SerializeField] GameObject completionist;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject scroll;
    

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
        StartMenuInputs.Instance.Controls.Player.Esc.performed += ESCAPE;
    }
    
    private void OnDisable()
    {
        StartMenuInputs.Instance.Controls.Player.Esc.performed -= ESCAPE;
    }

    public void ShowFromStartMenu()
    {
        startMenuView = true;
        panel.SetActive(true);

        StartCoroutine(Animate());

        //SoundMaster.Instance.PlayMusic(MusicName.CreditsMusic);
    }

    private void ESCAPE(InputAction.CallbackContext context)
    {
        // Player tries to escape the credits screen
        Debug.Log("SKIP ESC");
        Hide();
    }
    public void Show()
    {
        startMenuView = false;
        panel.SetActive(true);

        Debug.Log("Win screen Active Pause game");
        StartCoroutine(Animate());
        SoundMaster.Instance.PlayMusic(MusicName.CreditsMusic);
    }

    private IEnumerator Animate()
    {
        RectTransform rect = scroll.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(0, 0); 
        //scroll.transform.localPosition = new Vector3(0,-1000f,0);
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        rect.anchoredPosition = new Vector3(0, StartPosition);
        float textBoxHeight = rect.rect.height;
        float screenHeight = panel.GetComponent<RectTransform>().rect.height;
        
        StartPosition = - screenHeight - TopScrollPadding;
        //StartPosition = - screenHeight - ScrollPadding;
        EndPosition = textBoxHeight + ScrollPadding;
        Debug.Log("End position should be 6400 ish = "+ EndPosition+" = ["+textBoxHeight+"]+["+ScrollPadding+"]");

        yield return null;

        Debug.Log("Text box height = "+textBoxHeight);
        Debug.Log("Text box anchored pos = "+ rect.anchoredPosition);
        Debug.Log("End position should be 6400 ish = "+ EndPosition);

        Vector2 pos = new Vector3(0, StartPosition);
        rect.anchoredPosition = pos;
        while (pos.y<EndPosition) {         
            yield return null;
            float animationSpeed = Mouse.current.leftButton.isPressed  ? speed * Speedup : speed;
            pos += Vector2.up * animationSpeed * Time.unscaledDeltaTime;
            rect.anchoredPosition = pos;
        }
        Debug.Log("Animation of End Credits complete");
        //Hide();

        rect.anchoredPosition = new Vector3(0, StartPosition);
        if (startMenuView) 
            Hide();
        else
            UIController.Instance.ToMainMenu();
        StopAllCoroutines();
    }

    public void Hide()
    {
        Completed?.Invoke();
        panel.SetActive(false);
    }

    internal void SetCompleteTimeText(string winTime) => winTimeText.text = winTime;

    internal void SetCompletePercentText(string winPercent) => winPercentText.text = winPercent;

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
