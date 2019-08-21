using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static List<PlayerCamera> playerCameras = new List<PlayerCamera>();
    private Transform playerCameraTargetTransform;

    public bool RotationInProgress { get; set; }

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
        if (System.Math.Abs(Quaternion.Angle(LevelManager.MapOrientation.rotation, this.transform.rotation)) > Mathf.Epsilon)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, playerCameraTargetTransform.position, 6 * Time.deltaTime);
        }
        else
        {
            this.transform.position = playerCameraTargetTransform.position;
            if (RotationInProgress)
            {
                RotationInProgress = false;
            }
        }
        this.transform.LookAt(playerCameraTargetTransform.parent.transform.position);
    }

    public static bool CamerasTurning()
    {
        foreach(PlayerCamera pc in playerCameras)
        {
            if (pc.RotationInProgress)
            {
                return true;
            }
        }
        return false;
    }
}