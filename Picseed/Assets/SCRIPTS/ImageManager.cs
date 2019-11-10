using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour {

	public Sprite texPic;
	[Range(-1.0f,1.0f )]
	public float mipMapBias;

	private GameObject PicPlane;
	private int picPixelHeight;
	private int picPixelWidth;

	// Use this for initialization
	void Start () {
		PicPlane = GameObject.Find ("PicPlane");
		PicPlane.GetComponent<SpriteRenderer> ().sprite = texPic;
		picPixelHeight = PicPlane.GetComponent<SpriteRenderer> ().sprite.texture.height;
		picPixelWidth = PicPlane.GetComponent<SpriteRenderer> ().sprite.texture.width;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
