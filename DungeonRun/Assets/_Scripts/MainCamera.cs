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
        //SmoothPan();
    }

    private void SmoothFollow()
    {
        Vector3 currPos = this.transform.position;
        Vector3 playerPos = GameManager.CurrentPlayer.transform.position;
        Vector3 partPos;

        Partition p = Player.CurrentRoom.GetPartition(Player.PlayerDestination);
        if(p != null)
        {
            partPos = p.transform.position;
        } else
        {
            partPos = Player.LatestSpawn.GetComponent<Spawn>().GetPartition().transform.position;
        }

        Vector3 toPos = new Vector3((playerPos.x + partPos.x)/2, currPos.y, (playerPos.z + partPos.z)/2);

        this.transform.position =
            Vector3.Lerp(currPos,toPos, Time.deltaTime * 2.0f);
    }

    private void SmoothPan()
    {
        float toSize = (((Player.Speed - GameManager.SpeedMin) / (GameManager.SpeedMax - GameManager.SpeedMin)) *
            (GameManager.CameraSizeMax - GameManager.CameraSizeMin)) + GameManager.CameraSizeMin;
        this.GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(this.GetComponent<Camera>().orthographicSize, toSize, 0.1f);
    }
}
