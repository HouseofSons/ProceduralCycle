using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImage : MonoBehaviour
{
    private static GameObject playerImage;

    // Start is called before the first frame update
    void Start()
    {
        playerImage = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(GameManager.GetCamera().transform);
        this.transform.rotation = Quaternion.Euler(new Vector3Int(90,0,0));
    }
}
