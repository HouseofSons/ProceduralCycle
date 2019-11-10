using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterStatusRight : MonoBehaviour {

	private static int xRightMove;
	private static GameBoardHex RightCharHex;
	
	void Start() {
		xRightMove =  Screen.width + 510;
	}
	
	public IEnumerator RightStatusPop (GameBoardHex hex, bool pop) {
		
		RightCharHex = hex;
		
		if(hex.IsOccupied()) {
		
			SetStats(hex);
			
			if (hex.GetOccupant().IsFriendly()) {
				this.gameObject.transform.Find("RightStatusBackground").GetComponent<CanvasRenderer>().SetColor(Color.white);
			} else {
				this.gameObject.transform.Find("RightStatusBackground").GetComponent<CanvasRenderer>().SetColor(Color.gray);
			}
		}
		
		int smoothing = 2;
		
		if (pop) {
			xRightMove = Screen.width;
		} else {
			xRightMove = Screen.width + 510;
		}
		
		Vector3 pos = this.transform.position;
		
		while (Mathf.Abs(this.transform.position.x - xRightMove) > 1) {
			this.transform.position = Vector3.Lerp(this.transform.position,new Vector3(xRightMove,pos.y,pos.z),Time.deltaTime * smoothing);
			smoothing += smoothing;
			yield return null;
		}
		
		this.transform.position = new Vector3(xRightMove,pos.y,pos.z);
	}
	
	public bool RightStatusPopOn () {
		if (xRightMove == Screen.width) {
			return true;
		} else {
			return false;
		}
	}
	
	public bool RightStatusPopChar (GameBoardHex hex) {
		if (hex == RightCharHex) {
			return true;
		} else {
			return false;
		}
	}
	
	public void SetStats (GameBoardHex hex) {
		this.gameObject.transform.Find("RightCharacterName").GetComponent<Text>().text = hex.GetOccupant().GetCharacterName();
		this.gameObject.transform.Find("RightHealth").GetComponent<Text>().text = hex.GetOccupant().GetCurrentHealth() + "/" + hex.GetOccupant().GetMaxHealth();
		this.gameObject.transform.Find("RightAttack").GetComponent<Text>().text = "ATK " + hex.GetOccupant().GetAttackStat();
		this.gameObject.transform.Find("RightDefense").GetComponent<Text>().text = "DEF  " + hex.GetOccupant().GetDefenseStat();
		this.gameObject.transform.Find("RightSpeed").GetComponent<Text>().text = "SPD " + hex.GetOccupant().GetSpeed();
		this.gameObject.transform.Find("RightJump").GetComponent<Text>().text = "JMP " + hex.GetOccupant().GetJumpHeight();
		
	}
}
