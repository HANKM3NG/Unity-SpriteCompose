using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor (typeof (SpriteCompose))]
public class SpriteComposeEditor : Editor {
    public override void OnInspectorGUI () {
            SpriteCompose s = (SpriteCompose) target;
            if (GUILayout.Button ("Compose")) {
                s.Compose();
            }   
            DrawDefaultInspector ();
        }
}
#endif