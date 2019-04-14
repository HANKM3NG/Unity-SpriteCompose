using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

public class EditorResourceSetting : AssetPostprocessor
{
    private string KeyWords = @"[Sprites][Sprite]";
    public void OnPreprocessTexture()
    {
        string dirName = System.IO.Path.GetDirectoryName(assetPath);
        string folderStr = System.IO.Path.GetFileName(dirName);
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        string FileName = System.IO.Path.GetFileName(assetPath);

        if (Regex.IsMatch(folderStr, KeyWords))
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            // textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = true;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}