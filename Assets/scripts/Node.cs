using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    int x;
    int y;
    bool buildable;
    public Node(int x, int y) {
        this.x = x;
        this.y = y;
        buildable = true;
    }

    public bool CanBuild() {
        return buildable;
    }

    public void BuildAt(bool build) {
        buildable = build;
    }

}
