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
    private int[] sides;

    GameObject corner_1;
    GameObject corner_2;
    GameObject corner_3;
    GameObject corner_4;

    void Start ()
    {
		controller = this.gameObject.GetComponent<CharacterController> ();
        playerTransform = this.gameObject.transform;
        prevPosition = this.gameObject.transform.position;
        UpdateSidesToLayers();

        playerInactive = true;
        PlayerToggleActive();

        corner_1 = GameObject.Find("1");
        corner_2 = GameObject.Find("2");
        corner_3 = GameObject.Find("3");
        corner_4 = GameObject.Find("4");

    }

	void Update ()
    {
		if (!playerInactive)
        { 
            move = Input.GetAxisRaw ("Horizontal") * playerTransform.right;

			move *= moveSpeed;

            CheckEdgeCollision();

            //if (IsGrounded() && jumpFrameCount == 0)
            //{
            //    if (!beenGrounded)
            //    {
            //        beenGrounded = true;
            //    }
            //gravity = Vector3.zero;
            //    jumpCount = 0;
            //}
            //else
            //{
            //    beenGrounded = false;
            gravity += -playerTransform.up * fallSpeed * Time.deltaTime;
            //}

            //if (jumpFrameCount > 0)
            //{
            //    jumpFrameCount++;
            //    if (jumpFrameCount > 2)
            //    {
            //        jumpFrameCount = 0;
            //    }
            //}

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

        Ray ray = new Ray();
        ray.origin = this.gameObject.transform.position;
        ray.direction = -playerTransform.up;
        RaycastHit hit;
        if (Physics.SphereCast(ray, controller.radius, out hit, groundRayLength, 1 << 9)) //needs to adjust to bottom layer when map turned
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
        else
        {
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
        ray.origin = playerTransform.position +
            (playerTransform.up * playerTransform.lossyScale.y) + (playerTransform.right * playerTransform.lossyScale.x) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);

        corner_1.transform.position = ray.origin;
        if (Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //north west ray
        ray.origin = playerTransform.position +
            (playerTransform.up * playerTransform.lossyScale.y) - (playerTransform.right * playerTransform.lossyScale.x) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);

        corner_2.transform.position = ray.origin;
        if (Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //south east ray
        ray.origin = playerTransform.position -
            (playerTransform.up * playerTransform.lossyScale.y) + (playerTransform.right * playerTransform.lossyScale.x) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);

        corner_3.transform.position = ray.origin;
        if (Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(playerTransform.position + ray.origin);
        }
        //south west ray
        ray.origin = playerTransform.position -
            (playerTransform.up * playerTransform.lossyScale.y) - (playerTransform.right * playerTransform.lossyScale.x) -
            (playerTransform.forward * (LevelManager.GridSize + 1) * LevelManager.BlockSize);

        corner_4.transform.position = ray.origin;
        if (Physics.Raycast(ray, ((LevelManager.GridSize + 1) * LevelManager.BlockSize) * 2))
        {
            ToggleLayerCollision(ray.origin - playerTransform.position);
        }
    }
    //called when cube not hit
    private void ToggleLayerCollision(Vector2 direction)
    {
        if (direction.x > 0.4f)
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[3], true);
        }
        if (direction.x < -0.4f)
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[2], true);
        }
        if (direction.y > 0.4f)
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[0], true);
        }
        if (direction.y < -0.4f)
        {
            Physics.IgnoreLayerCollision(this.gameObject.layer, sides[1], true);
        }
    }
    //Should be called whenever Player changes Orientation
    private void UpdateSidesToLayers()
    {
        //might need to check if player is askew
        Vector3 pRight = playerTransform.right.normalized;
        Vector3 pUp = playerTransform.up.normalized;

        if (pRight == Vector3.right)
        {
            if(pUp == Vector3.up)
            {
                sides = new int[] { 10, 11, 12, 13 };
            } else if (pUp == Vector3.down)
            {
                sides = new int[] { 11, 10, 12, 13 };
            } else if (pUp == Vector3.forward)
            {
                sides = new int[] { 14, 15, 12, 13 };
            } else //back
            {
                sides = new int[] { 15, 14, 12, 13 };
            }
        } else if (pRight == Vector3.left)
        {
            if (pUp == Vector3.up)
            {
                sides = new int[] { 10, 11, 13, 12 };
            }
            else if (pUp == Vector3.down)
            {
                sides = new int[] { 11, 10, 13, 12 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new int[] { 14, 15, 13, 12 };
            }
            else //back
            {
                sides = new int[] { 15, 14, 13, 12 };
            }

        } else if (pRight == Vector3.up)
        {
            if (pUp == Vector3.right)
            {
                sides = new int[] { 13, 12, 11, 10 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new int[] { 12, 13, 11, 10 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new int[] { 14, 15, 11, 10 };
            }
            else //back
            {
                sides = new int[] { 15, 14, 11, 10 };
            }

        } else if (pRight == Vector3.down)
        {
            if (pUp == Vector3.right)
            {
                sides = new int[] { 13, 12, 10, 11 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new int[] { 12, 13, 10, 11 };
            }
            else if (pUp == Vector3.forward)
            {
                sides = new int[] { 14, 15, 10, 11 };
            }
            else //back
            {
                sides = new int[] { 15, 14, 10, 11 };
            }

        } else if (pRight == Vector3.forward)
        {
            if (pUp == Vector3.right)
            {
                sides = new int[] { 13, 12, 15, 14 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new int[] { 12, 13, 15, 14 };
            }
            else if (pUp == Vector3.up)
            {
                sides = new int[] { 10, 11, 15, 14 };
            }
            else //down
            {
                sides = new int[] { 11, 10, 15, 14 };
            }

        } else //back
        {
            if (pUp == Vector3.right)
            {
                sides = new int[] { 13, 12, 14, 15 };
            }
            else if (pUp == Vector3.left)
            {
                sides = new int[] { 12, 13, 14, 15 };
            }
            else if (pUp == Vector3.up)
            {
                sides = new int[] { 10, 11, 14, 15 };
            }
            else //down
            {
                sides = new int[] { 11, 10, 14, 15 };
            }

        }
    }
}