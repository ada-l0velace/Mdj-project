using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap {
    int chunkSize;
    int mapSize;

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
