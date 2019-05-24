using UnityEngine;
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
        Destroy(selectableObject.selectionCircle.gameObject);
        selectableObject.selectionCircle = null;
        LineRenderer lr = selectableObject.gameObject.GetComponent<LineRenderer>();
        if (lr != null)
            lr.enabled = false;
        selectableObject.unitSelection.enabled = false;
        
    }

    private void EnableSelection(SelectableUnitComponent selectableObject) {
        LineRenderer lr = selectableObject.gameObject.GetComponent<LineRenderer>();
        if (lr != null)
            lr.enabled = true;
        selectableObject.unitSelection.enabled = true;
    }
    void Update() {
        // If we press the left mouse button, begin selection and remember the location of the mouse
        if (Input.GetMouseButtonDown(0)) {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;

            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (selectableObject.selectionCircle != null) {
                    DisableSelection(selectableObject);
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0)) {
            var selectedObjects = new List<SelectableUnitComponent>();
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (IsWithinSelectionBounds(selectableObject.gameObject)) {
                    selectedObjects.Add(selectableObject);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Selecting [{0}] Units", selectedObjects.Count));
            foreach (var selectedObject in selectedObjects) {
                EnableSelection(selectedObject);
                sb.AppendLine("-> " + selectedObject.gameObject.name);
            }
            Debug.Log(sb.ToString());
            
            isSelecting = false;
        }

        // right click
        if (Input.GetMouseButtonDown(1)) {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (selectableObject.selectionCircle != null) {
                    DisableSelection(selectableObject);
                }
            }
            Boolean b = false;
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (IsWithinBounds(selectableObject.gameObject) && !b) {
                    selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                    EnableSelection(selectableObject);
                    b = true;
                }
                else {
                    if (selectableObject.selectionCircle != null) {
                        DisableSelection(selectableObject);
                    }
                }
            }
        }

        // Highlight all objects within the selection box
        if (isSelecting) {
            foreach (var selectableObject in FindObjectsOfType<SelectableUnitComponent>()) {
                if (IsWithinSelectionBounds(selectableObject.gameObject)) {
                    if (selectableObject.selectionCircle == null) {
                        selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                        EnableSelection(selectableObject);
                        /*selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);
                        Projector p = selectableObject.selectionCircle.GetComponent<Projector>();
                        p.farClipPlane = 2;*/
                        //Debug.Log(p.farClipPlane);
                    }
                }
                else {
                    if (World.Instance.selectedDetails)
                        UnitGUI.enemy = null;
                    if (selectableObject.selectionCircle != null) {
                        DisableSelection(selectableObject);
                    }
                }
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