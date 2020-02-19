using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partition : MonoBehaviour
{
    public Vector3Int Origin { private set; get; }
    public int Width { set; get; }
    public int Depth { set; get; }

    void Start()
    {
        Origin = new Vector3Int(
            Mathf.RoundToInt(this.transform.position.x),
            Mathf.RoundToInt(this.transform.position.y),
            Mathf.RoundToInt(this.transform.position.z));
        Width = Mathf.RoundToInt(this.transform.lossyScale.x);
        Depth = Mathf.RoundToInt(this.transform.lossyScale.z);
    }
}
