using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Partition> Partitions { get; set; }

    void Start()
    {
        foreach (Partition p in transform.GetComponentsInChildren<Partition>())
        {
            Partitions.Add(p);
        }
    }
}
