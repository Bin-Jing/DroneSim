using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    public GameObject Obj;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position =  new Vector3(Obj.transform.position.x,Obj.transform.position.y+0.3f,Obj.transform.position.z);
	}
}
