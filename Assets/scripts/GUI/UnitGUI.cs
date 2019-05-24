using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGUI : MonoBehaviour {
    public IGUI currentUI;

    void Update() {
        if (currentUI != null) {
            currentUI.ActivateUI();
        }
        //else {
        //    currentUI.DeactivateUI();
        //}
    }


}
