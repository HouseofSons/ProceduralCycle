using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Vector3 SpawnPoint { private set; get; }

    private void Start()
    {
        SpawnPoint = this.transform.position;
    }
}
