using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    private Transform target;
    private Enemy targetEnemy;
    
    [Header("General")]
    public TurretBlueprint turretBlueprint;
    public float range = 15f;
    public bool isBuilding = true;
    public Material rangeIndicatorM;

    [Header("Use Bullets (default)")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    public ParticleSystem impactEffect;


    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    // Use this for initialization
    void Start() {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        gameObject.AddComponent<LineRenderer>();
        CreatePoints();
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

    void LockOnTarget() {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

   

    void CreatePoints() {
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
}