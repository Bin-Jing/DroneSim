using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 需改進：RMP應緩慢下降，放開搖桿後速度不應直接歸零
public class DroneMove : MonoBehaviour {
	ConstValue _constval;
	Rigidbody _rigidbody;
	bool isFlying = false;
	[HideInInspector]public string CurCollision = "";

	Transform transform;
	public Transform HeliceUpR;
	public Transform HeliceUpL;
	public Transform HeliceDownR;
	public Transform HeliceDownL;

	//drone para
//	float DroneCrossSection = 0;
	public float droneWeight = 3f; //kg
	public float DroneLength = 0.5f; //m
	public float DroneWedth = 0.5f; //m
	float Tor = 0f;

	public float curRMP = 0;
	//Force
	public float upForce = 0f;
	float _gravity = 9.81f;

	//Speed
	float tiltAmountForward = 0f;
	float tiltAmountSwerve = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;
	float joinForces = 0;
	float tiltAmount = 0;
	public float curSpeed = 0;
	public float curSpeedX = 0;
	public float curSpeedY = 0;
	public float curSpeedZ = 0;

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
		_rigidbody.AddForce (new Vector3(0, -_gravity*droneWeight,0));
//		if (Input.GetKey (KeyCode.Z)) {
//			isFlying = false;
//			_rigidbody.velocity = new Vector3 (0, 0, 0);
//			upForce = 0;
//			return;
//		}
		MovemenForward ();
		Swerve ();
		PropellerForce ();
		Rotation ();
		updateLinearDrag ();
		updateAngularDrag ();
		updateAngularAcceleration ();

		AddHeliceForce ();
		_rigidbody.AddRelativeForce ( Vector3.up * upForce);

		curSpeed = _rigidbody.velocity.magnitude;

