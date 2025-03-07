﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.EventSystems;

public class UnitSelectionComponent : MonoBehaviour {
    bool isSelecting = false;
    Vector3 mousePosition1;
    public GameObject unitSelected;
  

    public GameObject selectionCirclePrefab;

    private void Start() {

    }
    private void DisableSelection(SelectableUnitComponent selectableObject) {
        if(!MouseInputUIBlocker.BlockedByUI) { 
            //Destroy(selectableObject.selectionCircle.gameObject);
            selectableObject.selectionCircle = null;
            LineRenderer lr = selectableObject.gameObject.GetComponent<LineRenderer>();
            if (lr != null)
                lr.enabled = false;
            selectableObject.unitSelection.enabled = false;
        
            UnitGUI a = World.Instance.GetComponent<UnitGUI>();
            if (a.currentUI != null) {
                a.currentUI.DeactivateUI();
                a.currentUI = null;
            }
        }


    }

    private void EnableSelection(SelectableUnitComponent selectableObject) {
        LineRenderer lr = selectableObject.unitSelection;
        if (lr != null)
            lr.enabled = true;
        selectableObject.unitSelection.enabled = true;
        Enemy e = selectableObject.enemy;
        Turret t = selectableObject.turret;
        UnitGUI a = World.Instance.unitGUI;
        if (a.currentUI != null) {
            a.currentUI.DeactivateUI();
        }
        if (e) {
            e.activateUI();
        }
        else if(t) {
            t.activateUI();
        }

    }
    void Update() {
        // If we press the left mouse button, begin selection and remember the location of the mouse
        if (Input.GetMouseButtonDown(0)) {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }

        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0)) {
            isSelecting = false;
        }

        // right click (needs fixing not really a good way to do this)
        if (Input.GetMouseButtonDown(1)) {
            DeselectAll();
            Boolean b = false;
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (IsWithinBounds(selectableObject.gameObject) && !b) {
                    //selectableObject.unitSelection.enabled = true;//Instantiate(selectionCirclePrefab);
                    EnableSelection(selectableObject);
                    b = true;
                }
                else {
                    if (selectableObject.unitSelection.enabled) {
                        DisableSelection(selectableObject);
                    }
                }
            }
        }

        // Highlight all objects within the selection box
        if (isSelecting) {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (IsWithinSelectionBounds(selectableObject.gameObject)) {
                    if (!selectableObject.unitSelection.enabled) {
                        //selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        EnableSelection(selectableObject);
                    }
                }
                else if (selectableObject.unitSelection.enabled) {
                        DisableSelection(selectableObject);
                }
            }
        }
    }

    public void DeselectAll() {
        foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
            if (selectableObject.selectionCircle != null) {
                DisableSelection(selectableObject);
            }
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject) {
        if (!isSelecting)
            return false;

        var camera = Camera.main;

       
        var viewportBounds = Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    public bool IsWithinBounds(GameObject gameObject) {
        var camera = Camera.main;
        Vector3 center = Input.mousePosition;
        Vector3 upperBound = new Vector3(center.x - gameObject.transform.localScale.x*12, center.y+ gameObject.transform.localScale.y*12, 0);
        Vector3 lowerBound = new Vector3(center.x + gameObject.transform.localScale.x*12, center.y - gameObject.transform.localScale.y*12, 0);
        var viewportBounds = Utils.GetViewportBounds(camera, upperBound, lowerBound);
        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    void OnGUI() {
        if (isSelecting) {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}