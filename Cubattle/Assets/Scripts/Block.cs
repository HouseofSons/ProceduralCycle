using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public MeshCollider topMC;
    public MeshCollider bottomMC;
    public MeshCollider leftMC;
    public MeshCollider rightMC;
    public MeshCollider frontMC;
    public MeshCollider backMC;

    void Start()
    {
        topMC = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<MeshCollider>();
        bottomMC = this.gameObject.transform.GetChild(1).GetChild(1).GetComponent<MeshCollider>();
        leftMC = this.gameObject.transform.GetChild(1).GetChild(2).GetComponent<MeshCollider>();
        rightMC = this.gameObject.transform.GetChild(1).GetChild(3).GetComponent<MeshCollider>();
        frontMC = this.gameObject.transform.GetChild(1).GetChild(4).GetComponent<MeshCollider>();
        backMC = this.gameObject.transform.GetChild(1).GetChild(5).GetComponent<MeshCollider>();
    }
}
