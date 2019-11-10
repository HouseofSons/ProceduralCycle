using UnityEngine;
using System.Collections;

public class MousePointer : MonoBehaviour {

	private MainCharacter mc;

	private Tile hovered;
	private enum HoverState{HOVER, NONE};
	private HoverState hover_state = HoverState.NONE;
	private LayerMask darkPlane;
	
	void Awake() {
		mc = GameObject.FindWithTag ("Player_0" + MainCharacter.PlayerCount ()).GetComponent<MainCharacter>();
	}

	void Start()
	{
		darkPlane = 1 << 8;
	}

	void Update()
	{
		this.transform.position = Input.mousePosition;

		RaycastHit hitInfo = new RaycastHit ();
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		mc.LookAtTile (ray.GetPoint(Camera.main.transform.position.y));

		if (Physics.Raycast (ray, out hitInfo, 50, ~darkPlane) && hover_state == HoverState.NONE) {
			if (hitInfo.collider.gameObject.layer == 9) {
				hitInfo.collider.gameObject.GetComponent<Tile> ().MouseEnter ();
				hovered = hitInfo.collider.gameObject.GetComponent<Tile> ();
				hover_state = HoverState.HOVER;
			}
		}

		if (Physics.Raycast (ray, out hitInfo, 50, ~darkPlane) && hover_state == HoverState.HOVER) {
			if (hitInfo.collider.gameObject.layer == 9 && hovered != hitInfo.collider.gameObject.GetComponent<Tile>()) {
				hovered.MouseExit ();
				hitInfo.collider.gameObject.GetComponent<Tile> ().MouseEnter ();
				hovered = hitInfo.collider.gameObject.GetComponent<Tile> ();
			}
		}

		if (!Physics.Raycast (ray, out hitInfo, 50, ~darkPlane) || (Physics.Raycast (ray, out hitInfo, 50, ~darkPlane) && hitInfo.collider.gameObject.layer != 9)) {
			if (hovered != null) {
				hovered.MouseExit ();
				hovered = null;
				hover_state = HoverState.NONE; 
			}
		}

		if (hover_state == HoverState.HOVER) {
			if (hitInfo.collider != null) {
				//hitInfo.collider.gameObject.GetComponent<Tile>().MouseOver();

				if (Input.GetMouseButtonDown (0)) {
						hitInfo.collider.gameObject.GetComponent<Tile> ().MouseDown ();
				}

				//if(Input.GetMouseButtonUp(0)){
				//	hitInfo.collider.gameObject.GetComponent<Tile>().MouseUp();
				//}
			}
		}
	}
}
