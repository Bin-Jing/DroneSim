using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFllow : MonoBehaviour {
	Transform Drone;
	Vector3 velocityCameraFllow;
	Vector3 CameraPosition = new Vector3(0, 40, -160);
	public float angle = 14;
	void Awake(){
		Drone = GameObject.FindGameObjectWithTag ("Player").transform;

	}
	void FixedUpdate(){
		transform.position = Vector3.SmoothDamp (transform.position, Drone.transform.TransformPoint (CameraPosition) + new Vector3(0,1,0) * Input.GetAxis ("Vertical"), ref velocityCameraFllow, 0.01f);
		transform.rotation = Quaternion.Euler (new Vector3 (angle, Drone.GetComponent<DroneMove> ().currentYRotation, 0));
	}
}
