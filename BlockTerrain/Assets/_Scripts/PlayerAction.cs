using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour {

	public void PlayerChoseMove() {
		GameBoardHex anyHex = FindObjectOfType<GameBoardHex>();
		if (GameCamera.ThirdPersonView) {
			if (GameCamera.focusHex != GameBoardHex.GetCTCHex ().transform.position) {
				GameObject.Find ("DialogueBoxMove").transform.position = new Vector3 (0, Screen.height + 100, 0);
				Pathing.AttackMoveGrid (GameBoardHex.GetCTCHex (), true, GameBoardHex.GetCTCHex ().GetOccupant ().GetMoved ());
				StartCoroutine (anyHex.Move (GameBoardHex.GetFocusHex ()));
				GameBoardHex.GameBoardEnabled = true;
			}
		} else {
			GameObject.Find ("DialogueBoxMove").transform.position = new Vector3 (0, Screen.height + 100, 0);
			Pathing.AttackMoveGrid (GameBoardHex.GetCTCHex (), true, GameBoardHex.GetCTCHex ().GetOccupant ().GetMoved ());
			StartCoroutine (anyHex.Move (GameBoardHex.GetFocusHex ()));
			GameBoardHex.GameBoardEnabled = true;
		}
	}
	
	public void PlayerChoseStay() {
		GameObject.Find("DialogueBoxMove").transform.position = new Vector3(0,Screen.height+100,0);
		if (GameBoardHex.GetFocusHex() != null) {
            GameBoardHex.GetFocusHex().changed = true;
		}
        GameBoardHex.SetFocusHex(GameBoardHex.GetCTCHex());
		GameBoardHex.GameBoardEnabled = true;
	}
	
	public void PlayerChoseAttack() {
		GameObject.Find("DialogueFight").transform.position = new Vector3(Screen.width/2,Screen.height/3,0);
		GameObject.Find("DialogueBoxAttack").transform.position = new Vector3(0,Screen.height+100,0);
		
		Color c = GameObject.Find("HUDCanvas").GetComponent<Image>().color;
		c.a = 0.75f;
		GameObject.Find("HUDCanvas").GetComponent<Image>().color = c;
		GameObject.Find("HUDCanvas").GetComponent<Image>().enabled = true;
		DisableEndTurnOption();
		
		GameBoardHex.GameBoardEnabled = false;
		Time.timeScale = 0;
	}
	
	public void PlayerChoseSustain() {
		GameObject.Find("DialogueBoxAttack").transform.position = new Vector3(0,Screen.height+100,0);
		if (GameBoardHex.GetFocusHex() != null) {
            GameBoardHex.GetFocusHex().changed = true;
		}
        GameBoardHex.SetFocusHex(GameBoardHex.GetCTCHex());
		GameBoardHex.GameBoardEnabled = true;
		if (Character.characterTargetHexLock != null) {
			StartCoroutine (FindObjectOfType<CharacterStatusRight>().RightStatusPop(Character.characterTargetHexLock,false));
		}
		Character.characterTargetHexLock = null;
	}
	
	public void DisableEndTurnOption() {
		Color c = GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("BackgroundText").GetComponent<Text>().color;
		c.a = 0;
		Color d = GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("ForegroundText").GetComponent<Text>().color;
		d.a = 0;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").GetComponent<Button>().interactable = false;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").GetComponent<Image>().enabled = false;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("BackgroundText").GetComponent<Text>().color = c;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("ForegroundText").GetComponent<Text>().color = d;
	}
	
	public void EnableEndTurnOption() {
		Color c = GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("BackgroundText").GetComponent<Text>().color;
		c.a = 1;
		Color d = GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("ForegroundText").GetComponent<Text>().color;
		d.a = 1;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").GetComponent<Button>().interactable = true;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").GetComponent<Image>().enabled = true;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("BackgroundText").GetComponent<Text>().color = c;
		GameObject.Find("CharacterStatusLeft").transform.Find("EndTurn").Find ("ForegroundText").GetComponent<Text>().color = d;
	}
	
	public void PlayerChoseEndTurnOption() {
		PlayerChoseSustain ();
		PlayerChoseStay ();
		GameObject.Find("DialogueBoxEndTurn").transform.position = new Vector3(Screen.width * 0.3f,Screen.height * 0.7f,0);
		GameBoardHex.GameBoardEnabled = false;
	}
	
	public void PlayerChoseEndTurn() {
        if (GameCamera.DoneMoving)
        {
            GameObject.Find("DialogueBoxEndTurn").transform.position = new Vector3(0, Screen.height + 100, 0);
            //StartCoroutine (FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.GetCTCHex(),false));
            Pathing.AttackMoveGrid(GameBoardHex.GetCTCHex(), false, GameBoardHex.GetCTCHex().GetOccupant().GetMoved());
            Character character = GameBoardHex.GetCTCHex().GetOccupant();
            GameBoardHex.GetCTCHex().changed = true;
            character.CharacterTurnOver();
            character.NowAttackNeighbors();
            Pathing.CharacterPaths();
            Pathing.AttackMoveGrid(GameBoardHex.GetCTCHex(), true, GameBoardHex.GetCTCHex().GetOccupant().GetMoved());
            StartCoroutine(FindObjectOfType<CharacterStatusLeft>().LeftStatusPop(GameBoardHex.ctcHex, true));
            StartCoroutine(GameCamera.SetCameraToObject(GameBoardHex.GetCTCHex().gameObject.transform.position));
            GameBoardHex.SetFocusHex(GameBoardHex.ctcHex);
            GameBoardHex.GameBoardEnabled = true;
        }
	}
	
	public void PlayerChoseNotToEndTurn() {
		GameObject.Find("DialogueBoxEndTurn").transform.position = new Vector3(0,Screen.height+100,0);
		GameBoardHex.GameBoardEnabled = true;
	}
}
