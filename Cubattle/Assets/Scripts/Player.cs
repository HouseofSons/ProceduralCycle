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
        sides = new List<int>();

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

            if (IsGrounded() && jumpFrameCount == 0)
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

            ////Drop Through Ledges
            //if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
            //{
            //    if (IsGrounded())
            //    {
            //        this.gameObject.transform.position -= LevelManager.Orientation.up * 2;
            //        gravity += -LevelManager.Orientation.up * fallSpeed * Time.deltaTime;
            //    }
            //}
            //else
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

		Ray ray = new Ray ();
		ray.origin = this.gameObject.transform.position;
		ray.direction = -playerTransform.up;
		RaycastHit hit;
		if (Physics.SphereCast(ray,controller.radius,out hit, groundRayLength, 1 << 9)) {
			if(!beenGrounded) {
				this.gameObject.transform.position = Vector3.MoveTowards (this.gameObject.transform.position,
					this.gameObject.transform.position - playerTransform.up * 3.0f,
					Mathf.Abs (Vector3.Scale(this.gameObject.transform.position - playerTransform.up * controller.radius
						, playerTransform.up).magnitude -
						Vector3.Scale(hit.point, playerTransform.up).magnitude) - 0.1f);
			}
			return true;
		} else {
			return false;
		}
	}

    private void CheckEdgeCollision()
    {
        //reset collission layers
        Physics.IgnoreLayerCollision(this.gameObject.layer, 10, false);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 11, false);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 12, false);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 13, false);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 14, false);
        Physics.IgnoreLayerCollision(this.gameObject.layer, 15, false);

        Vector3 playerVelocity = (this.gameObject.transform.position - prevPosition) / Time.deltaTime;
        prevPosition = this.gameObject.transform.position;

        Ray ray = new Ray();
        ray.direction = playerTransform.forward;

        //north east ray
        ray.origin = playerTransform.position + (Vector3.Normalize(playerVelocity) + playerTransform.lossyScale) +
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) + (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if(!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2, 1 << 9))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //north west ray
        ray.origin = playerTransform.position + (Vector3.Normalize(playerVelocity) + playerTransform.lossyScale) +
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) - (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2, 1 << 9))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //south east ray
        ray.origin = playerTransform.position + (Vector3.Normalize(playerVelocity) + playerTransform.lossyScale) -
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) + (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2, 1 << 9))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //south west ray
        ray.origin = playerTransform.position + (Vector3.Normalize(playerVelocity) + playerTransform.lossyScale) -
            (playerTransform.up * (playerTransform.lossyScale.y / 2.0f)) - (playerTransform.right * (playerTransform.lossyScale.x / 2.0f)) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);
        if (!Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2, 1 << 9))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
    }
    //called when cube not hit
    private void ToggleLayerCollision(Vector2 direction)
    {
        if (direction.x > (playerTransform.lossyScale.x / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[3], true);
        }
        if (direction.x < -(playerTransform.lossyScale.x / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[2], true);
        }
        if (direction.y > (playerTransform.lossyScale.y / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[0], true);
        }
        if (direction.y < -(playerTransform.lossyScale.y / 2.0f))
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[1], true);
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