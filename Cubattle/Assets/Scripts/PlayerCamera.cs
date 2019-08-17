using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static List<PlayerCamera> playerCameras = new List<PlayerCamera>();
    private Transform playerCameraTargetTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerCameras.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void ConfigurePlayerCameraTarget(Transform t)
    {
        playerCameraTargetTransform = t;
    }

    private void FollowPlayer()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, playerCameraTargetTransform.position, 6 * Time.deltaTime);

        //When Map Rotates
        if (Mathf.Abs(Quaternion.Dot(playerCameraTargetTransform.parent.transform.rotation, this.transform.rotation)) < 1)
        {
            this.transform.LookAt(playerCameraTargetTransform.parent.transform.position);
        }
        else
        {
            this.transform.position = playerCameraTargetTransform.position;
            this.transform.rotation = LevelManager.MapOrientation.rotation;
        }
    }
}