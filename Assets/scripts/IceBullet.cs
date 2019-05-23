using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBullet : Bullet {
    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float modifier = 1.0f;

        if (e.eType == Enemy.ElementType.ICE) {
            modifier -= 0.5f;
        }
        else if (e.eType == Enemy.ElementType.WATER) {
            modifier += 1.00f;
        }
        
        if (e != null) {
            ImpactEnemyPhysics(e);
            e.TakeDamage(damage*modifier);
        }
    }
}
