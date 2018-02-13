using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addf : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Rigidbody2D rb = GetComponent<Rigidbody2D> ();
			rb.AddForce (Vector2.up * 1000);
		}
	}
}
