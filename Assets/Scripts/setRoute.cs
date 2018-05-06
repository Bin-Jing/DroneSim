using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class setRoute : MonoBehaviour {
	StreamWriter writer;
	bool fileOpen = false;
	int rec_gap = 0;
	int ring_gap = 30;
	int ring_show_gap = 10;
	float gerGap = 10f;
	int Filecount = 0;
	bool Recording = false;
	bool ENABLE = true;
	Vector3 pos;
	Vector3 prePos = Vector3.zero;
	GameObject player;
	string path;
	DroneMove DM;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		DM = player.GetComponent<DroneMove> ();

		//MavLink.MavLinkSerializer.Deserialize_ALTITUDE ();

//		else if(ENABLE){
//			/* generate rings , balls */
//			int counter = 0;
//			StreamReader reader = new StreamReader (path);
//			while (true) {
//				var str = reader.ReadLine ();
//				if (str == null)
//					break;
//				var parts = str.Split (' ');
//				pos = new Vector3 (float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
//				Quaternion rot = Quaternion.Euler(float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5]));
//				if (prePos != Vector3.zero && Vector3.Distance (prePos, pos) < gerGap)
//					continue;
//				if (counter == ring_show_gap) {
//					Instantiate (ring, pos, rot);
//				}
//				else
//					Instantiate (coin, pos , rot);
//				counter++;
//				counter %= ring_gap;
//				prePos = pos;
//			}
//		}
	}

	// Update is called once per frame
	void Update () {
		GetBytesSingle (player.transform.position.x);
		path = "Assets/routes/File" + Filecount + ".txt";
		//Write some text to the test.txt file


		if (Input.GetKey (KeyCode.Alpha1)) {
			fileOpen = false;
			writer.Close ();
			print ("close file");
		}
		if (Recording) {
			if (rec_gap == 0) {
				rec_gap = 1;
				writer.WriteLine (player.transform.position.x + "," + player.transform.position.y + "," + player.transform.position.z
					+ "," + player.transform.eulerAngles.x + "," + player.transform.eulerAngles.y+ "," + player.transform.eulerAngles.z
					+ "," + DM.curSpeed + "," + DM.curSpeedX + "," + DM.curSpeedY + "," + DM.curSpeedZ + "," + DM.curRMP);
				print ("write file");
			}
			rec_gap--;
		}
	}
	public static void GetBytesSingle( float argument )
	{
		byte[ ] byteArray = System.BitConverter.GetBytes( argument );
		//print(byteArray[0].ToString("X2"));
		//System.Console.WriteLine( formatter, argument,System.BitConverter.ToString( byteArray ) );
	}
	public void StartRecording(){
		Recording = !Recording;
		if (Recording) {
			Filecount += 1;
			writer = new StreamWriter (path, false);
			fileOpen = true;
			writer.WriteLine ("PositionX,PositionY,PositionZ,RotationX,RotationY,RotataionZ,Speed,SpeedX,SpeedY,SpeedZ,RMP");

		} else {
			fileOpen = false;
			writer.Close ();
			print ("close file");
		}

	}
}
