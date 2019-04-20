using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceTower : MonoBehaviour
{
    public bool buildTower;
    public Button button;
    public GameObject tower;
    private GameObject currTower;
    private Vector3 currentPos;
    private float gridSize = 1.0f;
    // Start is called before the first frame update
    void Start() {
        currentPos = new Vector3(0,0,0);
        button.onClick.AddListener(this.clicked);
    }

    void clicked()
    {
        buildTower = true;
        
        GameObject e = (GameObject) Instantiate(tower);
        e.AddComponent<SelectableUnitComponent>();
        
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
                float x = Mathf.Floor(hit.point.x / gridSize) * gridSize;
                float y = Mathf.Floor(hit.point.y / gridSize) * gridSize;
                float z = Mathf.Floor(hit.point.z / gridSize) * gridSize;
                GridMap grid = World.Instance.grid;
                if (hit.collider.tag == "PlacebleObject" && grid.IsBuildable((int)x,(int)z) && y >= 8) {
                    
                    Vector3 n = new Vector3(x, y, z);
                    currTower.transform.position = n;
                    currTower.GetComponent<Turret>().isBuilding = false;
                    currTower = null;

                    if (Input.GetKey("left shift")) {
                        clicked();
                    }
                    grid.OcupyPosition((int)x, (int)z);
                }

            }
        }
        else if (currTower != null) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit) && currTower) {
                float x = Mathf.Floor(hit.point.x / 1.0f) * 1;
                float y = Mathf.Floor(hit.point.y / 1.0f) * 1;
                float z = Mathf.Floor(hit.point.z / 1.0f) * 1;
                GridMap grid = World.Instance.grid;
 
                if (hit.collider.tag == "PlacebleObject" && y >=8) {

                    Vector3 n = new Vector3(x, y, z);
                    //Debug.Log(n.ToString());
                    currTower.transform.position = n;
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
