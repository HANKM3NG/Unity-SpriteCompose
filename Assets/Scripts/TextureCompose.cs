﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureCompose : MonoBehaviour {
    public int maxDrawTextureWidth = 80;
    public string baseSpritePath = "MultipleSprites/ground"; //文件夹名/文件名（不含后缀名）
    public string saveTexturePath = "Resources/GenSprites/genTex";
    private int gridWidth = 16;
    private int gridHeight = 16;
    private int textureWidth = 0;
    private int textureHeight = 0;

    public void Compose () {
        SaveTextureToFile (CreateGenTexture (), saveTexturePath);
    }

    private Texture2D CreateGenTexture () {
        BaseSpriteData[] baseSpriteData = GetBaseSpriteData ();
        List<Sprite> mixedSprites = new List<Sprite> ();
        List<BaseSpriteData> gridM = new List<BaseSpriteData> ();
        List<BaseSpriteData> gridN = new List<BaseSpriteData> ();
        List<BaseSpriteData> gridE = new List<BaseSpriteData> ();
        List<BaseSpriteData> gridS = new List<BaseSpriteData> ();
        List<BaseSpriteData> gridW = new List<BaseSpriteData> ();
        for (int i = 0; i < baseSpriteData.Length; i++) {
            switch (baseSpriteData[i].ornt) {
                case ORNT.M:
                    gridM.Add (baseSpriteData[i]);
                    break;
                case ORNT.N:
                    gridN.Add (baseSpriteData[i]);
                    break;
                case ORNT.E:
                    gridE.Add (baseSpriteData[i]);
                    break;
                case ORNT.S:
                    gridS.Add (baseSpriteData[i]);
                    break;
                case ORNT.W:
                    gridW.Add (baseSpriteData[i]);
                    break;
                default:
                    break;
            }
        }

        //  画出5种底图
        foreach (BaseSpriteData m in gridM) {
            mixedSprites.Add (m.sprite);
        }
        int gridBase = mixedSprites.Count;

        //  拼合1个边，每种底图对应16种可能，共16 * 5 = 80种可能，每步一个girdM的循环是为了画的工整后面好找
        //  N+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int n = 0; n < gridN.Count; n++) {
                if (i != n) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridN[n] }));
            }
        }
        //  E+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int e = 0; e < gridE.Count; e++) {
                if (i != e) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridE[e] }));
            }
        }
        //  S+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int s = 0; s < gridE.Count; s++) {
                if (i != s) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridS[s] }));
            }
        }
        //  W+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int w = 0; w < gridE.Count; w++) {
                if (i != w) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridW[w] }));
            }
        }
        int grid1edge = mixedSprites.Count - gridBase;

        //  拼合2个边，NS\WE\NE\NW\SE\SW共6种情况，每种情况16种组合，5种底图，共 5 * 16 * 6 = 480种可能，每步一个girdM的循环是为了画的工整后面好找
        //  N&S+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int ns_n = 0; ns_n < gridN.Count; ns_n++) {
                for (int ns_s = 0; ns_s < gridS.Count; ns_s++) {
                    if (i != ns_n && i != ns_s) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridN[ns_n], gridS[ns_s] }));
                }
            }
        }
        //  W&E+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int we_w = 0; we_w < gridN.Count; we_w++) {
                for (int we_e = 0; we_e < gridS.Count; we_e++) {
                    if (i != we_w && i != we_e) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridW[we_w], gridE[we_e] }));
                }
            }
        }
        //  N&E+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int ne_n = 0; ne_n < gridN.Count; ne_n++) {
                for (int ne_e = 0; ne_e < gridS.Count; ne_e++) {
                    if (i != ne_n && i != ne_e) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridN[ne_n], gridE[ne_e] }));
                }
            }
        }
        //  N&W+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int nw_n = 0; nw_n < gridN.Count; nw_n++) {
                for (int nw_w = 0; nw_w < gridS.Count; nw_w++) {
                    if (i != nw_n && i != nw_w) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridN[nw_n], gridW[nw_w] }));
                }
            }
        }
        //  S&E+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int se_s = 0; se_s < gridN.Count; se_s++) {
                for (int se_e = 0; se_e < gridS.Count; se_e++) {
                    if (i != se_s && i != se_e) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridS[se_s], gridE[se_e] }));
                }
            }
        }
        //  S&W+底图 组合
        for (int i = 0; i < gridM.Count; i++) {
            for (int sw_s = 0; sw_s < gridN.Count; sw_s++) {
                for (int sw_w = 0; sw_w < gridS.Count; sw_w++) {
                    if (i != sw_s && i != sw_w) mixedSprites.Add (ComposeSpriteDataListToSprite (new List<BaseSpriteData> { gridM[i], gridS[sw_s], gridW[sw_w] }));
                }
            }
        }

        int grid2edges = mixedSprites.Count - gridBase - grid1edge;

        Debug.LogFormat ("Grid base:{0}, 1edge:{1}, 2edges:{2}", gridBase, grid1edge, grid2edges);

        return DrawTexture (mixedSprites);
    }

    /// <summary>
    /// 读取基础贴图的Sprite数组，组织SpriteData数据，区分中间、4外角、4内角
    /// </summary>
    /// <param name="path"></param>
    private BaseSpriteData[] GetBaseSpriteData () {
        Sprite[] sprites = Resources.LoadAll<Sprite> (baseSpritePath);
        BaseSpriteData[] baseSpriteData = new BaseSpriteData[sprites.Length];

        string[][] sArray = new string[sprites.Length][];
        int[][] sInt = new int[sprites.Length][];

        for (int i = 0; i < sprites.Length; i++) {
            var data = baseSpriteData[i] = new BaseSpriteData ();
            data.sprite = sprites[i];
            sArray[i] = sprites[i].name.Split (',');
            sInt[i] = new int[2] { int.Parse (sArray[i][0]), int.Parse (sArray[i][1]) };
            data.pos = new Vector2Int (sInt[i][0], sInt[i][1]);
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 1) { //M
                data.ornt = ORNT.M;
                data.layer = 0;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 1) { //N
                data.ornt = ORNT.N;
                data.layer = 1;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 1) { //S
                data.ornt = ORNT.S;
                data.layer = 1;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 0) { //W
                data.ornt = ORNT.W;
                data.layer = 1;
            } else
            if (sInt[i][0] == 1 && sInt[i][1] % 3 == 2) { //E
                data.ornt = ORNT.E;
                data.layer = 1;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 0) { //NW
                data.ornt = ORNT.NW;
                data.layer = 2;
            } else
            if (sInt[i][0] == 0 && sInt[i][1] % 3 == 2) { //NE
                data.ornt = ORNT.NE;
                data.layer = 2;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 2) { //SE
                data.ornt = ORNT.SE;
                data.layer = 2;
            } else
            if (sInt[i][0] == 2 && sInt[i][1] % 3 == 0) { //SW
                data.ornt = ORNT.SW;
                data.layer = 2;
            }
        }
        return baseSpriteData;
    }

    /// <summary>
    /// 根据多张Sprite和其层级关系，合并出一张混合图
    /// </summary>
    /// <param name="spriteData"></param>
    /// <returns></returns>
    private Sprite ComposeSpriteDataListToSprite (List<BaseSpriteData> spriteData) {
        Texture2D texture = new Texture2D ((int) spriteData[0].sprite.textureRect.width, (int) spriteData[0].sprite.textureRect.height);
        Color color;
        spriteData.Sort ((a, b) => -a.layer.CompareTo (b.layer));
        Vector2Int usedPos = new Vector2Int (-1, -1);
        int x, y;
        if (spriteData.Count > 0) {
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridHeight; j++) {
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

        return Sprite.Create (texture, new Rect (0, 0, gridWidth, gridHeight), new Vector2 (0.5f, 0.5f));
    }

    /// <summary>
    /// 根据Sprite列表和配置的贴图最大宽度，自动算出贴图的宽、高，并把sprite逐个画到贴图上，遇到边缘自动换行继续画
    /// </summary>
    /// <param name="mixedSpriteData"></param>
    /// <returns></returns>
    private Texture2D DrawTexture (List<Sprite> mixedSpriteData) {
        textureWidth = Mathf.Min (maxDrawTextureWidth, mixedSpriteData.Count * gridWidth);
        int col = textureWidth / gridWidth;
        int row = 1;
        if (mixedSpriteData.Count * gridWidth > maxDrawTextureWidth) {
            row = Mathf.Max (1, Mathf.CeilToInt (1f * mixedSpriteData.Count / col));
            textureHeight = row * gridHeight;
        } else {
            textureHeight = gridHeight;
        }
        Texture2D texture = new Texture2D (textureWidth, textureHeight);
        Color color;
        int x, y, drawHeight;
        for (int k = 0; k < mixedSpriteData.Count; k++) {
            for (int i = 0; i < gridWidth; i++) {
                for (int j = 0; j < gridHeight; j++) {
                    x = (int) mixedSpriteData[k].textureRect.x;
                    y = (int) mixedSpriteData[k].textureRect.y;
                    color = mixedSpriteData[k].texture.GetPixel (x + i, y + j);
                    drawHeight = Mathf.FloorToInt (1f * k / col) * gridHeight; //根据列数自动换行
                    texture.SetPixel (i + k * gridWidth, textureHeight - j - drawHeight - 1, color); //从左上角开始画
                    // texture.SetPixel (i + k * gridWidth, j + drawHeight, color); //从左下角开始画
                }
            }
        }
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

        Debug.LogFormat ("Save texture file to path: {0}.png  Texture width:{1}, height:{2}", path, textureWidth, textureHeight);
    }

    /// <summary>
    /// 传入一个sprite，返回一个texture
    /// </summary>
    /// <param name="sp"></param>
    /// <returns></returns>
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
}

public enum ORNT { M, N, NE, E, SE, S, SW, W, NW }

public class BaseSpriteData {
    public Sprite sprite = null;
    public Vector2Int pos = new Vector2Int (-1, -1);
    public int layer = -1;
    public ORNT ornt = ORNT.M;
}