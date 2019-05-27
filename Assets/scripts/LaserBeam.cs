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

    protected new void Start() {
        base.Start();
        CreateLaserBeam();
        impactEffect.Stop();
        if (impactLight)
            impactLight.enabled = false;
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
            LockOnTarget();
            Laser();
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
        texts[3].text = name;
        texts[0].text = "Type: " + eType.ToString();
        texts[1].text = "Damage over time: "+ damageOverTime ;
        texts[2].text = "Slows Enemies";
    }

}
