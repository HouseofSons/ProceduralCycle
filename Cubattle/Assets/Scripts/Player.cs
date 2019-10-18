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

    public bool RotatingPlayerInPlace { get; set; }
    public bool RotatingPlayerInPlace_Coroutine; //Is Public for Inspector Testing

    public bool RotatingMap { get; set; }
    public bool RotatingMap_Coroutine { get; set; }

    public bool PausePlayerMovement { get; set; }

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
        if (!PausePlayerMovement && !RotatingPlayerInPlace_Coroutine && !RotatingMap_Coroutine)
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
            controller.enabled = true;

            move += gravity;
            controller.Move(move * Time.deltaTime);

            controller.enabled = false;
        }
        else
        {
            gravity = Vector3.zero;
            if (RotatingPlayerInPlace_Coroutine)
            {
                RotatingPlayerInPlace_Coroutine = false;
                StartCoroutine(RotatePlayer(90));
            }
            if (RotatingMap_Coroutine)
            {
                RotatingMap_Coroutine = false;
                StartCoroutine(RotateMap(LevelManager.AxisPosition,90));
            }
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.up
        };
        if (Physics.SphereCast(ray, controller.radius, out RaycastHit hit, groundRayLength, ~(1 << 9)))
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
    
    private Block UpdatePlayerLocalBlock()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.up
        };
        if (Physics.Raycast(ray, out RaycastHit hit, LevelManager.GridSize * LevelManager.BlockSize,~(1 << 8)))
        {
            LocalBlock = hit.transform.parent.GetComponent<InsideBlock>();
            return LocalBlock;
        }
        return null;
    }

    public IEnumerator RotatePlayer(int degrees)
    {
        RotatingPlayerInPlace = true;
        PausePlayerMovement = true;

        Vector3 faceBlockLocation;

        UpdatePlayerLocalBlock();

        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

        if (this.LocalBlock.FaceBlocks[Direction] == null)
        {
            faceBlockLocation = this.LocalBlock.transform.position;
        }
        else
        {
            faceBlockLocation = this.LocalBlock.FaceBlocks[Direction].transform.position;
        }
        
        //Aligns character to InnerBlock
        if (Direction == 1 || Direction == 3)
        {
            this.transform.position = new Vector3(faceBlockLocation.x, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, faceBlockLocation.z);
        }

        this.transform.Rotate(Vector3.up, degrees);

        while (RotatingPlayerInPlace)
        {
            yield return null;
        }

        UpdatePlayerLocalBlock();

        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

        if (this.LocalBlock.FaceBlocks[Direction] == null)
        {
            faceBlockLocation = this.LocalBlock.transform.position;
        }
        else
        {
            faceBlockLocation = this.LocalBlock.FaceBlocks[Direction].transform.position;
        }
        
        //Aligns character to faceBlock
        if (Direction == 1 || Direction == 3)
        {
            this.transform.position = new Vector3(faceBlockLocation.x, this.transform.position.y, this.transform.position.z);
        }
        else
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, faceBlockLocation.z);
        }

        PausePlayerMovement = false;
    }

    public IEnumerator RotateMap(Vector3 center, int degrees)
    {
        RotatingMap = true;
        PausePlayerMovement = true;

        bool landedOnBlock = false;
        Vector3 currPos = this.transform.position;

        Vector3 faceBlockLocation;

        if (degrees == 90 || degrees == -270)
        {
            this.transform.position = new Vector3(-(currPos.z - center.z) + center.x,currPos.y,(currPos.x - center.x) + center.z);
        } else if (degrees == 180 || degrees == -180)
        {
            this.transform.position = new Vector3(-(currPos.x - center.x) + center.x,currPos.y,-(currPos.z - center.z) + center.z);
        } else if (degrees == 270 || degrees == -90)
        {
            this.transform.position = new Vector3(currPos.z - center.z + center.x,currPos.y,-(currPos.x - center.x) + center.z);
        } else
        {
            Debug.Log("bad degrees entry of: " + degrees);
        }

        if (UpdatePlayerLocalBlock() != null)
        {
            landedOnBlock = true;

            if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
            else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
            else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
            else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

            if (this.LocalBlock.FaceBlocks[Direction] == null)
            {
                faceBlockLocation = this.LocalBlock.transform.position;
            }
            else
            {
                faceBlockLocation = this.LocalBlock.FaceBlocks[Direction].transform.position;
            }

            //Aligns character to InnerBlock
            if (Direction == 1 || Direction == 3)
            {
                this.transform.position = new Vector3(faceBlockLocation.x, this.transform.position.y, this.transform.position.z);
            }
            else
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, faceBlockLocation.z);
            }
        } 

        this.transform.Rotate(Vector3.up, degrees);

        if (landedOnBlock) {

            UpdatePlayerLocalBlock();

            if (this.LocalBlock.FaceBlocks[Direction] == null)
            {
                faceBlockLocation = this.LocalBlock.transform.position;
            }
            else
            {
                faceBlockLocation = this.LocalBlock.FaceBlocks[Direction].transform.position;
            }

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

        while (RotatingMap)
        {
            yield return null;
        }

        PausePlayerMovement = false;
    }
}
