using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static Coroutine moveCameraCoroutine;
    public static bool CameraMoving;

    private void Start()
    {
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
            GameManager.GetCurrentPlayer().transform.position.z),0.01f);
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
