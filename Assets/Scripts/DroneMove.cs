using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMove : MonoBehaviour {
	Rigidbody _rigidbody;
	bool isFlying = false;

	//Forice
	public float upForce = 0f;
	public float droneWeight = 10f;
	public Transform[] propeller;
	const float g0 = 9.81f;

	//Speed
	float MoveSpeed = 1000f;
	float tiltAmountForward = 0f;
	float tiltAmountSwerve = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;

	//Rotation
	float YRotation = 0f;
	float currentYRotation = 0f;
	float rotationAmount = 2.5f;
	float rotationYVelocity = 0f;

	// Use this for initialization
	void Awake () {
		_rigidbody = GetComponent<Rigidbody>();
		Physics.gravity = new Vector3(0, -g0, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey (KeyCode.Z)) {
			isFlying = false;
		}
		MovemenForward ();
		Swerve ();
		PropellerForce ();
		Rotation ();
//		_rigidbody.AddRelativeForce (Vector3.up * upForce);
		for (int i = 0; i < propeller.Length; i++) {
			_rigidbody.AddForceAtPosition (Vector3.up * upForce, propeller[i].TransformPoint(propeller[i].position));
		}
		_rigidbody.rotation = Quaternion.Euler (new Vector3(tiltAmountForward, currentYRotation, tiltAmountSwerve));
		//_rigidbody.AddRelativeForce(Vector3.up * upForce/1000);
	}
	void PropellerForce(){
		
		if (Input.GetKey (KeyCode.I)) {
			isFlying = true;
			upForce = 400f;

		} else if (Input.GetKey (KeyCode.K)) {
			upForce = -200f;
		} else if ((!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K)) ) {
			upForce = 9.81f;
		} 
	}
	void MovemenForward(){
		if (Input.GetAxis ("Vertical") != 0) {
			_rigidbody.AddRelativeForce (Vector3.forward * MoveSpeed * Input.GetAxis ("Vertical"));
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, 20 * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.1f);
		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			_rigidbody.AddRelativeForce (Vector3.right * MoveSpeed * Input.GetAxis ("Horizontal"));
			tiltAmountSwerve = Mathf.SmoothDamp (tiltAmountSwerve, -20 * Input.GetAxis ("Horizontal"), ref tiltVelocitySwerve, 0.1f);
		}
	}
	void Rotation(){
		if (Input.GetKey (KeyCode.J)) {
			YRotation -= rotationAmount;
		}
		if (Input.GetKey (KeyCode.L)) {
			YRotation += rotationAmount;
		}
		currentYRotation = Mathf.SmoothDamp (currentYRotation, YRotation, ref rotationYVelocity, 0.25f);

	}
}
