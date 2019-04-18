using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteCompose : MonoBehaviour {
    public Texture2D baseTexture;

    // private SpriteData[] baseSpriteData;
    private List<SpriteData> upperSpriteData;
    private string path = "Sprites/ground_multiple";

    public void Compose () {
        SpriteData[] data = GetBaseSpriteData (path);
        // Texture2D composedGridTexture = ComposeSpritesGrid (upperSpriteData);
        // Texture2D composedTexture = ComposeTextures (baseTexture, composedGridTexture, new Vector2Int (16, 16));
        // SaveTextureToFile (composedTexture, "Resources/GenSprites/resultIMG");
    }

    /// <summary>
    /// 读取基础贴图的Sprite数组，组织SpriteData数据，区分中间、4外角、4内角
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private SpriteData[] GetBaseSpriteData (string path) {
        Sprite[] sprites = Resources.LoadAll<Sprite> (path);
        SpriteData[] baseSpriteData = new SpriteData[sprites.Length];

        string[][] sArray = new string[sprites.Length][];
        int[][] sInt = new int[sprites.Length][];

        for (int i = 0; i < sprites.Length; i++) {
            var data = baseSpriteData[i] = new SpriteData ();
            data.sprite = sprites[i];
            sArray[i] = sprites[i].name.Split (',');
            sInt[i] = new int[2] { int.Parse (sArray[i][0]), int.Parse (sArray[i][1]) };
            data.pos = new Vector2Int (sInt[i][0], sInt[i][1]);
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 1) { //M
                data.layer = 0;
                data.ornt = ORNT.M;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 1) { //N
                data.layer = 1;
                data.ornt = ORNT.N;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 1) { //S
                data.layer = 1;
                data.ornt = ORNT.S;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 0) { //W
                data.layer = 1;
                data.ornt = ORNT.W;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 2) { //E
                data.layer = 1;
                data.ornt = ORNT.E;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 0) { //NW
                data.layer = 2;
                data.ornt = ORNT.NW;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 2) { //NE
                data.layer = 2;
                data.ornt = ORNT.NE;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 2) { //SE
                data.layer = 2;
                data.ornt = ORNT.SE;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 0) { //SW
                data.layer = 2;
                data.ornt = ORNT.SW;
            }
        }
        return baseSpriteData;
    }

    /// <summary>
    /// 根据多张Sprite和其层级关系，合并出一张混合图
    /// </summary>
    /// <param name="spData"></param>
    /// <returns></returns>
    private Texture2D ComposeSpritesGrid (List<SpriteData> spData) {
        Texture2D texture = new Texture2D (16, 16);
        Color color;
        spData.Sort ((a, b) => -a.layer.CompareTo (b.layer));
        Vector2Int usedPos = new Vector2Int (-1, -1);
        int x, y;
        if (spData.Count > 0) {
            for (int i = 0; i < 16; i++) {
                for (int j = 0; j < 16; j++) {
                    for (int k = 0; k < spData.Count; k++) {
                        x = (int) spData[k].sprite.textureRect.x;
                        y = (int) spData[k].sprite.textureRect.y;
                        color = spData[k].sprite.texture.GetPixel (x + i, y + j);
                        if (color.a != 0 && usedPos != new Vector2Int (i, j)) {
                            texture.SetPixel (i, j, color);
                            usedPos = new Vector2Int (i, j);
                        } else {
                            continue;
                        }
                    }
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply ();
        return texture;
    }

    /// <summary>
    /// 把合并计算出的图画到现有的贴图文件的指定位置
    /// </summary>
    /// <param name="baseIMG">已有贴图</param>
    /// <param name="addIMG">需要加的贴图</param>
    /// <param name="drawPos">开始画的坐标</param>
    /// <returns></returns>
    private Texture2D ComposeTextures (Texture2D baseIMG, Texture2D addIMG, Vector2Int drawPos) {
        Texture2D texture = new Texture2D (baseIMG.width, baseIMG.height);
        Color[] colors = baseIMG.GetPixels ();
        for (int i = 0; i < addIMG.width; i++) {
            for (int j = 0; j < addIMG.height; j++) {
                Color c = addIMG.GetPixel (i, j);
                if (c.a != 0) {
                    colors[texture.width * (j + drawPos.y) + (i + drawPos.x)] = c;
                }
            }
        }
        texture.SetPixels (0, 0, baseIMG.width, baseIMG.height, colors);
        texture.Apply ();
        return texture;
    }

    private Texture2D CreateNewTexture (int width, int height, int gridWidth, int gridHeight, Texture2D[] textures) {
        Texture2D texture = new Texture2D (width, height);
        Color[] colors = texture.GetPixels ();
        for (int i = 0; i < textures.Length; i++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (x >= gridWidth * i && x < gridWidth * (i + 1) && y >= gridHeight * i && y < gridHeight * (i + 1)) {
                        Color c = textures[i].GetPixel (x, y);
                        colors[texture.width * y + x] = c;
                    }
                }
            }
        }
        texture.SetPixels (0, 0, width, height, colors);
        texture.Apply ();
        return texture;

    }

    /// <summary>
    /// 把Texture2D保存为文件
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="path"></param>
    private void SaveTextureToFile (Texture2D texture, string path) {
        byte[] bytes = texture.EncodeToPNG ();
        FileStream file = File.Open (Application.dataPath + "/" + path + ".png", FileMode.Create);
        BinaryWriter binary = new BinaryWriter (file);
        binary.Write (bytes);
        file.Close ();

        //  删除内存里的Texture2D
        Texture2D.DestroyImmediate (texture);
        texture = null;

        Debug.LogFormat ("Save texture file to path: {0}.png", path);
    }
}

public enum ORNT {
    M,
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}

public class SpriteData {
    public Sprite sprite = null;
    public Vector2Int pos = new Vector2Int (-1, -1);
    public int layer = -1;
    public ORNT ornt = ORNT.M;
}