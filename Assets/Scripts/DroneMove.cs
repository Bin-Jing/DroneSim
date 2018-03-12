using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroneMove : MonoBehaviour {
	ConstValue _constv;
	Rigidbody _rigidbody;
	bool isFlying = false;

	Transform transform;
	public Transform HeliceUpR;
	public Transform HeliceUpL;
	public Transform HeliceDownR;
	public Transform HeliceDownL;


	//Forice
	public float upForce = 0f;
	float droneWeight = 1f;

	float _gravity = 9.81f;

	//Speed
	float MoveSpeed = 10f;
	float tiltAmountForward = 0f;
	float tiltAmountSwerve = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;

	//Rotation
	float YRotation = 0f;
	public float currentYRotation = 0f;
	float rotationAmount = 2.5f;
	float rotationYVelocity = 0f;

	// Use this for initialization
	void Awake () {
		_constv = GetComponent<ConstValue> ();
		_rigidbody = GetComponent<Rigidbody>();
		transform = GetComponent<Transform> ();
		Physics.gravity = new Vector3(0, -_constv.GetGravity() * droneWeight, 0);
		_gravity = _constv.GetGravity ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		updateEnvironmentForce ();

		if (Input.GetKey (KeyCode.Z)) {
			isFlying = false;
		}
		MovemenForward ();
		Swerve ();
		PropellerForce ();
		Rotation ();


		AddHeliceForce ();
		_rigidbody.AddRelativeForce (Vector3.up * upForce);


		_rigidbody.rotation = Quaternion.Euler (new Vector3(tiltAmountForward, currentYRotation, tiltAmountSwerve));
	}
	void PropellerForce(){
		if ((Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f) || (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
			if (Input.GetKey (KeyCode.I) || Input.GetKey (KeyCode.K)) {
				_rigidbody.velocity = _rigidbody.velocity;
			}
			if (!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K) && !Input.GetKey (KeyCode.J) && !Input.GetKey (KeyCode.L)) {
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
				upForce = 28.1f;
			}
			if (!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K) && (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L))) {
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
				upForce = 11f;
			}
			if (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L)) {
				upForce = 41f;
			}
		}

		if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
			upForce = 13.5f;
		}
		if (Input.GetKey (KeyCode.I)) {
			isFlying = true;
			upForce = 40f;
			if (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
				upForce = 50f;
			}

		} else if (Input.GetKey (KeyCode.K)) {
			upForce = -20f;
		} else if ((!Input.GetKey (KeyCode.I) && !Input.GetKey (KeyCode.K)) && (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.2f)) {
			upForce = _constv.GetGravity();
		} 
		print (_rigidbody.velocity);
	}
	void MovemenForward(){
		if (Input.GetAxis ("Vertical") != 0) {
			//_rigidbody.AddRelativeForce (Vector3.forward * MoveSpeed * Input.GetAxis ("Vertical"));
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, 20 * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.1f);
		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			//_rigidbody.AddRelativeForce (Vector3.right * MoveSpeed * Input.GetAxis ("Horizontal"));
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
	void updateEnvironmentForce(){
		_gravity = _constv.GetGravity() * (1 - (transform.position.y/_constv.GetEarthRadius()));
		Physics.gravity = new Vector3 (0, -_gravity * droneWeight, 0);
	}
	void AddHeliceForce(){
//		Vector3 worldForcePosition1 = HeliceUpL.TransformPoint(HeliceUpL.position);
//		Vector3 worldForcePosition2 = HeliceUpR.TransformPoint(HeliceUpR.position);
//		Vector3 worldForcePosition3 = HeliceDownL.TransformPoint(HeliceDownL.position);
//		Vector3 worldForcePosition4 = HeliceDownR.TransformPoint(HeliceDownR.position);
//		_rigidbody.AddForceAtPosition(Vector3.up * upForce, worldForcePosition1);
//		_rigidbody.AddForceAtPosition(Vector3.up * upForce, worldForcePosition2);
//		_rigidbody.AddForceAtPosition(Vector3.up * upForce, worldForcePosition3);
//		_rigidbody.AddForceAtPosition(Vector3.up * upForce, worldForcePosition4);
	}
}
