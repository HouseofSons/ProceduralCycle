using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int BlockSize { get; private set; }
    public static int GridSize { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        BlockSize = 5;
        GridSize = 64;
    }
}
