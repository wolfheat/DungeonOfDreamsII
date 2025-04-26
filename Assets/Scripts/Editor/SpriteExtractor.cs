using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExtractor
{
    [MenuItem("Tools/Extract Sprite as PNG")]
    public static void ExtractSprite()
    {
        var sprite = Selection.activeObject as Sprite;
        if (sprite == null) {
            Debug.LogError("Select a sprite to extract.");
            return;
        }

        Texture2D sourceTex = sprite.texture;
        Rect rect = sprite.textureRect;
        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
        Color[] pixels = sourceTex.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height);
        newTex.SetPixels(pixels);
        newTex.Apply();

        byte[] pngData = newTex.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("Save Sprite", "Assets", sprite.name + ".png", "png");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path, pngData);
            AssetDatabase.Refresh();
            Debug.Log("Sprite saved to " + path);
        }
    }
}