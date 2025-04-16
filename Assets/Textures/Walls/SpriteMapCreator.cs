using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteMapCreator : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Image image;

    [SerializeField] private GameObject wallHolder;

    [SerializeField] private GameObject playerArrow;

    [SerializeField] private RectTransform rectTransform;

    int mapWidth = 128;
    int mapHeight = 128;
    int tileSize = 128;

    Vector2 offset = Vector2.zero;

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
        Debug.Log("Creating Sprite");

        tileSize = 32;

        Resources.UnloadUnusedAssets();
        Texture2D texture2D = FillTexture();

        Rect rect = new Rect(0,0,mapWidth,mapHeight);

        Sprite newMapSprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f), 100.0f);
        newMapSprite.name = "Generated Map Sprite";
        Debug.Log("Craeting a new sprite with size "+texture2D.width+" "+texture2D.height);
        image.sprite = newMapSprite;

        rectTransform.sizeDelta = new Vector2(mapWidth,mapHeight);
    }

    internal Wall[] GetWalls() => wallHolder.GetComponentsInChildren<Wall>();

    private Texture2D FillTexture()
    {
        // Get list of all walls
        Wall[] walls = GetWalls();

        Vector2Int minCorner = new Vector2Int(Mathf.RoundToInt(walls[0].transform.position.x), Mathf.RoundToInt(walls[0].transform.position.z));
        Vector2Int maxCorner = new Vector2Int(Mathf.RoundToInt(walls[0].transform.position.x), Mathf.RoundToInt(walls[0].transform.position.z));

        // Find bottom corner and top Corner
        
        foreach (Wall wall in walls) {
            int xPos = Mathf.RoundToInt(wall.transform.position.x);  
            int yPos = Mathf.RoundToInt(wall.transform.position.z);  
            if(xPos < minCorner.x) minCorner.x = xPos;
            if(xPos > maxCorner.x) maxCorner.x = xPos;
            if(yPos < minCorner.y) minCorner.y = yPos;
            if(yPos > maxCorner.y) maxCorner.y = yPos;
        }
        

        int width  = maxCorner.x - minCorner.x + 1;
        int height = maxCorner.y - minCorner.y + 1;

        int Xdisplace = minCorner.x;
        int Ydisplace = minCorner.y;

        Debug.Log("Displacements ["+Xdisplace+","+Ydisplace+"]");

        mapWidth = width * tileSize;
        mapHeight = height * tileSize;

        offset = new Vector2((float)mapWidth/2+minCorner.x*tileSize-tileSize/2, (float)mapHeight/2 + minCorner.y * tileSize - tileSize / 2);

        Debug.Log("GameSize ["+mapWidth+","+mapHeight+"] ["+width+","+height+"] ");

        // We now know the full size of the map
        Texture2D fullMapTexture = new Texture2D(mapWidth, mapHeight);

        // Fill each tile in the texture


        Color[] colors = sprites[0].texture.GetPixels(0);

        foreach (Wall wall in walls) {
            int xPos = Mathf.RoundToInt(wall.transform.position.x)-Xdisplace;
            int yPos = Mathf.RoundToInt(wall.transform.position.z)-Ydisplace;
            Debug.Log("Setting pixel for map at ["+xPos+","+yPos+"]");
            fullMapTexture.SetPixels(xPos*tileSize, yPos*tileSize, tileSize, tileSize, colors);
        }
        fullMapTexture.Apply();
        return fullMapTexture;
    }
}
