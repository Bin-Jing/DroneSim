using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// rotation Y RPM
public class DroneMove : MonoBehaviour {
	ConstValue _constval;
	Rigidbody _rigidbody;
	bool isFlying = false;
	[HideInInspector]public string CurCollision = "";

	Transform transform;

	float timer;
	//drone para
//	float DroneCrossSection = 0;
	public float droneWeight = 1f; //kg
	public float DroneLength = 0.5f; //m
	public float DroneWedth = 0.5f; //m
	float Tor = 0f;
	float propellerDiameter;
	float propellerVelocity;
	float maxRPM;
	public float curRPM = 0;
	float refRPM = 0f;
	float airDensity;
	float thrustCoefficient;
	float powerCoefficient;
	float linearDragCoefficient;
	float AngularDragCoefficient;

	//Force
	public float upForce = 0f;
	float refForce = 0f;
	float _gravity = 9.81f;
	public float Lineardrag = 0f;
	public float AngularDrag = 0f;
	[HideInInspector]public float _thrust;


	//RotationXZ
	float currentXRotation = 0f;
	float currentZRotation = 0f;
	float tiltVelocityForward = 0f;
	float tiltVelocitySwerve = 0f;
	[HideInInspector]public float rotationXVelocity = 0f;
	[HideInInspector]public float rotationZVelocity = 0f;
	float tiltAmount = 0;//control the max rotation

	//Speed
	public float curSpeed = 0;
	public float curSpeedX = 0;
	public float curSpeedY = 0;
	public float curSpeedZ = 0;
	float AngularAcceleration;
	float _Acceleration;
	[HideInInspector]public float _AccelX;
	[HideInInspector]public float _AccelY;
	[HideInInspector]public float _AccelZ;
	[HideInInspector]public float _throttle;

	//RotationY
	[HideInInspector]float YRotation = 180f;
	[HideInInspector]public float currentYRotation;
	[HideInInspector]float rotationAmount = 0f;
	[HideInInspector]public float rotationYVelocity = 0f;

	[HideInInspector]public float vibrationX;
	[HideInInspector]public float vibrationY;
	[HideInInspector]public float vibrationZ;
	float oldXRotation;
	float oldYRotation;
	float oldZRotation;


