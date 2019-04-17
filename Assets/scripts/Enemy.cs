using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public float startSpeed = 10f;
   
    public Transform endPosition;
    public GameObject canvas;
    public bool moving;
    [HideInInspector]
    public NavMeshAgent m_Agent;
    public float speed;

    public float startHealth = 100;
    private float health;

    public int worth = 50;

    public GameObject deathEffect;

    [Header("Unity Stuff")]
    public Slider healthBar;
    //Image hpBar;

    private bool isDead = false;

    void Start() {
        this.enabled = true;
        speed = startSpeed;
        health = startHealth;
        gameObject.tag = "Enemy";
        gameObject.AddComponent<SelectableUnitComponent>();
        if (m_Agent == null)
            m_Agent = gameObject.AddComponent<NavMeshAgent>();
        endPosition = World.Instance.endPosition;
        canvas = World.Instance.canvas;
        //hpBar = Instantiate(healthBar);
        //foreach (Image child in transform) {
        //    hpBar = child;
        //    break;
        //}
        healthBar = Instantiate(World.Instance.healthBar);
        healthBar.transform.SetParent(canvas.transform, false);
        moving = false;
    }

    private void Update() {
        if (!moving) {
            if (m_Agent == null)
                m_Agent = gameObject.AddComponent<NavMeshAgent>();
            m_Agent.destination = endPosition.transform.position;
            moving = true;
        }
        //Debug.Log(Vector3.Distance(transform.position, destination));
        if (Vector3.Distance(transform.position, endPosition.transform.position) <= 1.2f) {
            DestroyImmediate(gameObject);
        }
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector3.up * 0.1f) + transform.position);

    }

    public void TakeDamage(float amount) {
        health -= amount;

        healthBar.value = health / startHealth;

        if (health <= 0 && !isDead) {
            Die();
        }
    }

    public void Slow(float pct) {
        speed = startSpeed * (1f - pct);
    }

    void Die() {
        isDead = true;

        //PlayerStats.Money += worth;

        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        //healthBar.transform.SetParent(null);
        DestroyImmediate(healthBar.gameObject);
        //WaveSpawner.EnemiesAlive--;

        DestroyImmediate(gameObject);
    }

}