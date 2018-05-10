using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {
	public Text SpeedText;
	public Text RMPText;
	public Text CollisionText;
	public Text ConnectText;
	DroneMove DM;
	UdpClient udp;

	void Start(){
		DM = GameObject.FindGameObjectWithTag ("Player").GetComponent<DroneMove> ();
		udp = GameObject.FindGameObjectWithTag ("System").GetComponent<UdpClient> ();
	}
		
	void Update () {
		SpeedText.text = "Speed : " + DM.curSpeed;
		RMPText.text = "RPM : " + DM.curRPM;
		ConnectText.text = "Connect : " + udp.ConnectString;
		CollisionText.text = DM.CurCollision;
	}
}
