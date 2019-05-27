using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledObject {
    void onObjectSpawn(Transform location, string text, GameObject canvas);
}
