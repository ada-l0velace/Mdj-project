using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class script : MonoBehaviour
{
    public Button button;
    public GameObject tower;
    public GameObject enemy;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(this.clicked);
    }

    void clicked()
    {
        GameObject e = (GameObject) Instantiate(tower);
        e.transform.position = new Vector3(Random.Range(-1, 1) * 500, 1, Random.Range(-1, 1) * 500);
        count++;
        Debug.Log("Count: " + count);
    }

    // Update is called once per frame
    void Update() {
    }
}
