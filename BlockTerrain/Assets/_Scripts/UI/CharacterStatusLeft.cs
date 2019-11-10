using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterStatusLeft : MonoBehaviour {

	private static int xLeftMove;
	private static GameBoardHex LeftCharHex;
	
	void Start() {
		xLeftMove = -510;
	}

	public IEnumerator LeftStatusPop (GameBoardHex hex, bool pop) {

		LeftCharHex = hex;

		if(hex.IsOccupied()) {
		
			SetStats(hex);
		
			if (hex.GetOccupant().IsFriendly()) {
				this.gameObject.transform.Find("LeftStatusBackground").GetComponent<CanvasRenderer>().SetColor(Color.white);
			} else {
				this.gameObject.transform.Find("LeftStatusBackground").GetComponent<CanvasRenderer>().SetColor(Color.gray);
			}
		}

		int smoothing = 2;
		
		if (pop) {
			xLeftMove = 0;
		} else {
			xLeftMove = -510;
		}
	
		Vector3 pos = this.transform.position;
		
		while (Mathf.Abs(this.transform.position.x + xLeftMove) > 1) {
			this.transform.position = Vector3.Lerp(this.transform.position,new Vector3(xLeftMove,pos.y,pos.z),Time.deltaTime * smoothing);
			smoothing += smoothing;
			yield return null;
		}
		
		this.transform.position = new Vector3(xLeftMove,pos.y,pos.z);
	}
	
	public bool LeftStatusPopOn () {
		if (xLeftMove == 0) {
			return true;
		} else {
			return false;
		}
	}
	
	public bool LeftStatusPopChar (GameBoardHex hex) {
		if (hex == LeftCharHex) {
			return true;
		} else {
			return false;
		}
	}
	
	public void SetStats (GameBoardHex hex) {
		this.gameObject.transform.Find("LeftCharacterName").GetComponent<Text>().text = hex.GetOccupant().GetCharacterName();
		this.gameObject.transform.Find("LeftHealth").GetComponent<Text>().text = hex.GetOccupant().GetCurrentHealth() + "/" + hex.GetOccupant().GetMaxHealth();
		this.gameObject.transform.Find("LeftAttack").GetComponent<Text>().text = "ATK " + hex.GetOccupant().GetAttackStat();
		this.gameObject.transform.Find("LeftDefense").GetComponent<Text>().text = "DEF  " + hex.GetOccupant().GetDefenseStat();
		this.gameObject.transform.Find("LeftSpeed").GetComponent<Text>().text = "SPD " + hex.GetOccupant().GetSpeed();
		this.gameObject.transform.Find("LeftJump").GetComponent<Text>().text = "JMP " + hex.GetOccupant().GetJumpHeight();
		
	}
}
