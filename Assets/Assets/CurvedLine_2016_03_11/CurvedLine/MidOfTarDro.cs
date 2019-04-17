using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidOfTarDro : MonoBehaviour {
    public GameObject Tar;
    public GameObject Dro;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = new Vector3((Tar.transform.position.x + Dro.transform.position.x) / 2,
                                              (Tar.transform.position.y + Dro.transform.position.y),
                                              (Tar.transform.position.z + Dro.transform.position.z) / 2);
	}
}
