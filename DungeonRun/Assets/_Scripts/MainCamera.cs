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
    void Update()
    {
        SmoothFollow();
    }

    private void SmoothFollow()
    {
        Vector3 toPos = GameManager.GetCurrentPlayer().transform.position;
        Vector3 currPos = this.transform.position;
        float toFOV = Mathf.Clamp(GameManager.Speed * 200.0f,45,100);
        this.transform.position =
            Vector3.Lerp(currPos,new Vector3(toPos.x, currPos.y, toPos.z), Time.deltaTime * 1f);

        this.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(this.GetComponent<Camera>().fieldOfView,toFOV,0.1f);
    }
}
