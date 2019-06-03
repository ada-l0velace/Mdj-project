using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public enum ElementType { WATER, FIRE, EARTH, ICE };

    public float startSpeed = 10f;
    public ElementType eType;
    public bool isBoss;
    [HideInInspector]
    public Transform endPosition;
    [HideInInspector]
    public GameObject canvas;
    [HideInInspector]
    public bool moving;

    [HideInInspector]
    public IGUI enemyGUI;
    [HideInInspector]
    public NavMeshAgent m_Agent;
    [HideInInspector]
    public NavMeshObstacle m_Obstacle;
    private PIDRigidbody pid;
    public float startHealth = 400;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public LineRenderer unitSelection;
    public int worth = 50;
    public GameObject deathEffect;
    [Header("Unity Stuff")]
    [HideInInspector]
    public Slider healthBar;
    private bool isDead = false;
    [HideInInspector]
    public bool isSlowed = false;
    private NavMeshPath path;
    Rigidbody rb;
    public void Start() {
        this.enabled = true;
        FloatingTextController.Initialize();
        if (m_Agent == null) {
            m_Agent = gameObject.AddComponent<NavMeshAgent>();
            m_Agent.stoppingDistance = 1f;
            m_Agent.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        m_Obstacle = gameObject.AddComponent<NavMeshObstacle>();
        m_Obstacle.enabled = false;
        startSpeed = m_Agent.speed;
        health = startHealth;
        gameObject.tag = "Enemy";
        SelectableUnitComponent suc = gameObject.AddComponent<SelectableUnitComponent>();
        //gameObject.AddComponent<NavMeshObstacle>();
        unitSelection = suc.unitSelection;
        endPosition = World.Instance.endPosition;
        canvas = World.Instance.canvas;
        enemyGUI = new UnitEnemyGUI(this, World.Instance.selectedDetails);
        healthBar = Instantiate(World.Instance.healthBar, Camera.main.WorldToScreenPoint((Vector3.up * 0.1f) + transform.position), Quaternion.identity, canvas.transform);
        moving = false;
        m_Agent.enabled = true;
        path = new NavMeshPath();

    }

    private void Update() {
        if (!moving) {
            if (m_Agent == null) {
                m_Agent = gameObject.AddComponent<NavMeshAgent>();
            }
            m_Agent.destination = endPosition.transform.position;
            if(rb) { 
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
            }
            moving = true;
        }
        if (moving) {
            /*GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject e in gos) {
                Enemy o = e.GetComponent<Enemy>();
                if (o.GetHashCode() == this.GetHashCode())
                    continue;

                if ((transform.position - e.transform.position).sqrMagnitude < Mathf.Pow(m_Agent.stoppingDistance, 2) && o.moving) {
                    m_Agent.enabled = false;
                    m_Obstacle.enabled = true;
                    break;
                }
                else {
                    m_Obstacle.enabled = false;
                    m_Agent.enabled = true;
                    m_Agent.destination = endPosition.transform.position;
                }
            }*/
            //NavMesh.CalculatePath(transform.position, endPosition.transform.position, NavMesh.AllAreas, path);
            //for (int i = 0; i < path.corners.Length - 1; i++)
            //    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            Vector3 desiredVelocity = m_Agent.desiredVelocity;
            if(desiredVelocity != Vector3.zero && rb) { 
                Vector3 desiredOrientation = Quaternion.LookRotation(desiredVelocity, Vector3.up).eulerAngles;
                pid.Update(rb, desiredVelocity, desiredOrientation, Time.deltaTime);
                m_Agent.nextPosition = transform.position;
                m_Agent.destination = endPosition.transform.position;
            }
            
            
            if (!isSlowed)
                m_Agent.speed = startSpeed;
            isSlowed = false;
            //m_Agent.enabled = false;
        }
        //Debug.Log(Vector3.Distance(transform.position, endPosition.transform.position));
        if (Vector3.Distance(transform.position, endPosition.transform.position) <= 2.5f) {
            int saved = PlayerStats.Money;
            Die();
            PlayerStats.Money = saved;
            //GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            if(!isBoss)
                PlayerStats.Lives--;
            else {
                PlayerStats.Lives -= 2;
            }
            //UnitSpawner.EnemiesAlive--;
            //Destroy(effect, 5f);
            //DestroyImmediate(healthBar.gameObject);
            //DestroyImmediate(gameObject);
            return;
        }
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector3.up * 0.1f) + transform.position);
        healthBar.transform.SetAsFirstSibling();
        if (unitSelection != null) {
            unitSelection.transform.position = transform.position + (Vector3.up * 0.1f);
        }
        else {
            unitSelection = gameObject.GetComponent<SelectableUnitComponent>().unitSelection;
        }
        //previewCam.transform.position = Camera.main.WorldToScreenPoint((Vector3.forward * 1) + transform.position);
    }

    public void TakeDamage(float amount) {
        //Debug.Log(health + " | " + amount + " | " + healthBar.value);
        
        health -= amount;
        healthBar.value = health / startHealth;
        healthBar.transform.SetAsFirstSibling();



        //selectedUnit.UpdateHealthBar(s, healthBar.value);
        if (health <= 0 && !isDead) {
            Die();
        }
    }

    public void TakeDamage(float amount, bool showDamage) {
        if(showDamage)
            FloatingTextController.CreateFloatingText(amount.ToString(), transform);
        TakeDamage(amount);
    }

    public void Slow(float pct) {
        if (m_Agent) { 
            m_Agent.speed = startSpeed * (1f - pct);
            isSlowed = true;
        }
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
        if (a.currentUI != null && a.currentUI.GetHashCode() == enemyGUI.GetHashCode())
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