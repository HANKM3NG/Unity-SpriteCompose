using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteCompose : MonoBehaviour {
    public Texture2D baseTexture;
    // private SpriteData[] baseSpriteData;
    private List<SpriteData> upperSpriteData;

    public void Compose () {
        // TODO: 计算出需要多少格，算出将要生成贴图的大小，生成一张空的新图，然后往里塞

        // Texture2D[] textures = new Texture2D[4];
        // for (int i = 0; i < textures.Length; i++) {
        //     textures[i] = (Texture2D) Resources.Load ("Resources/SpriteSortPoint/sp" + i + 1);
        // }

        // Texture2D cTexture = CreateNewTexture (64, 16, 16, 16, textures);
        // SaveTextureToFile(cTexture,"Resources/GenSprites/createIMG");

        // Rect texRect = new Rect (0, 0, tempTexture2D.width, tempTexture2D.height);
        // Result.sprite = Sprite.Create (tempTexture2D, texRect, new Vector2 (0.5f, 0.5f));

        //  1、读现有贴图
        //  2、把现有贴图分类，中间、四外角、四内角
        //  3、合并
        Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/ground_multiple");
        SpriteData[] baseSpriteData = new SpriteData[sprites.Length];

        string[][] sArray = new string[sprites.Length][];
        int[][] sInt = new int[sprites.Length][];

        for (int i = 0; i < sprites.Length; i++) {
            var data = baseSpriteData[i] = new SpriteData();
            
            data.sprite = sprites[i];
            sArray[i] = sprites[i].name.Split (',');
            sInt[i] = new int[2] { int.Parse (sArray[i][0]), int.Parse (sArray[i][1]) };
            data.pos = new Vector2Int (sInt[i][0], sInt[i][1]);
            // if (sInt[i][0] == 1 && sInt[i][1] % 3 == 1) { //M
            //     data.layer = 0;
            //     data.ornt = ORNT.M;
            // } else {
            //     data.layer = 1;
            //     if (sInt[i][0] == 0 && sInt[i][1] % 3 == 1) { //N
            //         data.ornt = ORNT.N;
            //     } else if (sInt[i][0] == 2 && sInt[i][1] % 3 == 1) { //S
            //         data.ornt = ORNT.S;
            //     } else if (sInt[i][0] == 1 && sInt[i][1] % 3 == 0) { //W
            //         data.ornt = ORNT.W;
            //     } else if (sInt[i][0] == 1 && sInt[i][1] % 3 == 2) { //E
            //         data.ornt = ORNT.E;
            //     }
            // }
        }
        // Texture2D composedGridTexture = ComposeSpritesGrid (upperSpriteData);
        // Texture2D composedTexture = ComposeTextures (baseTexture, composedGridTexture, new Vector2Int (16, 16));
        // SaveTextureToFile (composedTexture, "Resources/GenSprites/resultIMG");
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

// [Serializable]
public class SpriteData {
    public Sprite sprite = null;
    public Vector2Int pos = new Vector2Int (-1, -1);
    public int layer = -1;
    public ORNT ornt = ORNT.M;
}