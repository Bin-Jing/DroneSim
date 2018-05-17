using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public class LatLonAlt : MonoBehaviour {
	ConstValue _constval;
	GameObject player;

	public double lat;
	public double lon;
	public double alt;

	public double Startlat;
	public double Startlon;
	public double Startalt;
	float EARTH_RADIUS;
	// Use this for initialization
	void Start () {
		
		player = GameObject.FindGameObjectWithTag ("Player");
		_constval = player.GetComponent<ConstValue> ();
		EARTH_RADIUS = _constval.GetEarthRadius ();
		lat = 24.988782;
		lon = 121.572313;
		alt = 100;
		Startlat = lat;
		Startlon = lon;
		Startalt = alt;
	}

	// Update is called once per frame
	void Update () {


		lon = Startlon+player.transform.position.x / (Math.Cos(lat * Math.PI / 180.0f) * EARTH_RADIUS) * 180.0f / Math.PI;
		lat = Startlat+player.transform.position.z / EARTH_RADIUS * 180.0f / Math.PI;
		alt = Startalt-player.transform.position.y;
		print (lon.ToString("n6")+ " "+lat.ToString("n6"));
	}
}
