using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour {
    public NavMeshAgent m_Agent;
    public Transform endPosition;
    public bool moving;
    // Start is called before the first frame update
    void Start(){
        m_Agent= gameObject.AddComponent<NavMeshAgent>();
        moving = false;
        //destination = new Vector3(-21f, 4f, 26f);
        
        gameObject.AddComponent<Enemy>();
        //GameObject [] waypoints = GameObject.FindGameObjectsWithTag("WayPoint");
        //Debug.Log(waypoints.GetValue(0));
        //endPosition = (Transform) waypoints.GetValue(0);
        //gameObject.AddComponent<LookAtTarget>();
        //gameObject.AddComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update() {
        if(!moving) {
            m_Agent.destination = endPosition.position;
            moving = true;
        }
        //Debug.Log(Vector3.Distance(transform.position, destination));
        if (Vector3.Distance(transform.position, endPosition.position) <= 1.2f) {
            DestroyImmediate(gameObject);
        }
    }
}
