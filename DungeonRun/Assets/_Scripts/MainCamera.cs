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
        float toSize = Mathf.Clamp(GameManager.Speed * 50f,10,35);
        this.transform.position =
            Vector3.Lerp(currPos,new Vector3(toPos.x, currPos.y, toPos.z), Time.deltaTime * 2f);

        this.GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(this.GetComponent<Camera>().orthographicSize, toSize, 0.1f);
    }
}
