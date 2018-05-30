using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InfoUI : MonoBehaviour {
	public Text SpeedText;
	public Text RMPText;
	public Text CollisionText;
	public Text ConnectText;
	public Text LinDragText;
	public Text AngDragText;
	bool connected = true;
	DroneMove DM;
	UdpClient udp;
	void Start(){
		connected = true;
		DM = GameObject.FindGameObjectWithTag ("Player").GetComponent<DroneMove> ();
		udp = GameObject.FindGameObjectWithTag ("System").GetComponent<UdpClient> ();
	}
		
	void Update () {
		SpeedText.text = "Speed : " + DM.curSpeed.ToString("n2") + " m/s";
		RMPText.text = "RPM : " + DM.curRPM.ToString("n3");

		LinDragText.text = "Drag : " + DM.Lineardrag.ToString("n4");
		AngDragText.text = "Angular Drag : " + DM.AngularDrag.ToString("n4");
		CollisionText.text = DM.CurCollision;
		if (connected) {
			ConnectText.text = "Drone State - connect: " + udp.ConnectString;
		} else {
			ConnectText.text = "Drone State - connect: false";
		}

	}
	public void RestartBtn(){
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}
	public void DisconnectBtn(){
		udp.enabled = false;
		connected = false;
	}
}