	// Use this for initialization
	void Awake () {
		currentYRotation = 180f;
		_constval = GetComponent<ConstValue> ();
		_rigidbody = GetComponent<Rigidbody>();
		transform = GetComponent<Transform> ();
		valueInitialize ();
		timer = 0;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.deltaTime;
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

		_rigidbody.AddRelativeForce ( Vector3.up * upForce);


		_rigidbody.rotation = Quaternion.Euler (new Vector3(currentXRotation, currentYRotation, currentZRotation));
		updateSpeedInfo ();

		//print (_rigidbody.velocity);

	}
	void valueInitialize(){
		_Acceleration = 0;
		oldXRotation = transform.rotation.eulerAngles.x;
		oldYRotation = transform.rotation.eulerAngles.y;
		oldZRotation = transform.rotation.eulerAngles.z;
		_gravity = _constval.GetGravity ();
		propellerDiameter = _constval.GetPropellerDiameter ();
		propellerVelocity = _constval.GetAngularVelocity ();
		maxRPM = _constval.GetMaxRPM ();
		airDensity = _constval.GetAirDensity ();
		thrustCoefficient = _constval.GetC_T ();
		powerCoefficient = _constval.GetC_Pow ();
		linearDragCoefficient = _constval.GetClin ();
		AngularDragCoefficient = _constval.GetC_AngularDrag ();
	}
	void PropellerForce(){
		
		if ((Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f) || (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
			
			if (Input.GetAxis("FlyUp") == 0 && (Mathf.Abs (Input.GetAxis ("Vertical")) > 0.2f)) {
				upForce = thrustCoefficient * airDensity * Mathf.Pow (propellerVelocity, 2) 
					* Mathf.Pow (propellerDiameter, 4) * Mathf.Abs(Input.GetAxis ("Vertical"));
				curRPM = Mathf.Abs(maxRPM * Input.GetAxis ("Vertical"));

				tiltAmount = 40;
			}
			if (Input.GetAxis("FlyUp") == 0 && (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.2f)) {
				upForce = thrustCoefficient * airDensity * Mathf.Pow (propellerVelocity, 2) 
					* Mathf.Pow (propellerDiameter, 4) * Mathf.Abs(Input.GetAxis ("Horizontal"));
				curRPM = Mathf.Abs(maxRPM * Input.GetAxis ("Horizontal"));
				tiltAmount = 40;
			}

		}

		if (Mathf.Abs (Input.GetAxis ("FlyUp")) >= 0.3f) {
			
			upForce = thrustCoefficient * airDensity * Mathf.Pow (propellerVelocity, 2)
				* Mathf.Pow (propellerDiameter, 4) * Input.GetAxis ("FlyUp");
			
			if ((Mathf.Abs (Input.GetAxis ("Vertical")) != 0) || (Mathf.Abs (Input.GetAxis ("Horizontal")) != 0)) {
				tiltAmount = 20;
			} else {
				tiltAmount = 0;
			}
			curRPM = Mathf.Abs(maxRPM * Input.GetAxis ("FlyUp"));

		}  

		if(Input.GetAxis("FlyUp") == 0 && Input.GetAxis ("Vertical") == 0 && Input.GetAxis ("Horizontal") == 0){
			
			upForce = Mathf.SmoothDamp (upForce, 0, ref refForce, 1f);
			curRPM = Mathf.SmoothDamp (curRPM, 0, ref refRPM, 1/Mathf.Pow(2,0.5f));
		}

	}
	void MovemenForward(){
		if (Input.GetAxis ("Vertical") != 0) {
			Tor = (1f / (2 * Mathf.PI)) * powerCoefficient * airDensity
				* Mathf.Pow (rotationZVelocity, 2) * Mathf.Pow (propellerDiameter, 5) * Input.GetAxis ("FlyUp");
			_rigidbody.AddRelativeForce (Vector3.forward * upForce * Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.x * Mathf.PI / 180))
			* Input.GetAxis ("Vertical"));
			_rigidbody.AddRelativeTorque (Vector3.forward * Tor);
			if (Input.GetAxis ("Vertical") >= 0) {
				currentXRotation = Mathf.SmoothDamp (currentXRotation, tiltAmount , ref tiltVelocityForward, 1f);
			} else if (Input.GetAxis ("Vertical") < 0) {
				currentXRotation = Mathf.SmoothDamp (currentXRotation, -tiltAmount , ref tiltVelocityForward, 1f);
			}

		}
	}
	void Swerve(){
		if (Input.GetAxis ("Horizontal") != 0) {
			Tor = (1f / (2 * Mathf.PI)) * powerCoefficient * airDensity
				* Mathf.Pow (rotationXVelocity, 2) * Mathf.Pow (propellerDiameter, 5) * Input.GetAxis ("FlyUp");
			_rigidbody.AddRelativeForce (Vector3.right * upForce * Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.z * Mathf.PI / 180))
			* Input.GetAxis ("Horizontal"));
			_rigidbody.AddRelativeTorque (Vector3.right * Tor);
			if (Input.GetAxis ("Horizontal") >= 0) {
				currentZRotation = Mathf.SmoothDamp (currentZRotation, -tiltAmount , ref tiltVelocitySwerve, 1f);
			} else if (Input.GetAxis ("Horizontal") < 0) {
				currentZRotation = Mathf.SmoothDamp (currentZRotation, tiltAmount , ref tiltVelocitySwerve, 1f);
			}

		}
	}
	void Rotation(){

		if (Input.GetAxis ("Rotation") != 0) {
			if (upForce <= 40) {
				upForce = thrustCoefficient * airDensity * Mathf.Pow (propellerVelocity, 2)
					* Mathf.Pow (propellerDiameter, 4) * HoverControlInput();
				
				curRPM = maxRPM * Mathf.Abs(Input.GetAxis ("Rotation"))*HoverControlInput();
					 
					
			}

			YRotation += rotationAmount * Input.GetAxis ("Rotation") ;
			
		}
		currentYRotation = Mathf.SmoothDamp (currentYRotation, YRotation, ref rotationYVelocity, 1f);

	}
	void updateEnvironmentForce(){
		_gravity = _constval.GetGravity() * (1 - (transform.position.y/_constval.GetEarthRadius()));
//		print (_gravity);
	}

	void updateLinearDrag(){
		float sinx = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180));
		float cosx = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.x * Mathf.PI /180));
		float sinz = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180));
		float cosz = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.z * Mathf.PI /180));
		float cosy = Mathf.Abs (Mathf.Cos (_rigidbody.rotation.eulerAngles.y * Mathf.PI /180));
		float siny = Mathf.Abs (Mathf.Sin (_rigidbody.rotation.eulerAngles.y * Mathf.PI /180));
		float velocityForward = Mathf.Pow (Mathf.Pow (_rigidbody.velocity.x, 2) + Mathf.Pow (_rigidbody.velocity.z, 2), 0.5f);

		float dragForward = (1/2f) * Mathf.Pow(velocityForward,2) * airDensity
			* linearDragCoefficient * sinx * cosz * DroneWedth * DroneLength;

		float dragUPward = (1/2f) * Mathf.Pow(_rigidbody.velocity.y,2) * airDensity 
			* linearDragCoefficient * cosx * cosz * DroneWedth * DroneLength;
		
		float dragSwerve = (1/2f) * Mathf.Pow(velocityForward,2) * airDensity 
			* linearDragCoefficient * sinz * DroneWedth * DroneLength;
		
		_rigidbody.drag = Mathf.Pow( (Mathf.Pow(dragForward,2) + Mathf.Pow(dragUPward,2) + Mathf.Pow(dragSwerve,2)),0.5f);
		Lineardrag = _rigidbody.drag;
