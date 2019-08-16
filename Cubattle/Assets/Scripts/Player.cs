using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public static List<Player> Players { get; private set; }
	private static bool playersActive;
	private CharacterController controller;
    public Transform PlayerTransform { get; private set; }

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
        //Players.Add(this);

        controller = this.gameObject.GetComponent<CharacterController>();
        PlayerTransform = this.gameObject.transform;
        playersActive = true;

        playerCamera = (Instantiate(Resources.Load("PlayerCamera")) as GameObject).GetComponent<PlayerCamera>();
        playerCamera.AssignPlayer(this);
    }

	void Update()
    {
        //Always looking at Camera
        this.transform.GetChild(0).transform.LookAt(2 * this.transform.position - playerCamera.transform.position);

		if (playersActive)
        { 
            move = Input.GetAxisRaw ("Horizontal") * PlayerTransform.right;
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
			controller.Move (move * Time.deltaTime);
        } else {
			gravity = Vector3.zero;
		}
	}

	public static void PlayerToggleActive()
    {
        playersActive = !playersActive;
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
                this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position,
                    this.gameObject.transform.position - PlayerTransform.up * 3.0f,
                    Mathf.Abs(Vector3.Scale(this.gameObject.transform.position - PlayerTransform.up * controller.radius
                        , PlayerTransform.up).magnitude -
                        Vector3.Scale(hit.point, PlayerTransform.up).magnitude) - 0.1f);
            }
            return true;
        }
        return false;
    }
}