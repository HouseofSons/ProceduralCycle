using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Server : MonoBehaviour {

	public static void SignInRequest(string userName,string password) {
		//request server for entry
		Debug.Log ("Sign In Attempted");
	}

	public static void ResetPasswordRequest(string userName) {
		//request server for password reset
		Debug.Log ("Reset Password Attempted");
	}
	
	public static void NewUserCreationRequest(string userName,string email,string password,string passwordCheck) {
		//request server to create user
		Debug.Log ("New User Creation Attempted");
	}
}

//GET URL REQUEST EXAMPLE
//using UnityEngine;
//
//public class GetURL : MonoBehaviour {
//	
//	void Start () {
//		string url = "http://example.com/script.php?var1=value2&amp;var2=value2";
//		WWW www = new WWW(url);
//		StartCoroutine(WaitForRequest(www));
//	}
//	
//	IEnumerator WaitForRequest(WWW www)
//	{
//		yield return www;
//		
//		// check for errors
//		if (www.error == null)
//		{
//			Debug.Log("WWW Ok!: " + www.data);
//		} else {
//			Debug.Log("WWW Error: "+ www.error);
//		}    
//	}
//}

//POST URL REQUEST EXAMPLE
//using UnityEngine;
//
//public class PostURL : MonoBehaviour {
//	
//	void Start () {
//		
//		string url = "http://example.com/script.php";
//		
//		WWWForm form = new WWWForm();
//		form.AddField("var1", "value1");
//		form.AddField("var2", "value2");
//		WWW www = new WWW(url, form);
//		
//		StartCoroutine(WaitForRequest(www));
//	}
//	
//	IEnumerator WaitForRequest(WWW www)
//	{
//		yield return www
//			
//			// check for errors
//			if (www.error == null)
//		{
//			Debug.Log("WWW Ok!: " + www.data);
//		} else {
//			Debug.Log("WWW Error: "+ www.error);
//		}    
//	}    
//}