		_rigidbody.rotation = Quaternion.Euler (new Vector3(tiltAmountForward, currentYRotation, tiltAmountSwerve));
		curSpeedX = _rigidbody.velocity.x;
		curSpeedY = _rigidbody.velocity.y;
		curSpeedZ = _rigidbody.velocity.z;
		//print (_rigidbody.velocity);
	}

	void PropellerForce(){
		
		if ((Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f) || (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
			
			if (Input.GetAxis("FlyUp") == 0 && (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f)) {
				upForce = _constval.GetC_T () * _constval.GetAirDensity () * Mathf.Pow (_constval.GetAngularVelocity(), 2) 
					* Mathf.Pow (_constval.GetPropellerDiameter (), 4) * Mathf.Abs(Input.GetAxis ("Vertical"));
				curRMP = Mathf.Abs(_constval.GetMaxRMP () * Input.GetAxis ("Vertical"));
				tiltAmount = 40;
			}
			if (Input.GetAxis("FlyUp") == 0 && (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
				upForce = _constval.GetC_T () * _constval.GetAirDensity () * Mathf.Pow (_constval.GetAngularVelocity(), 2) 
					* Mathf.Pow (_constval.GetPropellerDiameter (), 4) * Mathf.Abs(Input.GetAxis ("Horizontal"));
				curRMP = Mathf.Abs(_constval.GetMaxRMP () * Input.GetAxis ("Horizontal"));
				tiltAmount = 40;
			}

		}

		if (Mathf.Abs (Input.GetAxis ("FlyUp")) >= 0.3f) {
			
			upForce = _constval.GetC_T () * _constval.GetAirDensity () * Mathf.Pow (_constval.GetAngularVelocity (), 2)
			* Mathf.Pow (_constval.GetPropellerDiameter (), 4) * Input.GetAxis ("FlyUp");
			
			if ((Mathf.Abs (Input.GetAxis ("Vertical")) != 0) || (Mathf.Abs (Input.GetAxis ("Horizontal")) != 0)) {
				tiltAmount = 20;
			} else {
				tiltAmount = 0;
			}
			curRMP = Mathf.Abs(_constval.GetMaxRMP () * Input.GetAxis ("FlyUp"));

		}  

		if(Input.GetAxis("FlyUp") == 0 && Input.GetAxis ("Vertical") == 0 && Input.GetAxis ("Horizontal") == 0){
			upForce = 0;
			curRMP = 0;
		}

	}
	void MovemenForward(){
		if (Input.GetAxis ("Vertical") != 0) {
			Tor = (1f/(2*Mathf.PI)) * _constval.GetC_Pow () * _constval.GetAirDensity () 
				* Mathf.Pow (rotationAmount, 2) * Mathf.Pow (_constval.GetPropellerDiameter (), 5) * Input.GetAxis ("FlyUp");
			_rigidbody.AddRelativeForce (Vector3.forward * upForce * Mathf.Abs (Mathf.Sin(_rigidbody.rotation.eulerAngles.x * Mathf.PI /180))
				* Input.GetAxis ("Vertical"));
			_rigidbody.AddRelativeTorque (Vector3.forward * Tor);
			tiltAmountForward = Mathf.SmoothDamp (tiltAmountForward, tiltAmount * Input.GetAxis ("Vertical"), ref tiltVelocityForward, 0.01f);
		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			Tor = (1f / (2 * Mathf.PI)) * _constval.GetC_Pow () * _constval.GetAirDensity ()
				* Mathf.Pow (rotationAmount, 2) * Mathf.Pow (_constval.GetPropellerDiameter (), 5) * Input.GetAxis ("FlyUp");
			_rigidbody.AddRelativeForce (Vector3.right * upForce * Mathf.Abs (Mathf.Sin(_rigidbody.rotation.eulerAngles.z* Mathf.PI /180)) 
				* Input.GetAxis ("Horizontal"));
			_rigidbody.AddRelativeTorque (Vector3.right * Tor);
			tiltAmountSwerve = Mathf.SmoothDamp (tiltAmountSwerve, -tiltAmount * Input.GetAxis ("Horizontal"), ref tiltVelocitySwerve, 0.01f);
		}
	}
	void Rotation(){
		if (Input.GetAxis ("Rotation") < 0) {
			YRotation -= rotationAmount;
		}
		if (Input.GetAxis ("Rotation") > 0) {
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
		float sinx = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180));
		float cosx = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180));
		float sinz = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180));
		float cosz = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180));
		float cosy = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.y * Mathf.PI /180));
		float siny = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.y * Mathf.PI /180));
		float velocityForward = Mathf.Pow (Mathf.Pow (_rigidbody.velocity.x, 2) + Mathf.Pow (_rigidbody.velocity.z, 2), 0.5f);

		float dragForward = (1/2f) * Mathf.Pow(velocityForward,2) * _constval.GetAirDensity () 
			* _constval.GetClin () * sinx * cosz * DroneWedth * DroneLength;

		float dragUPward = (1/2f) * Mathf.Pow(_rigidbody.velocity.y,2) * _constval.GetAirDensity () 
			* _constval.GetClin () * cosx * cosz * DroneWedth * DroneLength;
		
		float dragSwerve = (1/2f) * Mathf.Pow(velocityForward,2) * _constval.GetAirDensity () 
			* _constval.GetClin () * sinz * DroneWedth * DroneLength;
		
		_rigidbody.drag = Mathf.Pow( (Mathf.Pow(dragForward,2) + Mathf.Pow(dragUPward,2) + Mathf.Pow(dragSwerve,2)),0.5f);
//		print (_rigidbody.drag);
	}

	void updateAngularDrag(){
		_rigidbody.angularDrag = (Mathf.PI * _constval.GetC_AngularDrag () / 5f) * _constval.GetAirDensity ()
			* Mathf.Pow (rotationAmount, 2) * Mathf.Pow (((DroneWedth / 2f) + (DroneLength / 2f)) / 2f, 5);
	}
	void updateAngularAcceleration(){
		float TorNet = Tor + (_constval.GetPropellerDiameter()/2) * upForce - _rigidbody.angularDrag;
		float AngularAcceleration = TorNet / _constval.GetMomentInertia ();
//		print (AngularAcceleration);

	}
	void OnCollisionEnter (Collision col){
		CurCollision = "Collision detected";
	}
	void OnCollisionExit (Collision col){
		CurCollision = "";
	}
}
