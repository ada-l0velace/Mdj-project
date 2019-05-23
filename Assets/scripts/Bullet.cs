using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Transform target;
    private float speed = 70f;
    public GameObject impactEffect;
    public int damage = 50;
    public float explosionRadius = 0f;

    public void Seek(Transform _target) {
        target = _target;
    }

    // Update is called once per frame
    void Update() {
        
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        //Debug.Log("WTF");
        if (dir.magnitude <= distanceThisFrame) {
           
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    protected virtual void Damage(Transform enemy) {
        Enemy e = enemy.GetComponent<Enemy>();
        if (e != null) {
            e.TakeDamage(damage);
        }
    }

    protected virtual void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders) {
            if (collider.tag == "Enemy") {
                Damage(collider.transform);
            }
        }
    }

    protected void ImpactEnemyPhysics(Enemy e)
    {
        Rigidbody rb = e.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 force = transform.TransformDirection(new Vector3(0, 0, 1));
            force = new Vector3(force.x, .2f, force.z).normalized;

            force *= 3f;
            rb.AddForceAtPosition(force, transform.position, ForceMode.Impulse);

            float disturb = 2;
            Vector3 torque = new Vector3(
                (Random.value - .5f) * disturb,
                (Random.value - .5f) * disturb,
                (Random.value - .5f) * disturb
                );
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }

    void HitTarget() {
        GameObject effectInstance = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);

        if (explosionRadius > 0f) {
            Explode();
        }
        else {
            Damage(target);
        }

        Destroy(effectInstance, 5f);
        Destroy(gameObject);
    }
}
