using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private bool playerInactive;
	private CharacterController controller;

	protected Vector3 move = Vector3.zero;
	protected Vector3 gravity = Vector3.zero;

	public float moveSpeed;
	public float fallSpeed;
	public float jumpSpeed;
	public int jumpCountMax;
	private int jumpCount;
	private int jumpFrameCount = 0;
	private bool beenGrounded = false;

	void Start () {

		controller = this.gameObject.GetComponent<CharacterController> ();
		playerInactive = true;
		PlayerToggleActive ();
	}

	void Update () {

		if (!playerInactive) {

			move = Input.GetAxisRaw ("Horizontal") * LevelManager.walls [LevelManager.currentWallNumber].transform.right;

			move *= moveSpeed;

			if (IsGrounded () && jumpFrameCount == 0) {
				if(!beenGrounded) {
					beenGrounded = true;
				}
				gravity = Vector3.zero;
				jumpCount = 0;
			} else {
				beenGrounded = false;
				gravity += -LevelManager.walls [LevelManager.currentWallNumber].transform.up * fallSpeed * Time.deltaTime;
			}

			if (jumpFrameCount > 0) {
				jumpFrameCount++;
				if (jumpFrameCount > 2) {
					jumpFrameCount = 0;
				}
			}

			if (Input.GetKey (KeyCode.S) && Input.GetKeyDown (KeyCode.Space)) {
				if(IsGrounded ()){//Drop Through
					this.gameObject.transform.position -= LevelManager.walls [LevelManager.currentWallNumber].transform.up * 2;
					gravity += -LevelManager.walls [LevelManager.currentWallNumber].transform.up * fallSpeed * Time.deltaTime;
				}
			} else if (Input.GetKeyDown (KeyCode.Space) && jumpCount < jumpCountMax) {
				gravity = Vector3.zero;
				gravity += LevelManager.walls [LevelManager.currentWallNumber].transform.up * jumpSpeed;
				jumpCount++;
				jumpFrameCount = 1;
			}

			move += gravity;
			controller.Move (move * Time.deltaTime);
		} else {
			gravity = Vector3.zero;
		}
	}

	public void PlayerToggleActive() {
		playerInactive = !playerInactive;
	}

	private bool IsGrounded() {

		Ray ray = new Ray ();
		ray.origin = this.gameObject.transform.position;
		ray.direction = -LevelManager.walls [LevelManager.currentWallNumber].transform.up;
		RaycastHit hit;
		if (Physics.SphereCast(ray,controller.radius,out hit,0.5f,1 << 10)) {
			if(!beenGrounded) {
				this.gameObject.transform.position = Vector3.MoveTowards (this.gameObject.transform.position,
					this.gameObject.transform.position - LevelManager.walls [LevelManager.currentWallNumber].transform.up * 3.0f,
					Mathf.Abs (Vector3.Scale(this.gameObject.transform.position - LevelManager.walls [LevelManager.currentWallNumber].transform.up * controller.radius
						,LevelManager.walls [LevelManager.currentWallNumber].transform.up).magnitude -
						Vector3.Scale(hit.point,LevelManager.walls [LevelManager.currentWallNumber].transform.up).magnitude) - 0.1f);
			}
			return true;
		} else {
			return false;
		}
	}
}