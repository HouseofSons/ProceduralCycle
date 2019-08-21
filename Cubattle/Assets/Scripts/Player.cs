using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    public static List<Player> Players = new List<Player>();
    private CharacterController controller;
    public Transform PlayerTransform { get; private set; }
    public Block OccupiedBlock { get; private set; }

    public PlayerCamera playerCamera;

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

    private void Awake()
    {
        Players.Add(this);

        controller = this.gameObject.GetComponent<CharacterController>();
        PlayerTransform = this.gameObject.transform;

        playerCamera = (Instantiate(Resources.Load("PlayerCamera")) as GameObject).GetComponent<PlayerCamera>();
        playerCamera.ConfigurePlayerCameraTarget(this.gameObject.transform.GetChild(0).transform);
    }

    void Update()
    {
        if (!LevelManager.PlayerFreeze)
        {
            move = Input.GetAxisRaw("Horizontal") * PlayerTransform.right;
            move *= moveSpeed;
            
            if (IsGrounded() /*&& jumpFrameCount == 0*/)
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
                gravity += -PlayerTransform.up * fallSpeed * Time.deltaTime;
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
                gravity += PlayerTransform.up * jumpSpeed;
                jumpCount++;
                jumpFrameCount = 1;
            }
           
            move += gravity;
            controller.Move(move * Time.deltaTime);
            controller.transform.position = AlignPositionToFace(controller.transform.position);
        }
        else
        {
            gravity = Vector3.zero;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.up
        };
        if (Physics.SphereCast(ray, controller.radius, out RaycastHit hit, groundRayLength))
        {
            if (!beenGrounded)
            {
                this.gameObject.transform.position =
                    Vector3.MoveTowards(this.gameObject.transform.position,
                                        this.gameObject.transform.position - PlayerTransform.up * 3.0f,
                                        Mathf.Abs(Vector3.Scale(this.gameObject.transform.position - PlayerTransform.up * controller.radius,
                                                                PlayerTransform.up).magnitude - Vector3.Scale(hit.point,
                                                                PlayerTransform.up).magnitude) - 0.1f);
            }
            OccupiedBlock = hit.transform.parent.transform.parent.GetComponent<Block>();
            return true;
        }
        return false;
    }

    private Vector3 AlignPositionToFace(Vector3 pos)
    {
        switch (LevelManager.FacingCoordinate)
        {
            case 0:
                return new Vector3(0, pos.y, pos.z);
            case 1:
                return new Vector3(pos.x, 0, pos.z);
            case 2:
                return new Vector3(pos.x, pos.y, 0);
            case 3:
                return new Vector3((LevelManager.GridSize - 1) * LevelManager.BlockSize, pos.y, pos.z);
            case 4:
                return new Vector3(pos.x, (LevelManager.GridSize - 1) * LevelManager.BlockSize, pos.z);
            default:
                return new Vector3(pos.x, pos.y, (LevelManager.GridSize - 1) * LevelManager.BlockSize);
        }
    }
    
    public static void MovePlayersTo3DPosition(Vector3 axis)
    {
        //Only considers rotations around the Y axis
        foreach(Player p in Players)
        {
            switch (LevelManager.FacingCoordinate)
            {
                case 0:
                    if (axis == Vector3.up)
                    {
                        p.transform.position = new Vector3(p.transform.position.z, p.transform.position.y, p.transform.position.z);
                    }
                    else if (axis == Vector3.down)
                    {
                        p.transform.position = new Vector3((LevelManager.GridSize - 1) * LevelManager.BlockSize - p.transform.position.z, p.transform.position.y, p.transform.position.z);
                    }
                    else
                    {
                        Debug.Log("Error when Moving Player: " + p);
                    }

                    break;
                case 2:
                    if (axis == Vector3.up)
                    {
                        p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, (LevelManager.GridSize - 1) * LevelManager.BlockSize - p.transform.position.x);
                    }
                    else if (axis == Vector3.down)
                    {
                        p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.x);
                    }
                    else
                    {
                        Debug.Log("Error when Moving Player: " + p);
                    }

                    break;
                case 3:
                    if (axis == Vector3.up)
                    {
                        p.transform.position = new Vector3(p.transform.position.z, p.transform.position.y, p.transform.position.z);
                    }
                    else if (axis == Vector3.down)
                    {
                        p.transform.position = new Vector3((LevelManager.GridSize - 1) * LevelManager.BlockSize - p.transform.position.z, p.transform.position.y, p.transform.position.z);
                    }
                    else
                    {
                        Debug.Log("Error when Moving Player: " + p);
                    }

                    break;
                case 5:
                    if (axis == Vector3.up)
                    {
                        p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, (LevelManager.GridSize - 1) * LevelManager.BlockSize - p.transform.position.x);
                    }
                    else if (axis == Vector3.down)
                    {
                        p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.x);
                    }
                    else
                    {
                        Debug.Log("Error when Moving Player: " + p);
                    }

                    break;
                default:
                    Debug.Log("Bad Coordinate");
                    break;
            }
        }
    }
}