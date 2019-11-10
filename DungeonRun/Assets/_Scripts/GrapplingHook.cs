using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GrapplingHook : MonoBehaviour {

	private Coroutine shootGrapplingHookCoRoutine;
	private LineRenderer rope;
	private bool grappleHitTrigger;

	void Start () {
		rope = this.gameObject.GetComponent<LineRenderer> ();
		rope.SetWidth (0.5f,0.5f);
		rope.enabled = false;
		grappleHitTrigger = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				if (GameManager.GrapplingHookReady) {
					if (Player.GrappleCharges >= 1.0f) {
						GameManager.GrapplingHookReady = false;
						Player.GrappleCharges -= 1.0f;
						UI.UpdateGrappleText (Mathf.FloorToInt(Player.GrappleCharges),Mathf.FloorToInt(Player.MaxGrappleCharges)); //weird placement
						shootGrapplingHookCoRoutine = StartCoroutine (ShootGrapplingHook ());
					}
				}
			}
		}
		if (GameManager.GrappleShotComplete && !grappleHitTrigger) {
			//Grappling hook missed everything
			GameManager.GrappleShot = false;
			GameManager.GrappleShotComplete = false;
			grappleHitTrigger = false;
			StartCoroutine(ReelInGrapplingHook());
		}
	}
	//On Trigger
	void OnTriggerEnter(Collider col) {
		if (GameManager.GrappleShot && !GameManager.GrappleShotComplete) {
			if(col.name.StartsWith("Floor")) {
				GameManager.GrappleShot = false;
				GameManager.GrappleShotComplete = false;
				grappleHitTrigger = true;
				Debug.Log ("Grappling Hook hit Floor");
				StopCoroutine(shootGrapplingHookCoRoutine);
				StartCoroutine (ReelInGrapplingHook());
			} else if(col.name.StartsWith("Ramp")) {
				GameManager.GrappleShot = false;
				GameManager.GrappleShotComplete = false;
				grappleHitTrigger = true;
				Debug.Log ("Grappling Hook hit Ramp");
				StopCoroutine(shootGrapplingHookCoRoutine);
				StartCoroutine (ReelInGrapplingHook());
			} else if(col.name.StartsWith("Wall")) {
				GameManager.GrappleShot = false;
				GameManager.GrappleShotComplete = false;
				grappleHitTrigger = true;
				Debug.Log ("Grappling Hook hit Wall");
				StopCoroutine(shootGrapplingHookCoRoutine);
				StartCoroutine (ReelInGrapplingHook());
			} else if(col.name.StartsWith("Door")) {
				GameManager.GrappleShot = false;
				GameManager.GrappleShotComplete = false;
				grappleHitTrigger = true;
				Debug.Log ("Grappling Hook hit Door");
				StopCoroutine(shootGrapplingHookCoRoutine);
				StartCoroutine (ReelInGrapplingHook());
			} else if(col.name.StartsWith("Sticky")) {
				GameManager.GrappleShot = false;
				GameManager.GrappleShotComplete = false;
				grappleHitTrigger = true;
				Debug.Log ("Grappling Hook Stuck");
				StopCoroutine(shootGrapplingHookCoRoutine);
				StartCoroutine (HookedGrapplingHook(col.gameObject.transform.position));
			}
		}
	}
	//Shoots grappling hook
	private IEnumerator ShootGrapplingHook() {
		GameManager.GrappleShot = true;
		this.gameObject.GetComponent<MeshRenderer> ().enabled = true;
		this.gameObject.transform.parent = GameObject.Find ("GameManager").transform;
		Vector3 grappleAimLocation = GameManager.MouseLocation ();
		Ray grappleRay = new Ray (GameManager.GetCurrentPlayer ().transform.position, grappleAimLocation - GameManager.GetCurrentPlayer ().transform.position);
		grappleAimLocation = grappleRay.GetPoint (35.0f);
		Vector3 rayPoint = grappleAimLocation;
		float distance = 0.0f;
		Debug.Log ("Shooting Grappling Hook");
		rope.enabled = true;

		while (Vector3.Distance(this.gameObject.transform.position,grappleAimLocation) > 0.2f) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}

			rayPoint = grappleRay.GetPoint(distance);
			this.gameObject.transform.position = new Vector3(rayPoint.x,GameManager.YPositionPlayer(this.transform.position),rayPoint.z);

			rope.SetPosition (0, GameManager.GetCurrentPlayer ().transform.position);
			rope.SetPosition (1, this.transform.position);

			distance+=2.0f;
			yield return null;
		}

		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}

		rope.enabled = false;

		GameManager.GrappleShotComplete = true;
		yield return null;
	}
	//Reel in grappling hook
	private IEnumerator ReelInGrapplingHook() {
		GameManager.GrappleNotCaught = true;
		grappleHitTrigger = false;
		float smoothing = 0.01f;
		Debug.Log ("Reeling In Grappling Hook");
		rope.enabled = true;

		while (Vector3.Distance(this.gameObject.transform.position,GameManager.GetCurrentPlayer().transform.position) > 1.0f) {
			while (GameManager.IsPaused) { //for game pause

				yield return null;
			}

			this.transform.position = Vector3.Lerp (this.transform.position, GameManager.GetCurrentPlayer ().transform.position, smoothing);

			rope.SetPosition (0, GameManager.GetCurrentPlayer ().transform.position);
			rope.SetPosition (1, this.transform.position);

			smoothing += 0.04f;
			yield return null;
		}

		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}

		rope.enabled = false;

		this.transform.position = GameManager.GetCurrentPlayer ().transform.position;
		this.transform.parent = GameManager.GetCurrentPlayer ().transform;
		this.transform.rotation = this.transform.parent.rotation;
		this.transform.Rotate (new Vector3 (270,0,0));
		this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		GameManager.GrappleNotCaughtComplete = true;
		yield return null;
	}
	//Pull to grappling hook
	private IEnumerator HookedGrapplingHook(Vector3 stickyPos) {
		GameManager.GrappleCaught = true;
		grappleHitTrigger = false;
		this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		float smoothing = 0.01f;
		Debug.Log ("Pulling to Grappling Hook");
		rope.enabled = true;

		while(Vector3.Distance(GameManager.GetCurrentPlayer().transform.position,stickyPos) > 0.2f) {

			while (GameManager.IsPaused) { //for game pause
				yield return null;
			}

			GameManager.GetCurrentPlayer().transform.position = Vector3.Lerp(GameManager.GetCurrentPlayer().transform.position,stickyPos,smoothing);
			
			rope.SetPosition (0, GameManager.GetCurrentPlayer ().transform.position);
			rope.SetPosition (1, stickyPos);

			smoothing += 0.02f;
			yield return null;
		}

		while (GameManager.IsPaused) { //for game pause
			yield return null;
		}

		rope.enabled = false;

		GameManager.GetCurrentPlayer ().transform.position = stickyPos;
		this.transform.parent = GameManager.GetCurrentPlayer().transform;
		this.transform.rotation = this.transform.parent.rotation;
		this.transform.Rotate (new Vector3 (270,0,0));
		GameManager.GrappleCaughtComplete = true;
		yield return null;
	}
}
