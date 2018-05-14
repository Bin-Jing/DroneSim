using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCameraControl : MonoBehaviour {

	// Use this for initialization
	Transform Drone;

	void Awake(){
		Drone = GameObject.FindGameObjectWithTag ("Player").transform;

	}
	void FixedUpdate(){
		transform.rotation = Quaternion.Euler (new Vector3(90,
			Drone.GetComponent<DroneMove> ().currentYRotation,
			Drone.transform.rotation.eulerAngles.z * Mathf.PI /180));
	}
}
