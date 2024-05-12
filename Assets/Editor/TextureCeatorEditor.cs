using UnityEngine;
using UnityEditor;

public class TextureCreatorEditor
{
    [MenuItem("Assets/Create/Texture2D", false, 100)]
    public static void CreateTexture()
    {
        Color transparentColor = new Color(0, 0, 0, 0);
        Texture2D newTexture = new Texture2D(256, 256);
        for (int y = 0; y < newTexture.height; y++)
        {
            for (int x = 0; x < newTexture.width; x++)
            {
                newTexture.SetPixel(x, y, transparentColor);
            }
        }
        newTexture.Apply();

        string path = EditorUtility.SaveFilePanelInProject("Save New Texture", "NewTexture", "png", "Please enter a file name to save the texture to");
        if (path.Length != 0)
        {
            System.IO.File.WriteAllBytes(path, newTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
    }
}
