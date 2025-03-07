﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Turret : MonoBehaviour {

    protected Transform target;
    protected Enemy targetEnemy;

    [Header("General")]
    public Texture2D image;
    public Enemy.ElementType eType;
    public string turretName;
    public IGUI towerGUI;
    public TurretBlueprint turretBlueprint;
    public float range = 15f;
    public bool isBuilding = true;
    public Material rangeIndicatorM;

    [Header("Use Bullets (default)")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public float turnSpeed = 10f;
    public Transform firePoint;

    [HideInInspector]
    public Bullet bullet;
    public Vector3 gridPosition;
    public LineRenderer rangeIndicator;
    // Use this for initialization
    protected void Start() {
        towerGUI = new UnitTowerGUI(this, World.Instance.towerDetails);
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        gameObject.AddComponent<LineRenderer>();
        if(bulletPrefab)
            bullet = bulletPrefab.GetComponent<Bullet>();
        CreateRangeIndicator();
    }

    void UpdateTarget() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range) {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();

        }
        else {
            target = null;
        }

    }

    public virtual void UpdateStats(Text[] texts) {
        texts[3].text = name.Replace("(Clone)", "");
        texts[0].text = "Type: " + eType.ToString();
        texts[1].text = "Damage: \nEarth " + bullet.GetDamageEarth() + "\nFire " + bullet.GetDamageFire() + "\nWater " + bullet.GetDamageWater() + "\nIce " + bullet.GetDamageIce();
        
    }

    // Update is called once per frame
    protected void Update() {
        if (!isBuilding) { 
            if (target == null) {
                return;
            }
            LockOnTarget();
            if (fireCountdown <= 0f) {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
    }

    protected void LockOnTarget() {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void SellTurret() {
        if(!isBuilding)
            PlayerStats.Money += turretBlueprint.GetSellAmount();
        towerGUI.DeactivateUI();
        World.Instance.GetComponent<UnitGUI>().currentUI = null;
        World.Instance.grid.UnOcupyPosition((int)gridPosition.x, (int)gridPosition.z);
        DestroyImmediate(this.gameObject);
    }

    public void UpgradeTurret() {
        if (!turretBlueprint.upgradedPrefab) {
            Debug.Log("No More Upgrades!");
            return;
        }

        if (PlayerStats.Money < turretBlueprint.upgradeCost) {
            Debug.Log("Not enough money to upgrade that!");
            return;
        }
        PlayerStats.Money -= turretBlueprint.upgradeCost;

        //Get rid of the old turret
        Destroy(gameObject);

        //Build a new one
        Turret tower = Instantiate(turretBlueprint.upgradedPrefab, transform.position, Quaternion.identity).GetComponent<Turret>();
        tower.gameObject.AddComponent<SelectableUnitComponent>();
        tower.isBuilding = false;
        tower.rangeIndicator.enabled = false;
        tower.GetComponent<SelectableUnitComponent>().unitSelection.enabled = false;
    }

    public void OccupyPosition(Vector3 vector) {
        gridPosition = new Vector3(vector.x, vector.y, vector.z);
    }


    void Shoot() {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Seek(target);
    }

    void CreateRangeIndicator() {
        var segments = 360;
        rangeIndicator = GetComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = false;
        rangeIndicator.material = rangeIndicatorM;
        rangeIndicator.startWidth = 0.5f;
        rangeIndicator.endWidth = 0.5f;
        rangeIndicator.positionCount = (segments + 1);

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];
        float theta = 0f;
        float theta_scale = 0.01f;
        for (int i = 0; i < pointCount; i++) {
            theta += (2.0f * Mathf.PI * theta_scale);
            rangeIndicator.SetPosition(i, new Vector3(Mathf.Sin(theta) * range, 0.5f, Mathf.Cos(theta) * range));
        }

    }

    public void activateUI() {
        World.Instance.GetComponent<UnitGUI>().currentUI = towerGUI;
        towerGUI.ActivateUI();
        rangeIndicator.enabled = true;
    }
}