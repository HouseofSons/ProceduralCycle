using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HexagonButton : MonoBehaviour {

	private static bool attackHexFocused;
	private static bool defendHexFocused;
	private static bool sustainHexFocused;
	
	private static int HexClick;
	
	void Start() {
		NoHexFocused ();
		HexClick = 2;
	}
	
	public void NoHexFocused () {
		attackHexFocused = false;
		defendHexFocused = false;
		sustainHexFocused = false;
	}
	
	public void AttackHexFocused () {
		attackHexFocused = true;
		defendHexFocused = false;
		sustainHexFocused = false;
	}
	
	public void DefendHexFocused () {
		attackHexFocused = false;
		defendHexFocused = true;
		sustainHexFocused = false;
	}
	
	public void SustainHexFocused () {
		attackHexFocused = false;
		defendHexFocused = false;
		sustainHexFocused = true;
	}
	
	public void ClickedAttackButtonHex() {
		HexClick = 1;
	}
	
	public void ClickedDefendButtonHex() {
		HexClick = -1;
	}
	
	public void ClickedSustainButtonHex() {
		HexClick = 0;
	}
	
	public int GetClickedHex() {
		return HexClick;
	}
	
	public bool GetAttackButtonHexFocused() {
		return attackHexFocused;
	}
	
	public bool GetDefendButtonHexFocused() {
		return defendHexFocused;
	}
	
	public bool GetSustainButtonHexFocused() {
		return sustainHexFocused;
	}

	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
}