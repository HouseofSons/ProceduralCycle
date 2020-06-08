using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static bool CameraMoving;

    private void Start()
    {
        CameraMoving = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SmoothFollow();
        SmoothPan();
    }

    private void SmoothFollow()
    {
        Vector3 currPos = this.transform.position;
        Vector3 playerPos = GameManager.GetCurrentPlayer().transform.position;
        //Vector3 playerLookPos = new Vector3(
        //                        AimArrow.Arrow.transform.up.x * (10 * Player.Speed) + transform.position.x,
        //                        transform.position.y,
        //                        AimArrow.Arrow.transform.up.z * (10 * Player.Speed) + transform.position.z);
        Vector3 nextPartitionPos = Player.NextPartition.transform.position;
        Vector3 currPartPos = GameManager.GetCurrentPlayer().CurrentRoom.GetPartition(playerPos).transform.position;
        Vector3 toPos = new Vector3((nextPartitionPos.x + currPartPos.x)/2, currPos.y, (nextPartitionPos.z + currPartPos.z)/2);

        this.transform.position =
            Vector3.Lerp(currPos,new Vector3(toPos.x, currPos.y, toPos.z), Time.deltaTime * 2.0f);
    }

    private void SmoothPan()
    {
        float toSize = (((Player.Speed - GameManager.SpeedMin) / (GameManager.SpeedMax - GameManager.SpeedMin)) *
            (GameManager.CameraSizeMax - GameManager.CameraSizeMin)) + GameManager.CameraSizeMin;
        this.GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(this.GetComponent<Camera>().orthographicSize, toSize, 0.1f);
    }
}
