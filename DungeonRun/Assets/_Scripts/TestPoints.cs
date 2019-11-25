using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoints : MonoBehaviour
{
    int index;

    // Start is called before the first frame update
    void Start()
    {
        index = int.Parse(this.gameObject.transform.name.Substring(6));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Player.WallCollisionPoints[index];
    }
}
