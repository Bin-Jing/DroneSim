using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDoorTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Block")
        {
            other.gameObject.transform.localPosition = new Vector3(Random.Range(-13, 13), 0, Random.value * 28);

        }
    }
}
