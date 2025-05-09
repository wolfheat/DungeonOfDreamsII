using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class SpriteMapCreator : MonoBehaviour
{

    [SerializeField] private FogOfWar FOV;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Image image;

    [SerializeField] private GameObject wallHolder;
    [SerializeField] private GameObject portalsHolder;
    [SerializeField] private GameObject healingHolder;

    [SerializeField] private GameObject playerArrow;

    [SerializeField] private RectTransform rectTransform;

    int mapWidth = 128;
    int mapHeight = 128;
    int tileSize = 32;

    public Vector2 offset = Vector2.zero;
    public Vector2Int origoOffset = Vector2Int.zero;

    private Texture2D fullMapTexture;
    private Color[] crackedWallColors;

    public static SpriteMapCreator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Update()
    {
        CenterOnPlayer();
    }

    public void CenterOnPlayer()
    {
        //Vector2 playerPos = PlayerController.Instance.transform.position;
        Vector2 playerPos = new Vector2(PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.z);

        // Set new position of map
        //transform.localPosition = offset;
        transform.localPosition = -(playerPos * tileSize)+offset;
        playerArrow.transform.rotation = Quaternion.Euler(0,0,-PlayerController.Instance.transform.rotation.eulerAngles.y);  
    }

    private void Start()
    {
        CreateMapFromTiles();
    }

    [ContextMenu("Create Tiles")]
    public void CreateMapFromTiles()
    {
        Debug.Log("Creating Map");

        tileSize = 32;

        Resources.UnloadUnusedAssets();
        fullMapTexture = FillTexture();

        Rect rect = new Rect(0,0,mapWidth,mapHeight);

        Sprite newMapSprite = Sprite.Create(fullMapTexture, rect, new Vector2(0.5f, 0.5f), 100.0f);
        newMapSprite.name = "Generated Map Sprite";
        image.sprite = newMapSprite;


        rectTransform.sizeDelta = new Vector2(mapWidth,mapHeight);

        FOV.CreateImage(mapWidth,mapHeight);
    }

    internal Transform[] GetWallsSpots() => wallHolder.GetComponentsInChildren<Wall>().Select(w => w.transform).ToArray();
    internal Transform[] GetDoorsSpots() => wallHolder.GetComponentsInChildren<Door>().Select(w => w.transform).ToArray();
    internal Transform[] GetPortalSpots() => portalsHolder.GetComponentsInChildren<ExitPoint>().Select(w => w.transform).ToArray();
    internal Transform[] GetHealingSpots() => healingHolder.GetComponentsInChildren<HealingArea>().Select(w => w.transform).ToArray();

    private Texture2D FillTexture()
    {
        // Get list of all walls
        Transform[] wallTransforms = GetWallsSpots();

        // Determine bounds of map
        Vector2Int minCorner = new Vector2Int(Mathf.RoundToInt(wallTransforms[0].transform.position.x), Mathf.RoundToInt(wallTransforms[0].transform.position.z));
        Vector2Int maxCorner = new Vector2Int(Mathf.RoundToInt(wallTransforms[0].transform.position.x), Mathf.RoundToInt(wallTransforms[0].transform.position.z));

        // Find bottom corner and top Corner        
        foreach (Transform walltransform in wallTransforms) {
            int xPos = Mathf.RoundToInt(walltransform.transform.position.x);  
            int yPos = Mathf.RoundToInt(walltransform.transform.position.z);  
            if(xPos < minCorner.x) minCorner.x = xPos;
            if(xPos > maxCorner.x) maxCorner.x = xPos;
            if(yPos < minCorner.y) minCorner.y = yPos;
            if(yPos > maxCorner.y) maxCorner.y = yPos;
        }        

        int width  = maxCorner.x - minCorner.x + 1;
        int height = maxCorner.y - minCorner.y + 1;

        int Xdisplace = minCorner.x;
        int Ydisplace = minCorner.y;

        origoOffset = new Vector2Int(Xdisplace, Ydisplace);

        // Determine the total width and height of the final map
        mapWidth = width * tileSize;
        mapHeight = height * tileSize;

        offset = new Vector2((float)mapWidth/2+minCorner.x*tileSize-tileSize/2, (float)mapHeight/2 + minCorner.y * tileSize - tileSize / 2);

        // We now know the full size of the map
        Texture2D fullMapTexture = new Texture2D(mapWidth, mapHeight);

        fullMapTexture.filterMode = FilterMode.Point;
        
        // Fill each tile in the texture
        Color[] wallColors = sprites[0].texture.GetPixels(0);
        Color[] doorColors = sprites[1].texture.GetPixels(0);
        Color[] healingColors = sprites[2].texture.GetPixels(0);
        Color[] portalColors = sprites[3].texture.GetPixels(0);        
        crackedWallColors = sprites[4].texture.GetPixels(0);
        
        // Set every tile of that type
        SetTextureFromSprite(wallTransforms, wallColors);
        SetTextureFromSprite(GetDoorsSpots(), doorColors);
        SetTextureFromSprite(GetPortalSpots(), portalColors, true);
        SetTextureFromSprite(GetHealingSpots(), healingColors, true);

        // Finalize the texture
        fullMapTexture.Apply();

        return fullMapTexture;

        // Local Method to set pixels
        void SetTextureFromSprite(Transform[] positions , Color[] spriteColors, bool blend = false)
        {
            //Debug.Log("Setting colors for "+positions.Length+" items blend ="+blend);
            foreach (Transform healingSpot in positions) {
                int xPos = Mathf.RoundToInt(healingSpot.position.x) - Xdisplace;
                int yPos = Mathf.RoundToInt(healingSpot.position.z) - Ydisplace;

                if (!blend) {
                    fullMapTexture.SetPixels(xPos * tileSize, yPos * tileSize, tileSize, tileSize, spriteColors);
                    continue;
                }

                // Get the existing pixels from the target area
                Color[] existingColors = fullMapTexture.GetPixels(xPos * tileSize, yPos * tileSize, tileSize, tileSize);

                // Blend healingColors over existingColors
                for (int i = 0; i < spriteColors.Length; i++) {
                    Color src = spriteColors[i];
                    Color dst = existingColors[i];

                    float alpha = src.a;

                    // Blend only if alpha > 0
                    if (alpha > 0f) {
                        existingColors[i] = Color.Lerp(dst, src, alpha);
                    }
                    // else leave dst unchanged
                }
                
                fullMapTexture.SetPixels(xPos * tileSize, yPos * tileSize, tileSize, tileSize, existingColors);

            }
        }

    }
    public void RevealCrackedWall(Vector2Int pos)
    {
        pos *= tileSize;

        pos -= Instance.origoOffset * tileSize;

        int tileAdjustAmount = 0;
        pos -= new Vector2Int(tileAdjustAmount, tileAdjustAmount);
        
        // Clamp to avoid out-of-bounds        
        fullMapTexture.SetPixels(pos.x, pos.y, tileSize, tileSize, crackedWallColors);
        fullMapTexture.Apply();
    }



}
