using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private bool playerInactive;
	private CharacterController controller;
    private Transform playerTransform;

	protected Vector3 move = Vector3.zero;
    protected Vector3 gravity = Vector3.zero;

	public float moveSpeed;
	public float fallSpeed;
	public float jumpSpeed;
	public int jumpCountMax;
    public float groundRayLength;
    private int jumpCount;
	private int jumpFrameCount;
	private bool beenGrounded;
    private Vector3 prevPosition;
    private List<int> sides;

    void Start ()
    {
		controller = this.gameObject.GetComponent<CharacterController> ();
        playerTransform = this.gameObject.transform;
        prevPosition = this.gameObject.transform.position;
        UpdateSidesToLayers();

        playerInactive = true;
        PlayerToggleActive();
    }

	void Update ()
    {
		if (!playerInactive)
        { 
            move = Input.GetAxisRaw ("Horizontal") * playerTransform.right;

			move *= moveSpeed;

            CheckEdgeCollision();

            if (IsGrounded()/* && jumpFrameCount == 0*/)
            {
                if (!beenGrounded)
                {
                    beenGrounded = true;
                }
                gravity = Vector3.zero;
                jumpCount = 0;
            }
            else
            {
                beenGrounded = false;
                gravity += -playerTransform.up * fallSpeed * Time.deltaTime;
            }

            if (jumpFrameCount > 0)
            {
                jumpFrameCount++;
                if (jumpFrameCount > 2)
                {
                    jumpFrameCount = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) /*&& jumpCount < jumpCountMax*/)
            {
                gravity = Vector3.zero;
                gravity += playerTransform.up * jumpSpeed;
                jumpCount++;
                jumpFrameCount = 1;
            }

            move += gravity;
			controller.Move (move * Time.deltaTime);
        } else {
			gravity = Vector3.zero;
		}
	}

	public void PlayerToggleActive()
    {
		playerInactive = !playerInactive;
	}

	private bool IsGrounded()
    {
        if (!Physics.GetIgnoreLayerCollision(this.gameObject.layer, sides[1]))
        {
            Ray ray = new Ray();
            ray.origin = this.gameObject.transform.position;
            ray.direction = -playerTransform.up;
            RaycastHit hit;
            if (Physics.SphereCast(ray, controller.radius, out hit, groundRayLength, 1 << 11))
            {
                if (!beenGrounded)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,
                        this.gameObject.transform.position - playerTransform.up * 3.0f,
                        Mathf.Abs(Vector3.Scale(this.gameObject.transform.position - playerTransform.up * controller.radius
                            , playerTransform.up).magnitude -
                            Vector3.Scale(hit.point, playerTransform.up).magnitude) - 0.1f);
                }
                return true;
            }
        }
        return false;

    }

    private void CheckEdgeCollision() //NEED TO RETHINK THIS!!!
    {
        //reset collission layers
        Physics.IgnoreLayerCollision(this.gameObject.layer, 10, true);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 11, true);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 12, true);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 13, true);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 14, true);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 15, true);

        Vector3 unitPlayerVelocity = Vector3.Normalize((this.gameObject.transform.position - prevPosition) / Time.deltaTime);
        prevPosition = this.gameObject.transform.position;

        Ray ray = new Ray();
        ray.direction = playerTransform.forward;

        //north east ray
        ray.origin = playerTransform.position + unitPlayerVelocity +
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) + (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if(!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(ray.origin);
        }
        //north west ray
        ray.origin = playerTransform.position + unitPlayerVelocity +
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) - (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(ray.origin);
        }
        //south east ray
        ray.origin = playerTransform.position + unitPlayerVelocity -
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) + (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(ray.origin);
        }
        //south west ray
        ray.origin = playerTransform.position + unitPlayerVelocity -
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) - (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(ray.origin);
        }
    }
    //called when cube not hit
    private void ToggleLayerCollision(Vector2 direction)
    {
        if (direction.x > (playerTransform.lossyScale.x / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[3], false);
        }
        if (direction.x < -(playerTransform.lossyScale.x / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[2], false);
        }
        if (direction.y > (playerTransform.lossyScale.y / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[0], false);
        }
        if (direction.y < -(playerTransform.lossyScale.y / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[1], false);
        }
    }
    //Should be called whenever Player changes Orientation
    private void UpdateSidesToLayers()
    {
        //might to check player isn't askew
        Vector3 pRight = playerTransform.right.normalized;
        Vector3 pUp = playerTransform.up.normalized;

        if (pRight == Vector3.right)
        {
            if(pUp == Vector3.up)
            {
                sides = new List<int>() { 10, 11, 12, 13 };
            } else if (pUp == Vector3.down)
            {
                sides = new List<int>() { 11, 10, 12, 13 };
            } else if (pUp == Vector3.forward)
            {
                sides = new List<int>() { 14, 15, 12, 13 };
            } else //back
            {
                sides = new List<int>() { 15, 14, 12, 13 };
            }
        } else if (pRight == Vector3.left)
        {
            if (pUp == Vector3.up)
            {
                sides = new List<int>() { 10, 11, 13, 12 };
            }
            else if (pUp == Vector3.down)
            {
                sides = new List<int>() { 11, 10, 13, 12 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new List<int>() { 14, 15, 13, 12 };
            }
            else //back
            {
                sides = new List<int>() { 15, 14, 13, 12 };
            }

        } else if (pRight == Vector3.up)
        {
            if (pUp == Vector3.right)
            {
                sides = new List<int>() { 13, 12, 11, 10 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new List<int>() { 12, 13, 11, 10 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new List<int>() { 14, 15, 11, 10 };
            }
            else //back
            {
                sides = new List<int>() { 15, 14, 11, 10 };
            }

        } else if (pRight == Vector3.down)
        {
            if (pUp == Vector3.right)
            {
                sides = new List<int>() { 13, 12, 10, 11 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new List<int>() { 12, 13, 10, 11 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new List<int>() { 14, 15, 10, 11 };
            }
            else //back
            {
                sides = new List<int>() { 15, 14, 10, 11 };
            }

        } else if (pRight == Vector3.forward)
        {
            if (pUp == Vector3.right)
            {
                sides = new List<int>() { 13, 12, 15, 14 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new List<int>() { 12, 13, 15, 14 };
            }
            else if (pUp == Vector3.up)
            {
                sides = new List<int>() { 10, 11, 15, 14 };
            }
            else //down
            {
                sides = new List<int>() { 11, 10, 15, 14 };
            }

        } else //back
        {
            if (pUp == Vector3.right)
            {
                sides = new List<int>() { 13, 12, 14, 15 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new List<int>() { 12, 13, 14, 15 };
            }
            else if (pUp == Vector3.up)
            {
                sides = new List<int>() { 10, 11, 14, 15 };
            }
            else //down
            {
                sides = new List<int>() { 11, 10, 14, 15 };
            }

        }
    }
}