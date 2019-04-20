using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : Bullet {
    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float modifier = 1.0f;

        if (e.eType == Enemy.ElementType.ICE) {
            modifier -= 0.5f;
        }
        else if (e.eType == Enemy.ElementType.FIRE) {
            modifier += 1.00f;
        }

        if (e != null) {
            e.TakeDamage(damage * modifier);
        }
    }
}
