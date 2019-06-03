using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour {
    private static FloatingText popupText;
    private static GameObject canvas;

    void Start() {
        Initialize();
    }
    public static void Initialize() {
        if(!canvas)
            canvas = GameObject.Find("Canvas");
        if (!popupText)
            popupText = Resources.Load<FloatingText>("Prefabs/UI/PopupTextParent");
    }
    public static void CreateFloatingText(string text, Transform location) {
       ObjectPooler.Instance.SpawnFromPool("Damage").onObjectSpawn(location,text,canvas);

    }
   
}
