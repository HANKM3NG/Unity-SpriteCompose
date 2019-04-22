using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor (typeof (TextureCompose))]
public class TextureComposeEditor : Editor {
    public override void OnInspectorGUI () {
        TextureCompose s = (TextureCompose) target;
        if (GUILayout.Button ("Compose")) {
            s.Compose ();
            AssetDatabase.Refresh ();   //刷新Asset资源目录
        }
        DrawDefaultInspector ();
    }
}
#endif