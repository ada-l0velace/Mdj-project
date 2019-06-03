using UnityEngine;
using UnityEngine.UI;

public class LaserBeam : Turret {

    [Header("Use Laser (default)")]
    public LineRenderer laserRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    public int damageOverTime = 30;
    public float slowAmount = .5f;
    public Material laserMaterial;
    float modifier = 0.5f;
    protected new void Start() {
        base.Start();
        CreateLaserBeam();
        impactEffect.Stop();
        if (!impactLight) {
            impactLight = impactEffect.GetComponentInChildren<Light>();
            impactLight.enabled = false;
        }
        laserRenderer.enabled = false;
    }

    protected new void Update() {
        if (!isBuilding) {
            
            if (target == null) {
                if (laserRenderer.enabled) {
                    laserRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;
                }
                return;
            }
            else { 
                LockOnTarget();
                Laser();
            }
        }
    }

    public float GetDamageIce() => damageOverTime * (modifier / 2);
    public float GetDamageWater() => damageOverTime * (modifier * 2);

    void AttackEnemy(Enemy e) {
        float damageT = damageOverTime * modifier;
        if (e.eType == Enemy.ElementType.ICE) {
            damageT = GetDamageIce();
        }
        else if (e.eType == Enemy.ElementType.WATER) {
            damageT = GetDamageWater();
        }

        if (e != null) {
            //ImpactEnemyPhysics(e);
            e.TakeDamage(damageT * Time.deltaTime);
            e.Slow(slowAmount);
        }
    }

    void Laser() {
        AttackEnemy(targetEnemy);
        
        if (!laserRenderer.enabled) {
            
            laserRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        laserRenderer.SetPosition(0, firePoint.position);
        laserRenderer.SetPosition(1, target.position);
        Vector3 direction = firePoint.position - target.position;

        impactEffect.transform.position = target.position + direction.normalized;
        impactEffect.transform.rotation = Quaternion.LookRotation(direction);
        /*Rigidbody rb = targetEnemy.GetComponent<Rigidbody>();
        float disturb = 80f;

        Vector3 vec = new Vector3(Random.value - .5f, Random.value - .5f, Random.value - .5f);
        rb.AddForce(vec * disturb);

        Vector3 vec2 = new Vector3(Random.value - .5f, Random.value - .5f, Random.value - .5f);
        rb.AddTorque(vec2 * 30);*/


    }

    void CreateLaserBeam() {
        GameObject g = new GameObject();
        g.transform.SetParent(transform, false);
        laserRenderer = g.AddComponent<LineRenderer>();
        laserRenderer.material = laserMaterial;
        laserRenderer.startColor = new Color(70, 130, 180);
        laserRenderer.endColor = new Color(240, 248, 255);
        laserRenderer.startWidth = 0.3f;
        laserRenderer.endWidth = 0.3f;
        laserRenderer.positionCount = 2;
    }

    public override void UpdateStats(Text[] texts) {
        texts[3].text = name.Replace("(Clone)", "");
        texts[0].text = "Type: " + eType.ToString();
        texts[1].text = "Damage over time:\n "+ damageOverTime ;
        texts[2].text = "Slows Enemies";
    }

}
