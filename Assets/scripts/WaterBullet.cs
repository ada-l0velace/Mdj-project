using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBullet : Bullet {
    float modifier = 1.0f;

    public override float GetDamageIce() => GetDamage() * (modifier - 0.5f);
    public override float GetDamageFire() => GetDamage() * (modifier + 1.0f);
    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float damageT = GetDamage()*modifier;

        if (e.eType == Enemy.ElementType.ICE) {
            damageT-= GetDamageIce();
        }
        else if (e.eType == Enemy.ElementType.FIRE) {
            damageT = GetDamageFire();
        }

        if (e != null) {
            ImpactEnemyPhysics(e);
            e.TakeDamage(damageT, true);
        }
    }
}
