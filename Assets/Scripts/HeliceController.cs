using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliceController : MonoBehaviour {
	Rigidbody _rigidbody;
	public Transform HeliceUpR;
	public Transform HeliceUpL;
	public Transform HeliceDownR;
	public Transform HeliceDownL;


	// Use this for initialization
	void Awake () {
		_rigidbody = GetComponent<Rigidbody> ();
//		Rigidbody _RigidbodyUpR = HeliceUpR.GetComponent<Rigidbody>();
//		Rigidbody _RigidbodyUpL = HeliceUpL.GetComponent<Rigidbody>();
//		Rigidbody _RigidbodyDownR = HeliceDownR.GetComponent<Rigidbody>();
//		Rigidbody _RigidbodyDownL = HeliceDownL.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}
	void AddHeliceForce(){
		
	}
}
