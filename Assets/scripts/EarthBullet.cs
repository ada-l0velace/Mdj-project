using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBullet : Bullet {
    float modifier = 1.0f;

    public override float GetDamageIce() => GetDamage() * (modifier + 1.0f);
    public override float GetDamageWater() => GetDamage() * (modifier - 0.5f);
    
    protected override void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        float damageT = GetDamage() *modifier;

        if (e.eType == Enemy.ElementType.WATER) {
            damageT = GetDamageWater();
        }
        else if (e.eType == Enemy.ElementType.ICE) {
            damageT = GetDamageIce();
        }

        if (e != null) {
            ImpactEnemyPhysics(e);
            e.TakeDamage(damageT,true);
        }
    }
}
