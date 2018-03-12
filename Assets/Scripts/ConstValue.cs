﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstValue : MonoBehaviour {
	const float EarthRadius = 6378100;
	const float g0 = 9.81f;
	const float linear_drag_coefficient = 1.3f / 4.0f;
	const float angular_drag_coefficient = linear_drag_coefficient;
	const float restitution = 0.15f;
	const float power_coefficient = 0.040164f; // the torque co-efficient at @ 6396.667 RPM
	const float thrust_coefficient =  0.109919f; // the thrust co-efficient @ 6396.667 RPM
	const float air_density = 1.225f;//  kg/m^3
	const float max_rpm = 6396.667f;
	const float propeller_diameter = 0.2f;

	public float GetEarthRadius(){
		return EarthRadius;
	}
	public float GetGravity(){
		return g0;
	}
	public float GetClin(){
		return linear_drag_coefficient;
	}
	public float GetCang(){
		return angular_drag_coefficient;
	}
	public float GetRestitution(){
		return restitution;
	}
	public float GetC_P(){
		return power_coefficient;
	}
	public float GetC_T(){
		return thrust_coefficient;
	}
	public float GetAirDensity(){
		return air_density;
	}
	public float GetMaxRMP(){
		return max_rpm;
	}
	public float GetPropellerDiameter(){
		return propeller_diameter;
	}
	public float GetAngularVelocity(){
		return max_rpm * 2 * Mathf.PI / 60;
	}
}
