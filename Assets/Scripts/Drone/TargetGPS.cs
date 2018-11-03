using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetGPS : MonoBehaviour {
    public double lat;
    public double lon;
    public double alt;
    GameObject player;
    ConstValue _constval;
    float EARTH_RADIUS;

    [HideInInspector] public double Startlat;
    [HideInInspector] public double Startlon;
    [HideInInspector] public double Startalt;

    public double newlat = 0;
    public double newlon = 0;
    public double newalt = 0;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        _constval = player.GetComponent<ConstValue>();
        EARTH_RADIUS = _constval.GetEarthRadius();
        lat = 24.988782;
        lon = 121.572313;
        alt = 0;
        Startlat = lat;
        Startlon = lon;
        Startalt = alt;
	}
	
	// Update is called once per frame
	void Update () {
        lon = Startlon + this.transform.position.x / (Math.Cos(lat * Math.PI / 180.0f) * EARTH_RADIUS) * 180.0f / Math.PI;
        lat = Startlat + this.transform.position.z / EARTH_RADIUS * 180.0f / Math.PI;
        alt = Startalt + this.transform.position.y;


	}
    public void NewGPSToLocal(double newlat, double newlon, double newalt)
    {
        double x = (newlon - Startlon) * (Math.Cos(lat * Math.PI / 180.0f) * EARTH_RADIUS) / 180.0f * Math.PI;
        double z = (newlat - Startlat) * EARTH_RADIUS / 180.0f * Math.PI;
        double y = newalt - Startalt;
        //print(x + " " + y + " " + z);
        this.transform.position = new Vector3((float) x,(float) y,(float) z);
    }
}
