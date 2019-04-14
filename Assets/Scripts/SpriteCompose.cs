using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteCompose : MonoBehaviour {
    public SpriteRenderer Result;
    public List<SpriteData> spriteData;

    private Texture2D tempTexture2D;
    private Color tempColor;
    private Vector2Int usedPos;

    public void Compose () {
        tempTexture2D = new Texture2D (16, 16);
        spriteData.Sort ((a, b) => -a.level.CompareTo (b.level));
        usedPos = new Vector2Int (-1, -1);
        int x, y;
        if (spriteData.Count > 0) {
            for (int i = 0; i < 16; i++) {
                for (int j = 0; j < 16; j++) {
                    for (int k = 0; k < spriteData.Count; k++) {
                        x = (int) spriteData[k].sprite.textureRect.x;
                        y = (int) spriteData[k].sprite.textureRect.y;
                        tempColor = spriteData[k].sprite.texture.GetPixel (x + i, y + j);

                        if (tempColor.a != 0 && usedPos != new Vector2Int (i, j)) {
                            tempTexture2D.SetPixel (i, j, tempColor);
                            usedPos = new Vector2Int (i, j);
                        } else {
                            continue;
                        }
                    }
                }
            }
        }

        tempTexture2D.filterMode = FilterMode.Point;
        tempTexture2D.Apply ();


        SaveTextureToFile (tempTexture2D, "Resources/GenSprites/tempTexture2D");
    }

    /// <summary>
    /// 把生成的贴图保存为文件
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="path"></param>
    private void SaveTextureToFile (Texture2D texture, string path) {
        //  把贴图贴到目标物体的spriteRender上
        Rect texRect = new Rect (0, 0, tempTexture2D.width, tempTexture2D.height);
        Result.sprite = Sprite.Create (tempTexture2D, texRect, new Vector2 (0.5f, 0.5f));;

        byte[] bytes = texture.EncodeToPNG ();
        FileStream file = File.Open (Application.dataPath + "/" + path + ".png", FileMode.Create);
        BinaryWriter binary = new BinaryWriter (file);
        binary.Write (bytes);
        file.Close ();

        //  删除内存里的Texture2D
        // Texture2D.DestroyImmediate(texture);
        // texture = null;

        Debug.LogFormat ("Save texture file to path: {0}.png", path);
    }
}

[Serializable]
public class SpriteData {
    public Sprite sprite;
    public int level;
}