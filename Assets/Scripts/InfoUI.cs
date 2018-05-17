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
	DroneMove DM;
	UdpClient udp;

	void Start(){
		DM = GameObject.FindGameObjectWithTag ("Player").GetComponent<DroneMove> ();
		udp = GameObject.FindGameObjectWithTag ("System").GetComponent<UdpClient> ();
	}
		
	void Update () {
		SpeedText.text = "Speed : " + DM.curSpeed.ToString("n2") + " m/s";
		RMPText.text = "RPM : " + DM.curRPM;
		ConnectText.text = "Connect : " + udp.ConnectString;
		LinDragText.text = "Drag : " + DM.Lineardrag.ToString("n4");
		AngDragText.text = "Angular Drag : " + DM.AngularDrag.ToString("n4");
		CollisionText.text = DM.CurCollision;
	}
	public void RestartBtn(){
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}
}
