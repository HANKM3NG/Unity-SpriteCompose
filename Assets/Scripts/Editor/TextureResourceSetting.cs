using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TextureResourceSetting : AssetPostprocessor {
    private string KeyWordsSprite = "Sprites"; //文件夹名字包含Sprite或Sprites就自动改
    private string KeyWordsMultiple = "Multiple"; //文件夹名字包含Multiple就自动改
    private int SliceWidth = 16;
    private int SliceHeight = 16;
    string dirName, folderName, fileNameNoExtension, path;

    public void OnPreprocessTexture () {
        dirName = System.IO.Path.GetDirectoryName (assetPath); //完整路径，不包含文件名
        folderName = System.IO.Path.GetFileName (dirName); //文件所属的文件夹名，不包含路径
        fileNameNoExtension = System.IO.Path.GetFileNameWithoutExtension (assetPath); //文件名，不包含后缀
        path = folderName + "/" + fileNameNoExtension;
        TextureImporter textureImporter = (TextureImporter) assetImporter;
        Texture2D texture = (Texture2D) Resources.Load (path); //切记这里是文件所在的文件夹名 + 文件名不含后缀

        if (Regex.IsMatch (folderName, KeyWordsSprite, RegexOptions.IgnoreCase)) {
            textureImporter.isReadable = true;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            if (Regex.IsMatch (folderName, KeyWordsMultiple, RegexOptions.IgnoreCase)) {
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            } else {
                textureImporter.spriteImportMode = SpriteImportMode.Single;
            }
            AssetDatabase.Refresh ();
        }
    }
    public void OnPostprocessTexture (Texture2D texture) {
        TextureImporter textureImporter = (TextureImporter) assetImporter;
        SplitMultipleSprite (texture, textureImporter);
        AssetDatabase.Refresh ();
    }

    private void SplitMultipleSprite (Texture2D texture, TextureImporter textureImporter) {
        if (texture != null) {
            List<SpriteMetaData> data = new List<SpriteMetaData> ();
            for (int i = 0; i < texture.width; i += SliceWidth) {
                for (int j = texture.height; j > 0; j -= SliceHeight) {
                    SpriteMetaData smd = new SpriteMetaData ();
                    smd.pivot = new Vector2 (0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (texture.height - j) / SliceHeight + "," + i / SliceWidth;
                    smd.rect = new Rect (i, j - SliceHeight, SliceWidth, SliceHeight);
                    data.Add (smd);
                }
            }
            textureImporter.spritesheet = data.ToArray ();
            AssetDatabase.ImportAsset (path, ImportAssetOptions.ForceUpdate);
        }
    }
}