using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCompose : MonoBehaviour {
    public Sprite Base0, Alpha0, Base1, Alpha1;
    public SpriteData[] Sprites = new SpriteData[9];
    public SpriteRenderer Result;
    private Texture2D tempTex2D;

    public void Compose () {
        tempTex2D = new Texture2D (16, 16);
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                Color gColor0 = Base0.texture.GetPixel ((int) Base0.textureRect.x + i, (int) Base0.textureRect.y + j);
                Color gColor1 = Alpha1.texture.GetPixel ((int) Alpha1.textureRect.x + i, (int) Alpha1.textureRect.y + j);
                if (gColor1.a == 0) {
                    tempTex2D.SetPixel (i, j, gColor0);
                } else {
                    tempTex2D.SetPixel (i, j, gColor1);
                }
            }
        }
       
        tempTex2D.filterMode = FilterMode.Point;
        tempTex2D.Apply ();

        Vector3 rPos = Result.transform.position;
        Rect texRect = new Rect (0, 0, tempTex2D.width, tempTex2D.height);
        Debug.LogFormat ("xmin:{0}, ymin:{1}, xmax:{2}, ymax:{3}", texRect.xMin, texRect.yMin, texRect.xMax, texRect.yMax);
        Sprite sp = Sprite.Create (tempTex2D, texRect, texRect.center);
        Result.sprite = sp;
    }

}

public class SpriteData {
    public Sprite Base;
    public Sprite Edge;
    public int weight;
}