using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class SelectableUnitComponent : MonoBehaviour {
    public GameObject selectionCircle;
    public LineRenderer unitSelection;
    public Enemy enemy;
    public Turret turret;

    void Start() {
        SelectionPoints();
        unitSelection.enabled = false;
        enemy = GetComponent<Enemy>();
        turret = GetComponent<Turret>();

    }
    private void Update() {
        unitSelection.transform.position = transform.position + (Vector3.up * 0.1f);
    }

    void SelectionPoints() {
        Color c1 = Color.green;
        Color c2 = Color.green;
        var segments = 360;
        GameObject g = new GameObject();
        Turret t = GetComponent<Turret>();
        if(t)
            g.transform.SetParent(transform, false);
        //g.transform.position = transform.position + (Vector3.up * 0.1f);
        unitSelection = g.AddComponent<LineRenderer>();
        unitSelection.material = new Material(Shader.Find("Sprites/Default"));
        unitSelection.useWorldSpace = false;
        //line.material = rangeIndicatorM;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 0.2f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        unitSelection.colorGradient = gradient;

        unitSelection.startWidth = 0.1f;
        unitSelection.endWidth = 0.1f;
        unitSelection.positionCount = segments + 1;

        var pointCount = (segments + 1); // add extra point to make startpoint and endpoint the same to close the circle
                                         //var points = new Vector3[pointCount];

        float theta = 0f;
        float theta_scale = 0.01f;
        for (int i = 0; i < pointCount; i++) {
            theta += (2.0f * Mathf.PI * theta_scale);
            unitSelection.SetPosition(i, new Vector3(Mathf.Sin(theta) * transform.localScale.x, 0.05f, Mathf.Cos(theta) * transform.localScale.z));
        }

        //line.SetPositions(points);
    }
}