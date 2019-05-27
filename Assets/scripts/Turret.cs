using UnityEngine;
using System.Collections;
using System;

public class Turret : MonoBehaviour {

    private Transform target;
    private Enemy targetEnemy;

    [Header("General")]
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
    public Material laserMaterial;

    

    [Header("Use Laser (default)")]
    public bool useLaser = false;
    public LineRenderer laserRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    public int damageOverTime = 30;
    public float slowAmount = .5f;


    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    // Use this for initialization
    void Start() {
        towerGUI = new UnitTowerGUI(this, World.Instance.towerDetails);
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        gameObject.AddComponent<LineRenderer>();
        CreateRangeIndicator();
        CreateLaserBeam();
        impactEffect.Stop();
        if(impactLight)
            impactLight.enabled = false;
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

    // Update is called once per frame
    void Update() {
        if (!isBuilding) { 
            if (target == null) {
                if(useLaser && laserRenderer.enabled) {
                    laserRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
                return;
            }

            LockOnTarget();
            if (useLaser) {
                Laser();
            }
            else { 
                if (fireCountdown <= 0f) {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
                fireCountdown -= Time.deltaTime;
            }
        }
    }

    void Laser() {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowAmount);
        if (!laserRenderer.enabled) {
            laserRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        laserRenderer.SetPosition(0, firePoint.position);
        if (target == null)
            return;
        laserRenderer.SetPosition(1, target.position);
        Vector3 direction = firePoint.position - target.position;

        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
    }

    void LockOnTarget() {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public void SellTurret() {
        Debug.Log("OMEGALUL");
        PlayerStats.Money += turretBlueprint.GetSellAmount();
        //GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        //Destroy(effect, 5f);
        towerGUI.DeactivateUI();
        World.Instance.GetComponent<UnitGUI>().currentUI = null;

        DestroyImmediate(this.gameObject);
        //turretBlueprint = null;
    }


    void Shoot() {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Seek(target);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

   

    void CreateRangeIndicator() {
        var segments = 360;
        var line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = rangeIndicatorM;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.positionCount = (segments + 1);

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];
        float theta = 0f;
        float theta_scale = 0.01f;
        for (int i = 0; i < pointCount; i++) {
            theta += (2.0f * Mathf.PI * theta_scale);
            line.SetPosition(i, new Vector3(Mathf.Sin(theta) * range, 0.5f, Mathf.Cos(theta) * range));
        }

    }

    void CreateLaserBeam() {
        GameObject g = new GameObject();
        g.transform.SetParent(transform, false);
        //var segments = 360;
        laserRenderer = g.AddComponent<LineRenderer>();
        laserRenderer.material = laserMaterial;
        laserRenderer.startColor = new Color(70, 130, 180);
        laserRenderer.endColor = new Color(240, 248, 255);
        laserRenderer.startWidth = 0.3f;
        laserRenderer.endWidth = 0.3f;
        laserRenderer.positionCount = 2;
        //laserRenderer.SetPosition(0, new Vector3(0,3,0));
        //laserRenderer.SetPosition(0, new Vector3(0, 3, 5));
    }


    public void activateUI() {
        /*IGUI aux = World.Instance.GetComponent<UnitGUI>().currentUI;
        if (aux != null) {
            aux.DeactivateUI();
        }*/
        World.Instance.GetComponent<UnitGUI>().currentUI = towerGUI;
        towerGUI.ActivateUI();
    }
}