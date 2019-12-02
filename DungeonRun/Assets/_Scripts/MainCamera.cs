using System.Collections;
using System.Collections.Generic;
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
        if (GameManager.MoveCamera) {
            GameManager.MoveCamera = false;
            if (moveCameraCoroutine != null)
            {
                StopCoroutine(moveCameraCoroutine);
            }
            moveCameraCoroutine = StartCoroutine(MoveCameraToNewPosition());
        }
    }

    public IEnumerator MoveCameraToNewPosition()
    {
        CameraMoving = true;
        Vector3 newCameraPosition = GameManager.GetCurrentLevel().transform.Find("CameraPosition").gameObject.transform.position;
        while (Vector3.Distance(this.transform.position, newCameraPosition) > 0.1f) {
            while (GameManager.IsPaused)
            { //for game pause
                yield return null;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, newCameraPosition, 0.3f);
            yield return null;
        }
        while (GameManager.IsPaused)
        { //for game pause
            yield return null;
        }
        this.transform.position = newCameraPosition;
        CameraMoving = false;
    }
}
