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
    }

    private void SmoothFollow()
    {
        Vector3 currPos = this.transform.position;
        Vector3 toPos = GameManager.GetCurrentPlayer().transform.position;

        this.transform.position =
            Vector3.Lerp(currPos,new Vector3(toPos.x, currPos.y, toPos.z), Time.deltaTime * 2.0f);

        float toSize = (((Player.Speed - GameManager.SpeedMin) / (GameManager.SpeedMax - GameManager.SpeedMin)) *
            (GameManager.CameraSizeMax - GameManager.CameraSizeMin)) + GameManager.CameraSizeMin;
        this.GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(this.GetComponent<Camera>().orthographicSize, toSize, 0.2f);
    }
}
