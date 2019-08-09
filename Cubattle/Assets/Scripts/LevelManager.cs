using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Transform Orientation { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Orientation = this.gameObject.transform;
    }
}
