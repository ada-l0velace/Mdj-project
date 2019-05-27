using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap {
    int chunkSize;
    int mapSize;
    private float gridSize = 1.0f;


    Node[,] nodes;
    public GridMap(int chunkSize, int mapSize) {
        this.chunkSize = chunkSize;
        this.mapSize = mapSize;
        nodes = new Node[chunkSize * mapSize*2, chunkSize * mapSize * 2];
        for (int i = 0; i < chunkSize * mapSize * 2; i++) {
            for (int j = 0; j < chunkSize * mapSize * 2; j++) {
                nodes[i, j] = new Node(i, j);
            }
        }
    }

    public Node GetNode(int x, int y) {
        return nodes[x, y];
    }

    public bool BuildAt(RaycastHit hit, GameObject currTower) {

        float x = Mathf.Floor(hit.point.x / gridSize) * gridSize;
        float y = Mathf.Floor(hit.point.y / gridSize) * gridSize;
        float z = Mathf.Floor(hit.point.z / gridSize) * gridSize;
        GridMap grid = World.Instance.grid;
        Turret turret = currTower.GetComponent<Turret>();
        bool canBuy = turret.turretBlueprint.cost <= PlayerStats.Money;
        bool isPlacebleObject = hit.collider.tag == "PlacebleObject" && grid.IsBuildable((int)x, (int)z) && y >= 8 && canBuy;
        if (isPlacebleObject) {

            Vector3 n = new Vector3(x, y, z);
            currTower.transform.position = n;
            turret.isBuilding = false;
            turret.GetComponent<LineRenderer>().enabled = false;

            turret.GetComponent<SelectableUnitComponent>().unitSelection.enabled = false;
            currTower = null;

            //if (Input.GetKey("left shift")) {
            //    clicked();
            //}

            grid.OcupyPosition((int)x, (int)z);
            PlayerStats.Money -= turret.turretBlueprint.cost;
        }
        return isPlacebleObject;
    }

    public int convertToLocal(int n) {
        if (n < 0) {
            return Mathf.Abs(n);
        }
        return (chunkSize * mapSize) + n;
    }
    public bool IsBuildable(int x, int y) {
        x= convertToLocal(x);
        y = convertToLocal(y);
        return nodes[x, y].CanBuild();
    }

    public void OcupyPosition(int x, int y) {
        x = convertToLocal(x);
        y = convertToLocal(y);
        nodes[x, y].BuildAt(false);
    }

    public void UnOcupyPosition(int x, int y) {
        x = convertToLocal(x);
        y = convertToLocal(y);
        nodes[x, y].BuildAt(true);
    }


}
