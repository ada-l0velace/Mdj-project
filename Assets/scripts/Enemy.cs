using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public enum ElementType { WATER, FIRE, EARTH, ICE };

    public float startSpeed = 10f;
    public ElementType eType;
    public Transform endPosition;
    public GameObject canvas;
    public bool moving;

    [HideInInspector]
    public IGUI enemyGUI;
    public NavMeshAgent m_Agent;
    public float speed;
    private PIDRigidbody pid;
   
    public float startHealth = 100;
    public float health;
    public LineRenderer unitSelection;
    public int worth = 50; 
    public GameObject deathEffect;
    [Header("Unity Stuff")]
    public Slider healthBar;
    private bool isDead = false;

    public void Start() {
        this.enabled = true;
        speed = startSpeed;
        health = startHealth;
        gameObject.tag = "Enemy";
        SelectableUnitComponent suc = gameObject.AddComponent<SelectableUnitComponent>();
        unitSelection = suc.unitSelection;
        endPosition = World.Instance.endPosition;
        canvas = World.Instance.canvas;
        enemyGUI = new UnitEnemyGUI(this, World.Instance.selectedDetails);
        healthBar = Instantiate(World.Instance.healthBar, Camera.main.WorldToScreenPoint((Vector3.up * 0.1f) + transform.position), Quaternion.identity, canvas.transform);
        moving = false;
    }

    private void Update() {
        if (!moving) {
            if (m_Agent == null)
                m_Agent = gameObject.AddComponent<NavMeshAgent>();
            m_Agent.destination = endPosition.transform.position;
            m_Agent.updatePosition = false;
            m_Agent.updateRotation = false;
            m_Agent.updateUpAxis = false;
            m_Agent.autoRepath = true;

            pid = new PIDRigidbody(
                new Vector3(1000, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(1000, 0, 1000),
                new Vector3(0, 0, 0)
                );

            moving = true;
        }
        if (moving)
        {
            Vector3 desiredVelocity = m_Agent.desiredVelocity;
            Vector3 desiredOrientation = Quaternion.LookRotation(desiredVelocity, Vector3.up).eulerAngles;
            pid.Update(GetComponent<Rigidbody>(), desiredVelocity, desiredOrientation, Time.deltaTime);
            Rigidbody rb = GetComponent<Rigidbody>();
            m_Agent.nextPosition = transform.position;
        }
        //Debug.Log(Vector3.Distance(transform.position, endPosition.transform.position));
        if (Vector3.Distance(transform.position, endPosition.transform.position) <= 2.5f) {
            Die();
            //GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            PlayerStats.Lives--;
            //UnitSpawner.EnemiesAlive--;
            //Destroy(effect, 5f);
            //DestroyImmediate(healthBar.gameObject);
            //DestroyImmediate(gameObject);
            return;
        }
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector3.up * 0.1f) + transform.position);
        if (unitSelection != null) {
            unitSelection.transform.position = transform.position+ (Vector3.up * 0.1f);
        }
        else {
            unitSelection = gameObject.GetComponent<SelectableUnitComponent>().unitSelection;
        }
        //previewCam.transform.position = Camera.main.WorldToScreenPoint((Vector3.forward * 1) + transform.position);
    }

    public void TakeDamage(float amount) {
        health -= amount;

        healthBar.value = health / startHealth;

        
        //selectedUnit.UpdateHealthBar(s, healthBar.value);
        if (health <= 0 && !isDead) {
            Die();
        }
    }

    public void Slow(float pct) {
        speed = startSpeed * (1f - pct);
    }

    void Die() {
        isDead = true;

        PlayerStats.Money += worth;

        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        //healthBar.transform.SetParent(null);
        DestroyImmediate(healthBar.gameObject);
        UnitSpawner.EnemiesAlive--;
        UnitGUI a = World.Instance.GetComponent<UnitGUI>();
        if(a.currentUI != null && a.currentUI.GetHashCode() == enemyGUI.GetHashCode())
            a.currentUI = null;
        enemyGUI.DeactivateUI();
        DestroyImmediate(unitSelection.gameObject);
        DestroyImmediate(gameObject);
    }

    public void activateUI() {
        /*IGUI aux = World.Instance.GetComponent<UnitGUI>().currentUI;
        if (aux != null) {
            aux.DeactivateUI();
        }*/
        World.Instance.GetComponent<UnitGUI>().currentUI = enemyGUI;
        enemyGUI.ActivateUI();
    }
}