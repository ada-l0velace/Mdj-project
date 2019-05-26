using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : Bullet {

    float modifier = 0.5f;
    public override float GetDamageIce() => GetDamage() * (modifier + 1.0f);
    public override float GetDamageWater() => GetDamage() * (modifier - 0.5f);
    public override float GetDamageFire() => GetDamage() * modifier;
    public override float GetDamageEarth() => GetDamage() * modifier;

    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float damageT = GetDamage() * modifier;

        if (e.eType == Enemy.ElementType.WATER) {
            damageT = GetDamageWater();
        }
        else if (e.eType == Enemy.ElementType.ICE) {
            damageT = GetDamageIce();
        }

        if (e != null) {
            ImpactEnemyPhysics(e);
            e.TakeDamage(damageT);
        }
    }
    protected override void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders) {
            if (collider.tag == "Enemy") {
                Damage(collider.transform);
            }
        }
    }
}