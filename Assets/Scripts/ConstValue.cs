using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstValue : MonoBehaviour {
	const float EarthRadius = 637810000;
	const float g0 = 9.81f;
	const float linear_drag_coefficient = 1.3f / 4.0f;
	const float angular_drag_coefficient = linear_drag_coefficient;
	const float restitution = 0.15f;


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


}
