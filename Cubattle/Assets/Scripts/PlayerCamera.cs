using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static List<PlayerCamera> playerCameras = new List<PlayerCamera>();
    private Transform playerTransform;

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

    public void AssignPlayerTransform(Transform t)
    {
        playerTransform = t;
    }

    private void FollowPlayer()
    {
        if (System.Math.Abs(Quaternion.Angle(playerTransform.rotation, this.transform.rotation)) > Mathf.Epsilon)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, playerTransform.GetChild(0).transform.position, 5 * Time.deltaTime);
        }
        else
        {
            this.transform.position = playerTransform.GetChild(0).transform.position;
            if (RotationInProgress)
            {
                RotationInProgress = false;
            }
        }
        this.transform.LookAt(playerTransform.position);
    }

    public static bool AllCamerasTurning()
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