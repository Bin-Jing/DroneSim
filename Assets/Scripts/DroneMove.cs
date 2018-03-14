using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroneMove : MonoBehaviour {
	ConstValue _constval;
	Rigidbody _rigidbody;
	bool isFlying = false;

	Transform transform;
	public Transform HeliceUpR;
	public Transform HeliceUpL;
	public Transform HeliceDownR;
	public Transform HeliceDownL;


	//Forice
	public float upForce = 0f;
	float droneWeight = 3f;

	float _gravity = 9.81f;

	//Speed
	float MoveSpeed = 10f;
	float tiltAmountForward = 0f;
	float tiltAmountSwerve = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;
	float joinForces = 0;

	//Rotation
	float YRotation = 0f;
	public float currentYRotation = 0f;
	float rotationAmount = 2.5f;
	float rotationYVelocity = 0f;

	// Use this for initialization
	void Awake () {
		_constval = GetComponent<ConstValue> ();
		_rigidbody = GetComponent<Rigidbody>();
		transform = GetComponent<Transform> ();

		_gravity = _constval.GetGravity ();
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
		updateLinearDrag ();

		AddHeliceForce ();
		_rigidbody.AddRelativeForce (Vector3.up * upForce);
		_rigidbody.AddForce (new Vector3(0, -_gravity*droneWeight,0));


		_rigidbody.rotation = Quaternion.Euler (new Vector3(tiltAmountForward, currentYRotation, tiltAmountSwerve));
	}
	void PropellerForce(){
		if ((Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f) || (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
			if (Input.GetAxis("FlyUp") != 0) {
				_rigidbody.velocity = _rigidbody.velocity;
			}
			if (Input.GetAxis("FlyUp") == 0 && !Input.GetKey (KeyCode.J) && !Input.GetKey (KeyCode.L)) {
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
				upForce = 0;//28.1f;
			}
			if (Input.GetAxis("FlyUp") == 0 && (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L))) {
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
				upForce = 0;//11f;
			}
			if (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L)) {
				upForce = 0;//41f;
			}
		}

		if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
			upForce = 0;//13.5f;
		}
		if (Mathf.Abs(Input.GetAxis("FlyUp")) >= 0.3f) {
			
//			isFlying = true;
			upForce = _constval.GetC_T () * _constval.GetAirDensity () * Mathf.Pow (_constval.GetMaxRMP(), 2) 
				* Mathf.Pow (_constval.GetPropellerDiameter (), 4) * Input.GetAxis ("FlyUp")/100;
//			if (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
//				upForce = 50f;
//			}

		} 
		print (_rigidbody.velocity);
		if(Input.GetAxis("FlyUp") == 0){
			upForce = _constval.GetGravity()*droneWeight;
		}
	}
	void MovemenForward(){
		if (Input.GetAxis ("Vertical") != 0) {
			_rigidbody.AddRelativeForce (Vector3.forward * MoveSpeed * Input.GetAxis ("Vertical"));
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, 30 * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.1f);
		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			_rigidbody.AddRelativeForce (Vector3.right * MoveSpeed * Input.GetAxis ("Horizontal"));
			tiltAmountSwerve = Mathf.SmoothDamp (tiltAmountSwerve, -30 * Input.GetAxis ("Horizontal"), ref tiltVelocitySwerve, 0.1f);
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
		_gravity = _constval.GetGravity() * (1 - (transform.position.y/_constval.GetEarthRadius()));
//		print (_gravity);
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
	void updateLinearDrag(){
		float absVelocity = Mathf.Pow(Mathf.Pow(_rigidbody.velocity.x,2)+Mathf.Pow(_rigidbody.velocity.y,2)+Mathf.Pow(_rigidbody.velocity.z,2),0.5f);
		_rigidbody.drag = (float)(0.5 * (Mathf.Pow(absVelocity,2)*_constval.GetAirDensity()*_constval.GetClin()*0.015f));
//		print (_rigidbody.drag);
	}
}
