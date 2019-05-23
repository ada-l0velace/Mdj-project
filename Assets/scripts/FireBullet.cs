using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : Bullet {
    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float modifier = 0.5f;

        if (e.eType == Enemy.ElementType.WATER) {
            modifier -= 0.5f;
        }
        else if (e.eType == Enemy.ElementType.ICE) {
            modifier += 1.00f;
        }

        if (e != null) {
            ImpactEnemyPhysics(e);
            e.TakeDamage(damage * modifier);
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