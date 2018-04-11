using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {
	public Text SpeedText;
	public Text RMPText;
	public Text CollisionText;
	DroneMove DM;

	void Start(){
		DM = GameObject.FindGameObjectWithTag ("Player").GetComponent<DroneMove> ();
	}
		
	void Update () {
		SpeedText.text = "Speed : " + DM.curSpeed;
		RMPText.text = "RMP : " + DM.curRMP;
		CollisionText.text = DM.CurCollision;
	}
}
