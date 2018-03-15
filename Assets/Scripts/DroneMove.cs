using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroneMove : MonoBehaviour {
	ConstValue _constval;
	Rigidbody _rigidbody;
	bool isFlying = false;

	Transform transform;
//	public Transform HeliceUpR;
//	public Transform HeliceUpL;
//	public Transform HeliceDownR;
//	public Transform HeliceDownL;

	//drone para
	float DroneCrossSection = 0;
	float droneWeight = 3f;
	float DroneLength = 0.5f;
	float DroneWedth = 0.5f;

	//Force
	public float upForce = 0f;
	float _gravity = 9.81f;

	//Speed
	float tiltAmountForward = 0f;
	float tiltAmountSwerve = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;
	float joinForces = 0;
	//Rotation
	float YRotation = 0f;
	[HideInInspector]public float currentYRotation = 0f;
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
//				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
//				upForce = 0;//28.1f;
			}
			if (Input.GetAxis("FlyUp") == 0 && (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L))) {
//				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Lerp(_rigidbody.velocity.y, 0, Time.deltaTime * 5), _rigidbody.velocity.z);
//				upForce = 0;//11f;
			}
			if (Input.GetKey (KeyCode.J) || Input.GetKey (KeyCode.L)) {
//				upForce = 0;//41f;
			}

		}

		if (Mathf.Abs (Input.GetAxis ("Vertical")) < 0.2f && Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f) {
//			upForce = 0;//13.5f;
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
			_rigidbody.AddRelativeForce (Vector3.forward * 10//upForce * Mathf.Sin(_rigidbody.rotation.eulerAngles.x) 
				* Input.GetAxis ("Vertical"));
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, 30 * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.01f);
		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			_rigidbody.AddRelativeForce (Vector3.right * 10//upForce * Mathf.Sin(_rigidbody.rotation.eulerAngles.z) 
				* Input.GetAxis ("Horizontal"));
			tiltAmountSwerve = Mathf.SmoothDamp (tiltAmountSwerve, -30 * Input.GetAxis ("Horizontal"), ref tiltVelocitySwerve, 0.01f);
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
		float sinx = Mathf.Sin (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180);
		float cosx = Mathf.Cos (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180);
		float sinz = Mathf.Sin (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180);
		float cosz = Mathf.Cos (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180);
		float cosy = Mathf.Cos (_rigidbody.rotation.eulerAngles.y * Mathf.PI /180);
		float dragx = _rigidbody.velocity.x * _constval.GetAirDensity () * _constval.GetClin () * sinz * cosy;
		float dragy = _rigidbody.velocity.y * _constval.GetAirDensity () * _constval.GetClin () * cosx * cosz;
		float dragz = _rigidbody.velocity.z * _constval.GetAirDensity () * _constval.GetClin () * sinx * cosz;
//		float absVelocity = Mathf.Pow(Mathf.Pow(_rigidbody.velocity.x,2)+Mathf.Pow(_rigidbody.velocity.y,2)+Mathf.Pow(_rigidbody.velocity.z,2),0.5f);
		_rigidbody.drag =Mathf.Pow( (Mathf.Pow(dragx,2) + Mathf.Pow(dragy,2) + Mathf.Pow(dragz,2)),0.5f);
//		print (_rigidbody.drag);
	}
}
