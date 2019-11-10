using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

	public void LoadScene(int SceneIndex) {
		Application.LoadLevel (SceneIndex);
	}

	//Home Page UI Element Code BEGIN
	GameObject resetPasswordButton;
	
	void Awake () {
		if (Application.loadedLevelName == "Home") { //Only for Home Page
			resetPasswordButton = GameObject.Find ("ResetPasswordButton");
			ToggleResetPasswordButton();
		}
	}
	
	public void ToggleResetPasswordButton() {
		resetPasswordButton.SetActive (!resetPasswordButton.activeSelf);
	}
	
	public void SignIn() {
		
		string userName = GameObject.Find ("UserNameOrEmail").GetComponent<InputField> ().text;
		string password = GameObject.Find ("Password").GetComponent<InputField> ().text;
		
		Server.SignInRequest (userName,password);
	}
	
	public void ResetPassword() {
		
		string userName = GameObject.Find ("UserNameOrEmail").GetComponent<InputField> ().text;
		
		Server.ResetPasswordRequest (userName);
	}
	//Home Page UI Element Code END

	//User Create Page UI Element Code BEGIN
	public void CreateNewUser() {
		
		string userName = GameObject.Find ("UserName").GetComponent<InputField> ().text;
		string email = GameObject.Find ("Email").GetComponent<InputField> ().text;
		string password = GameObject.Find ("Password").GetComponent<InputField> ().text;
		string passwordCheck = GameObject.Find ("ReEnterPassword").GetComponent<InputField> ().text;
		
		Server.NewUserCreationRequest (userName,email,password,passwordCheck);
	}
	//User Create Page UI Element Code END
}
