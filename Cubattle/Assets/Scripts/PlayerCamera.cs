using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static List<PlayerCamera> playerCameras = new List<PlayerCamera>();
    private Player player;

    // Start is called before the first frame update
    void Awake()
    {
        playerCameras.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void AssignPlayer(Player p)
    {
        player = p;
    }

    private void FollowPlayer()
    {
        if (System.Math.Abs(Quaternion.Angle(player.transform.rotation, this.transform.rotation)) > Mathf.Epsilon)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, player.transform.GetChild(0).transform.position, 8 * Time.deltaTime);
        }
        else
        {
            this.transform.position = player.transform.GetChild(0).transform.position;
            if (player.RotatingPlayerInPlace)
            {//Ends Player.RotatePlayer Coroutine
                player.RotatingPlayerInPlace = false;
            }
        }
        this.transform.LookAt(player.transform.position);
    }
}