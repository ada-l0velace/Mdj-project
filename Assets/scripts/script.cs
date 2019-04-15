using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class script : MonoBehaviour
{
    public bool buildTower;
    public Button button;
    public GameObject tower;
    private GameObject currTower;
    private Vector3 currentPos;
    // Start is called before the first frame update
    void Start() {
        currentPos = new Vector3(0,0,0);
        button.onClick.AddListener(this.clicked);
    }

    void clicked()
    {
        buildTower = true;
        
        GameObject e = (GameObject) Instantiate(tower);
        currTower = e;
        currentPos.y = transform.position.y;
        //e.transform.SetParent(gameObject.transform);
        //e.transform.localPosition = new Vector3(Input.mousePosition.x, 1, Input.mousePosition.z);
        /*
        count++;
        Debug.Log("Count: " + count);*/
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity) && currTower) {
                if (hit.collider.tag == "PlacebleObject") {
                    currTower.transform.position = hit.point;
                }   
                //Debug.DrawRay(hit);
                //Debug.Log(hit.transform.position);
                currTower = null;
            }
        }
        else if (currTower != null) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit) && currTower) {
                if (hit.collider.tag == "PlacebleObject") {
                    currTower.transform.position = hit.point;
                }
                //Debug.DrawRay(hit);
                //Debug.Log(hit.transform.position);
            }
        }
        //Debug.Log(Input.mousePosition);
    }

    void OnMouseOver() {
        /*Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("WTFF");
        if (buildTower) {
            currTower.transform.localPosition = new Vector3(pz.x, 1, pz.z);
        }*/
    }
}
