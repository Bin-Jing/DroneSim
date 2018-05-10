using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatLonAlt : MonoBehaviour {
	ConstValue _constval;
	GameObject player;
	public float lat = 0;
	public float lon = 0;
	public float alt = 100;

	public float Startlat;
	public float Startlon;
	public float Startalt;

	// Use this for initialization
	void Start () {
		_constval = GetComponent<ConstValue> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		Startlat = lat;
		Startlon = lon;
		Startalt = alt;
	}
	
	// Update is called once per frame
	void Update () {
		lat += player.transform.position.x / _constval.GetEarthRadius() * 180.0f / Mathf.PI;
		lon += player.transform.position.z / (Mathf.Cos(lat * Mathf.PI / 180.0f) * _constval.GetEarthRadius()) * 180.0f / Mathf.PI;
		alt -= player.transform.position.y;
	}
}
