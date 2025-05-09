using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private Image FOV_Image;

    [SerializeField] private RectTransform rectTransform;

    private Color[] colors;
    private Color32[] clearColors;
    private Color transparent = new Color(0,0,0,0);

    private int revealSize = 96;


    int mapWidth = 128;
    int mapHeight = 128;
    int tileSize = 32;

    private void Start()
    {
        colors = new Color[revealSize*revealSize];
        for (int i = 0; i < revealSize*revealSize; i++) {
            colors[i] = transparent;
        }
        // Create a square of fully transparent pixels
        clearColors = new Color32[revealSize * revealSize];
        for (int i = 0; i < clearColors.Length; i++) {
            clearColors[i] = new Color32(0, 0, 0, 0); // Transparent
        }

        PlayerController.Instance.MovedToNewSquare += Reveal;
    }

    private void Update()
    {
        //StartCoroutine(RevealCO());
    }

    Vector2 offset = Vector2.zero;
    private IEnumerator RevealCO()
    {
        while (true) {
            yield return new WaitForSeconds(0.25f);
            Resources.UnloadUnusedAssets();
            //Reveal(new Vector2Int((int)transform.parent.localPosition.x, (int)transform.parent.localPosition.y));
            Reveal(new Vector2Int((int)PlayerController.Instance.transform.position.x*tileSize, (int)PlayerController.Instance.transform.position.z*tileSize));
        }
        
    }
    HashSet<string> usedReveals = new HashSet<string>();

    public void Reveal(Vector2Int pos)
    {
        if (!UIController.Instance.MapIsActive())
            return;
        pos *= tileSize;

        //Debug.Log("Checking to Reveal around position " + pos);

        string posAsString = pos.x + "_"+pos.y;

        if (usedReveals.Contains(posAsString))
            return;

        // Make it local pos in the sprite
        
        //Debug.Log("Revealing map position " + pos);

        usedReveals.Add(posAsString);
        
        Texture2D tex = FOV_Image.sprite.texture;

        pos -= SpriteMapCreator.Instance.origoOffset*tileSize;
        int tileAdjustAmount = (revealSize-tileSize)/2;
        pos -= new Vector2Int(tileAdjustAmount, tileAdjustAmount);
            
        // Clamp to avoid out-of-bounds
        int clampedX = Mathf.Clamp(pos.x, 0, tex.width - revealSize);
        int clampedY = Mathf.Clamp(pos.y, 0, tex.height - revealSize);

        tex.SetPixels32(clampedX, clampedY, revealSize, revealSize, clearColors);
        tex.Apply();
    }


    public void TestMethod()
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); 

        stopwatch.Stop();
        UnityEngine.Debug.Log("Method took " + stopwatch.ElapsedMilliseconds + " ms.");
    }


    public void CreateImage(int mapWidth, int mapHeight)
    {
        // We now know the full size of the map
        Texture2D fullMapTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false);

        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;

        fullMapTexture.filterMode = FilterMode.Point;

        // Awaited Pixels set
        Color32[] pixels = new Color32[mapWidth * mapHeight];
        System.Array.Fill(pixels, new Color32(0, 0, 0, 255));

        fullMapTexture.SetPixelData(pixels, 0);

        Rect rect = new Rect(0, 0, mapWidth, mapHeight);

        fullMapTexture.Apply();

        Sprite newMapSprite = Sprite.Create(fullMapTexture, rect, new Vector2(0.5f, 0.5f), 100.0f);
        newMapSprite.name = "FOV_Sprite";

        SetSprite(newMapSprite);

        rectTransform.sizeDelta = new Vector2(mapWidth, mapHeight);

        StartCoroutine(RevealPlayerPosition());
    }


    private IEnumerator RevealPlayerPosition()
    {
        // reveal players position
        yield return null;
        //UnityEngine.Debug.Log("Reveal from player position "+ PlayerController.Instance.transform.position+" = "+ Convert.V3ToV2Int(PlayerController.Instance.transform.position));
        Reveal(Convert.V3ToV2Int(PlayerController.Instance.transform.position));
    }

    private void SetSprite(Sprite newMapSprite) => FOV_Image.sprite = newMapSprite;
}
