using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCameraControl : MonoBehaviour {
    //Transform Drone;
    public Camera ca;
	void Awake(){
        //Drone = GameObject.FindGameObjectWithTag ("Player").transform;
        ca.depthTextureMode = DepthTextureMode.Depth;

	}
	void FixedUpdate(){
		//transform.rotation = Quaternion.Euler (new Vector3(27.3f + -1 * Drone.transform.rotation.eulerAngles.x * Mathf.PI /180,
			//Drone.GetComponent<DroneMove> ().currentYRotation,
			//Drone.transform.rotation.eulerAngles.z * Mathf.PI /180));
	}

}
