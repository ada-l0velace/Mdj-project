using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower : MonoBehaviour
{
    public GameObject tower_head;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tower_head.transform.localEulerAngles += new Vector3(0, 1f, 0);
    }
}
