//This script is attached to a Spawn Game Object
//On Start up will identify the Partition Game Object associated with it's location
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Vector3 SpawnPoint { private set; get; }

    private void Start()
    {
        SpawnPoint = transform.position;
    }
}
