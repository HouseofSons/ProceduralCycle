using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Need to adjust Camera Target on Player to be dynamically distant depending on map size
public class Player : MonoBehaviour
{
    public static List<Player> Players = new List<Player>();
    private CharacterController controller;
    public Transform PlayerTransform { get; private set; }
    public Block OccupyingBlock { get; private set; }

    public PlayerCamera playerCamera;

    protected Vector3 move = Vector3.zero;
    protected Vector3 gravity = Vector3.zero;

    public float moveSpeed;
    public float fallSpeed;
    public float jumpSpeed;
    public float groundRayLength;

    private bool beenGrounded;
    private bool playerObstructed;
    public int Direction { get; private set; }

    public bool RotatingPlayerInPlace { get; set; }
    public bool RotatingPlayerInPlace_Coroutine; //Is Public for Inspector Testing

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
        if (!PausePlayerMovement && !RotatingPlayerInPlace_Coroutine)
        {
            AlignObstructedPlayer();

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
    
    private Block UpdatePlayerOccupyingBlock()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.up
        };
        if (Physics.Raycast(ray, out RaycastHit hit, LevelManager.GridSize * LevelManager.BlockSize,~(1 << 8)))
        {
            OccupyingBlock = hit.transform.parent.GetComponent<Block>();
            return OccupyingBlock;
        }
        
        return null;
    }

    private bool PlayerObstructed()
    {
        Ray ray = new Ray
        {
            origin = this.gameObject.transform.position,
            direction = -PlayerTransform.forward
        };
        if (Physics.Raycast(ray, out RaycastHit hit, LevelManager.GridSize * LevelManager.BlockSize, ~(1 << 8)))
        {
            if (!hit.transform.parent.GetComponent<Block>().Cloned)
            {
                playerObstructed = true;
                return true;
            }
        }
        ray.direction = PlayerTransform.forward;
        if (Physics.Raycast(ray, out hit, LevelManager.GridSize * LevelManager.BlockSize, ~(1 << 8)))
        {
            if (!hit.transform.parent.GetComponent<Block>().Cloned)
            {
                playerObstructed = true;
                return true;
            }
        }
        playerObstructed = false;
        return false;
    }

    private void AlignObstructedPlayer()
    {
        if (playerObstructed)
        {
            Ray ray;

            if (Direction == 1)
            {
                ray = new Ray
                {
                    origin = new Vector3(-LevelManager.BlockSize, this.transform.position.y, this.transform.position.z),
                    direction = PlayerTransform.forward
                };
            }
            else if (Direction == 3)
            {
                ray = new Ray
                {
                    origin = new Vector3(LevelManager.GridSize * LevelManager.BlockSize, this.transform.position.y, this.transform.position.z),
                    direction = PlayerTransform.forward
                };
            }
            else if (Direction == 2)
            {
                ray = new Ray
                {
                    origin = new Vector3(this.transform.position.x, this.transform.position.y, LevelManager.GridSize * LevelManager.BlockSize),
                    direction = PlayerTransform.forward
                };
            }
            else
            {
                ray = new Ray
                {
                    origin = new Vector3(this.transform.position.x, this.transform.position.y, -LevelManager.BlockSize),
                    direction = PlayerTransform.forward
                };
            }
            if (!Physics.SphereCast(ray,this.transform.localScale.x/2,LevelManager.GridSize * LevelManager.BlockSize, ~((1 << 8) | (1 << 10))))
            {
                //Aligns character to proper Face
                if (Direction == 1)
                {
                    this.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z);
                }
                else if (Direction == 3)
                {
                    this.transform.position = new Vector3((LevelManager.GridSize - 1) * LevelManager.BlockSize, this.transform.position.y, this.transform.position.z);
                }
                else if (Direction == 2)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, (LevelManager.GridSize - 1) * LevelManager.BlockSize);
                }
                else
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                }
                playerObstructed = false;
            }
        }
    }

    public IEnumerator RotatePlayer(int degrees)
    {
        RotatingPlayerInPlace = true;
        PausePlayerMovement = true;

        Vector3 faceBlockLocation;
        
        UpdatePlayerOccupyingBlock();
        
        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }
        
        if (this.OccupyingBlock.FaceBlocks[Direction] == null)
        {
            faceBlockLocation = this.OccupyingBlock.transform.position;
        }
        else
        {
            faceBlockLocation = this.OccupyingBlock.FaceBlocks[Direction].transform.position;
        }
        
        if (!playerObstructed)
        {
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

        while (RotatingPlayerInPlace)
        {
            yield return null;
        }

        UpdatePlayerOccupyingBlock();

        if (Mathf.RoundToInt(PlayerTransform.forward.x) == 1) { Direction = 1; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.z) == 1) { Direction = 0; }
        else if (Mathf.RoundToInt(PlayerTransform.forward.x) == -1) { Direction = 3; }
        else /*(Mathf.RoundToInt(PlayerTransform.forward.z) == -1)*/ { Direction = 2; }

        if (this.OccupyingBlock.FaceBlocks[Direction] == null)
        {
            faceBlockLocation = this.OccupyingBlock.transform.position;
        }
        else
        {
            faceBlockLocation = this.OccupyingBlock.FaceBlocks[Direction].transform.position;
        }

        if (!PlayerObstructed()) {
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

        PausePlayerMovement = false;
    }
}
