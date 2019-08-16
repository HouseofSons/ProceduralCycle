using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void AssignPlayer(Player p)
    {
        playerTransform = p.gameObject.transform;
    }

    private void FollowPlayer()
    {
        Vector3 focusPosition = playerTransform.position;
        int face = LevelManager.FacingCoordinate();
        Vector3 targetPosition;

        if (face == 0 || face == 3)
        {
            targetPosition = new Vector3((face == 0 ? -1 : 1) * LevelManager.GridSize * LevelManager.BlockSize * 2, focusPosition.y, focusPosition.z);
        } else if (face == 1 || face == 4)
        {
            targetPosition = new Vector3(focusPosition.x, (face == 1 ? -1 : 1) * LevelManager.GridSize * LevelManager.BlockSize * 2, focusPosition.z);
        } else /*  is Z direction)*/
        {
            targetPosition = new Vector3(focusPosition.x, focusPosition.y, (face == 2 ? -1 : 1) * LevelManager.GridSize * LevelManager.BlockSize * 2);
        }
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime);
    }
}
