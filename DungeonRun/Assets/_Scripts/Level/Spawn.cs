//This script is attached to a Spawn Game Object
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Vector3 SpawnPoint { private set; get; }

    private void Start()
    {
        SpawnPoint = transform.position;
    }
}