//		print (_rigidbody.drag);
	}

	void updateAngularDrag(){
		_rigidbody.angularDrag = (Mathf.PI * AngularDragCoefficient / 5f) * airDensity
			* Mathf.Pow (rotationYVelocity, 2) * Mathf.Pow (((DroneWedth / 2f) + (DroneLength / 2f)) / 2f, 5)+
			(Mathf.PI * AngularDragCoefficient / 5f) * airDensity
			* Mathf.Pow (rotationZVelocity, 2) * Mathf.Pow (((DroneWedth / 2f) + (DroneLength / 2f)) / 2f, 5)+
			(Mathf.PI * AngularDragCoefficient / 5f) * airDensity
			* Mathf.Pow (rotationXVelocity, 2) * Mathf.Pow (((DroneWedth / 2f) + (DroneLength / 2f)) / 2f, 5);
		AngularDrag = _rigidbody.angularDrag;
	}
	void updateAngularAcceleration(){
		float TorNet = Tor + (propellerDiameter/2) * upForce - _rigidbody.angularDrag;
		AngularAcceleration = TorNet / _constval.GetMomentInertia ();
		rotationAmount = AngularAcceleration * Time.deltaTime;
	}
	void updateSpeedInfo(){
		_rigidbody.mass = droneWeight;
		_throttle = (curRPM / maxRPM) * 100;
		_Acceleration = (_rigidbody.velocity.magnitude - curSpeed)/Time.deltaTime;
		_thrust = _rigidbody.mass * _Acceleration;
		curSpeed = _rigidbody.velocity.magnitude;
		_AccelX = (_rigidbody.velocity.x - curSpeedX)/Time.deltaTime;
		_AccelY = (_rigidbody.velocity.y - curSpeedY)/Time.deltaTime;
		_AccelZ = (_rigidbody.velocity.z - curSpeedZ)/Time.deltaTime;
		curSpeedX = _rigidbody.velocity.x;
		curSpeedY = _rigidbody.velocity.y;
		curSpeedZ = _rigidbody.velocity.z;
		vibrationX = (transform.rotation.eulerAngles.x - oldXRotation);
		vibrationY = (transform.rotation.eulerAngles.y - oldYRotation);
		vibrationZ = (transform.rotation.eulerAngles.z - oldZRotation);
		if (vibrationX > 180) {
			vibrationX -= 360;
		} else if(vibrationX < -180){
			vibrationX += 360;
		}

		if (vibrationZ > 180) {
			vibrationZ -= 360;
		} else if(vibrationZ < -180){
			vibrationZ += 360;
		}
		rotationXVelocity = vibrationX/Time.deltaTime;
		rotationZVelocity = vibrationZ/Time.deltaTime;

		oldXRotation = transform.rotation.eulerAngles.x;
		oldYRotation = transform.rotation.eulerAngles.y;
		oldZRotation = transform.rotation.eulerAngles.z;
	}
	float HoverControlInput(){
		return 0.5f;
	}
	void OnCollisionEnter (Collision col){
		CurCollision = "Collision detected";
	}
	void OnCollisionExit (Collision col){
		CurCollision = "";
	}
}
