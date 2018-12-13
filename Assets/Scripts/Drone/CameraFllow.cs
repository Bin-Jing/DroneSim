using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFllow : MonoBehaviour {
	public Transform tar;

	float angle = 15;
	void Awake(){

	}
	void FixedUpdate(){
        transform.LookAt(tar);
	}
}
