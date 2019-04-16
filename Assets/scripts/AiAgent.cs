using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour {
    public NavMeshAgent m_Agent;
    public Vector3 destination;
    public bool moving;
    // Start is called before the first frame update
    void Start(){
        m_Agent= gameObject.AddComponent<NavMeshAgent>();
        moving = false;
        destination = new Vector3(-21f, 4f, 26f);
        gameObject.AddComponent<SelectableUnitComponent>();

    }

    // Update is called once per frame
    void Update() {
        if(!moving) { 
            m_Agent.destination =destination;
            moving = true;
        }
        
        if (Vector3.Distance(transform.position, destination) <= 0.2f) {
            DestroyImmediate(gameObject);
        }
    }
}
