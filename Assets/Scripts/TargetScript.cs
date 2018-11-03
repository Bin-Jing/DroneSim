using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerEnter(Collider other)
	{
        
        if(other.gameObject.tag == "Block"){
            other.gameObject.transform.localPosition = new Vector3(Random.Range(-35, 35), 0, Random.Range(20, 50));

        }
	}
}
