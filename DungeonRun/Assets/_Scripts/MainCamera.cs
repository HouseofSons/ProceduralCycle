using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static Coroutine moveCameraCoroutine;
    public static bool CameraMoving;
    private static float ZoomSize;

    private void Start()
    {
        ZoomSize = 15;
        CameraMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.MoveCamera) {
        //    GameManager.MoveCamera = false;
        //    if (moveCameraCoroutine != null)
        //    {
        //        StopCoroutine(moveCameraCoroutine);
        //    }
        //    moveCameraCoroutine = StartCoroutine(MoveCameraToNewPosition());
        //}
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(
            GameManager.GetCurrentPlayer().transform.position.x, this.transform.position.y,
            GameManager.GetCurrentPlayer().transform.position.z),0.05f);
        this.gameObject.GetComponent<Camera>().orthographicSize = Mathf.SmoothStep(
            this.gameObject.GetComponent<Camera>().orthographicSize, ZoomSize,
            this.gameObject.GetComponent<Camera>().orthographicSize < ZoomSize ? 0.15f : 0.1f);
    }

    public static void Zoom(bool aiming)
    {
        if(aiming)
        {
            ZoomSize = 20;
        } else
        {
            ZoomSize = Mathf.Clamp(GameManager.Speed * 20,10,20);
        }
    }

    public IEnumerator MoveCameraToNewPosition(Vector3 newPos)
    {
        CameraMoving = true;
        Vector3 newCameraPosition = newPos;
        while (Vector3.Distance(transform.position, newCameraPosition) > 0.1f) {
            while (GameManager.IsPaused)
            { //for game pause
                yield return null;
            }
            transform.position = Vector3.MoveTowards(transform.position, newCameraPosition, 0.3f);
            yield return null;
        }
        while (GameManager.IsPaused)
        { //for game pause
            yield return null;
        }
        transform.position = newCameraPosition;
        CameraMoving = false;
    }
}
