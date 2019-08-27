using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static List<Player> Players = new List<Player>();
    private CharacterController controller;
    public Transform PlayerTransform { get; private set; }
    public Block LocalBlock { get; private set; }

    public PlayerCamera playerCamera;

    protected Vector3 move = Vector3.zero;
    protected Vector3 gravity = Vector3.zero;

    public float moveSpeed;
    public float fallSpeed;
    public float jumpSpeed;
    public float groundRayLength;

    private bool beenGrounded;
    public int Direction { get; private set; }

    public bool RotatingCamera;
    public bool RotatingCamera_Coroutine; //Is Public for Inspector Testing

    private void Awake()
    {
        Players.Add(this);

        controller = this.gameObject.GetComponent<CharacterController>();
        PlayerTransform = this.gameObject.transform;

        playerCamera = (Instantiate(Resources.Load("PlayerCamera")) as GameObject).GetComponent<PlayerCamera>();
        playerCamera.AssignPlayer(this);
    }

    void Update()
    {
        if (!LevelManager.PausePlayerMovement)
        {
            move = Input.GetAxisRaw("Horizontal") * PlayerTransform.right;
            move *= moveSpeed;

            if (IsGrounded())
            {
                if (!beenGrounded)
                {
                    beenGrounded = true;
                }
                gravity = Vector3.zero;
            }
            else
            {
                beenGrounded = false;
                gravity += -PlayerTransform.up * fallSpeed * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravity = Vector3.zero;
                gravity += PlayerTransform.up * jumpSpeed;
            }

            move += gravity;
            controller.Move(move * Time.deltaTime);
            UpdatePlayerLocalBlock();
        }
        else
        {
            gravity = Vector3.zero;
        }

        if (RotatingCamera_Coroutine)
        {
            RotatingCamera_Coroutine = false;
            StartCoroutine(RotatePlayer(Vector3.up, 90));
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
            return true;
        }
        return false;
    }
    
    private void UpdatePlayerLocalBlock()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.up
        };
        if (Physics.Raycast(ray, out RaycastHit hit, LevelManager.GridSize * LevelManager.BlockSize,~(1 << 8)))
        {
            LocalBlock = hit.transform.parent.GetComponent<InsideBlock>();
        }
    }

    public IEnumerator RotatePlayer(Vector3 axis, int degrees)
    {
        RotatingCamera = true;

        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

        //Aligns character to center depth axis on Parented Map Block
        if (Direction == 1 || Direction == 3)
        {
            this.transform.position = new Vector3(LocalBlock.transform.position.x, this.transform.position.y, this.transform.position.z);
        } else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, LocalBlock.transform.position.z);
        }

        while(RotatingCamera)
        {
            yield return null;
        }

        this.transform.Rotate(axis, degrees);

        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

        Vector3 faceBlockLocation;

        if (this.LocalBlock.FaceBlocks[Direction] == null)
        {
            faceBlockLocation = this.LocalBlock.transform.position;
        }
        else
        {
            faceBlockLocation = this.LocalBlock.FaceBlocks[Direction].transform.position;
        }
        Debug.Log(faceBlockLocation);
        //Aligns character to faceBlock
        if (Direction == 1 || Direction == 3)
        {
            this.transform.position = new Vector3(faceBlockLocation.x, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, faceBlockLocation.z);
        }


    }
}