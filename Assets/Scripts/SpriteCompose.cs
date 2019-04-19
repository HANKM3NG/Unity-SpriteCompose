using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteCompose : MonoBehaviour {
    // public SpriteRenderer result;
    public Texture2D baseTexture;
    private string tpath = "MultipleSprites/ground";//文件夹名/文件名（不含后缀名）

    public void Compose () {
        Texture2D newTexture = CreateGenTexture (64, 64, 16, 16, GetBaseSpriteData ());
        SaveTextureToFile (newTexture, "Resources/GenSprites/genTex");
        // Texture2D composedGridTexture = ComposeSpritesGrid (upperSpriteData);
        // Texture2D composedTexture = ComposeTextures (baseTexture, composedGridTexture, new Vector2Int (16, 16));
        // SaveTextureToFile (composedTexture, "Resources/GenSprites/resultIMG");
    }

    /// <summary>
    /// 读取基础贴图的Sprite数组，组织SpriteData数据，区分中间、4外角、4内角
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private SpriteData[] GetBaseSpriteData () {
        Sprite[] sprites = Resources.LoadAll<Sprite> (tpath);
        SpriteData[] baseSpriteData = new SpriteData[sprites.Length];

        string[][] sArray = new string[sprites.Length][];
        int[][] sInt = new int[sprites.Length][];

        for (int i = 0; i < sprites.Length; i++) {
            var data = baseSpriteData[i] = new SpriteData ();
            data.sprite = sprites[i];
            sArray[i] = sprites[i].name.Split (',');
            sInt[i] = new int[2] { int.Parse (sArray[i][0]), int.Parse (sArray[i][1]) };
            data.pos = new Vector2Int (sInt[i][0], sInt[i][1]);
            data.layer = Mathf.RoundToInt (sInt[i][1] / 3);
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 1) { //M
                data.ornt = ORNT.M;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 1) { //N
                data.ornt = ORNT.N;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 1) { //S
                data.ornt = ORNT.S;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 0) { //W
                data.ornt = ORNT.W;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 2) { //E
                data.ornt = ORNT.E;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 0) { //NW
                data.ornt = ORNT.NW;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 2) { //NE
                data.ornt = ORNT.NE;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 2) { //SE
                data.ornt = ORNT.SE;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 0) { //SW
                data.ornt = ORNT.SW;
            }
        }
        return baseSpriteData;
    }

    private Texture2D CreateGenTexture (int width, int height, int gridWidth, int gridHeight, SpriteData[] baseSpriteData) {
        Texture2D texture = new Texture2D (width, height);
        // if (baseSpriteData.Length > 0) {
        //     // SpriteData[] sortSpriteData = baseSpriteData;
        //     // Color color;
        //     // int x, y;
        //     // Array.Sort (sortSpriteData, (a, b) => -a.layer.CompareTo (b.layer));
        //     List<SpriteData> gridM = new List<SpriteData> ();
        //     List<SpriteData> gridN = new List<SpriteData> ();
        //     for (int i = 0; i < baseSpriteData.Length; i++) {
        //         if (baseSpriteData[i].ornt == ORNT.M) {
        //             gridM.Add (baseSpriteData[i]);
        //         } else
        //         if (baseSpriteData[i].ornt == ORNT.N) {
        //             gridN.Add (baseSpriteData[i]);
        //         }
        //     }
        //     List<SpriteData> mixGrid = new List<SpriteData> ();
        //     mixGrid.Add (gridM[0]);
        //     mixGrid.Add (gridN[1]);
        //     mixGrid.Add (gridN[2]);
        //     mixGrid.Add (gridN[3]);

        //     Texture2D newGrid = ComposeSpritesGrid (mixGrid);
        //     Color[] colors = newGrid.GetPixels ();
        //     newGrid.SetPixels (0, 0, newGrid.width, newGrid.height, colors);
        //     newGrid.Apply ();
        //     Debug.Log (" ");
        // }
        Sprite sp = baseSpriteData[19].sprite;

        return GetTexture2DFromSprite (sp);
    }

    private Texture2D GetTexture2DFromSprite (Sprite sp) {
        Color[] colors = sp.texture.GetPixels (
            (int) sp.textureRect.x,
            (int) sp.textureRect.y,
            (int) sp.textureRect.width,
            (int) sp.textureRect.height
        );
        Texture2D texture = new Texture2D ((int) sp.textureRect.width, (int) sp.textureRect.height);
        texture.SetPixels (colors);
        texture.Apply ();
        return texture;
    }

    /// <summary>
    /// 根据多张Sprite和其层级关系，合并出一张混合图
    /// </summary>
    /// <param name="spriteData"></param>
    /// <returns></returns>
    private Texture2D ComposeSpritesGrid (List<SpriteData> spriteData) {
        Texture2D texture = new Texture2D ((int) spriteData[0].sprite.textureRect.width, (int) spriteData[0].sprite.textureRect.height);
        Color color;
        spriteData.Sort ((a, b) => -a.layer.CompareTo (b.layer));
        Vector2Int usedPos = new Vector2Int (-1, -1);
        int x, y;
        if (spriteData.Count > 0) {
            for (int i = 0; i < 16; i++) {
                for (int j = 0; j < 16; j++) {
                    for (int k = 0; k < spriteData.Count; k++) {
                        x = (int) spriteData[k].sprite.textureRect.x;
                        y = (int) spriteData[k].sprite.textureRect.y;
                        color = spriteData[k].sprite.texture.GetPixel (x + i, y + j);
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
    /// <param name="path">Resources/ + 文件夹名 + 文件名 (不含后缀名)</param>
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
    public int layer = -1; //混合图片时，layer值大的覆盖小的
    public ORNT ornt = ORNT.M;
}