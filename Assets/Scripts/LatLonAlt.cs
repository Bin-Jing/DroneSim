using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatLonAlt : MonoBehaviour {
	ConstValue _constval;
	GameObject player;

	public float lat;
	public float lon;
	public float alt;

	public float Startlat;
	public float Startlon;
	public float Startalt;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		_constval = player.GetComponent<ConstValue> ();

		lat = 24.986264f;
		lon = 121.571793f;
		alt = 100;
		Startlat = lat;
		Startlon = lon;
		Startalt = alt;
	}
	
	// Update is called once per frame
	void Update () {
		lon = Startlon+player.transform.position.x / (Mathf.Cos(lat * Mathf.PI / 180.0f) * _constval.GetEarthRadius()) * 180.0f / Mathf.PI;
		lat = Startlat+player.transform.position.z / _constval.GetEarthRadius() * 180.0f / Mathf.PI;
		alt = Startalt-player.transform.position.y;
	}
}
