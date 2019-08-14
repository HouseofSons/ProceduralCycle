using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int BlockSize { get; private set; }
    public static int GridSize { get; private set; }
    public static Transform GameOrientation { get; private set; }

    void Awake()
    {
        BlockSize = 4;
        GridSize = 64;
        GameOrientation = this.gameObject.transform;
    }
}
