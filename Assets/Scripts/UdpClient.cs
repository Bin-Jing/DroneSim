using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using MavLink;

public class UdpClient:MonoBehaviour
{
	const int buf_len = 17;
	Msg_local_position_ned mlpn;
	Msg_heartbeat Mheratbeat;
	Msg_sys_status Msts;
	Msg_attitude Matt;
	Msg_request_data_stream Mdatastream;
	Mavlink Mv = new Mavlink();

	GameObject player;
	DroneMove DM;

	float time = 0;
	[HideInInspector]public string ConnectString = "false";
	int seq = 0;
	int SystemId;
	Socket socket; 
	EndPoint serverEnd; 
	IPEndPoint ipEnd; 
	IPEndPoint ipLoc;
	string recvStr; 
	string sendStr; 
	byte[] recvData=new byte[buf_len];
	byte[] sendData=new byte[buf_len]; 
	int recvLen; 
	Thread connectThread; 


	void InitSocket()
	{


		ipEnd=new IPEndPoint(IPAddress.Parse("127.0.0.1"),14550);
		//ipLoc = new IPEndPoint(IPAddress.Parse("127.0.0.1"),14551);

		socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
		IPEndPoint sender=new IPEndPoint(IPAddress.Any,0);
		serverEnd=(EndPoint)sender;

		print("waiting for sending UDP dgram");
		//socket.Bind (ipLoc);
		//SocketSend();
		connectThread=new Thread(new ThreadStart(SocketReceive));
		connectThread.Start();
	}
		


	void SocketReceive()
	{

		while(true)
		{

			recvData=new byte[buf_len];
			recvLen=socket.ReceiveFrom(recvData,ref serverEnd);
			print("message from: "+serverEnd.ToString()); //打印服务端信息

			recvStr=Encoding.ASCII.GetString(recvData,0,recvLen);
			print (recvStr);

		}
	}


	void SocketQuit()
	{

		if(connectThread!=null)
		{
			connectThread.Interrupt();
			connectThread.Abort();
		}
		if(socket!=null)
			socket.Close();
	}

	// Use this for initialization
	void Start()
	{
		Application.runInBackground = true;
		mlpn = new Msg_local_position_ned ();
		Mheratbeat = new Msg_heartbeat ();
		Msts = new Msg_sys_status ();
		Matt = new Msg_attitude ();
		Mdatastream = new Msg_request_data_stream();

		player = GameObject.FindGameObjectWithTag ("Player");
		DM = player.GetComponent<DroneMove> ();
		InitSocket(); 


	}

	// Update is called once per frame
	void Update()
	{
		if (socket.Connected) {
			ConnectString = serverEnd.ToString();
		}else{
			ConnectString = "false";
		}
		if (seq >= 255) {
			seq = 0;
		} else {
			seq += 1;
		}
		time += Time.deltaTime;
		//heartbeat
		HeartBeat();

//		//status
		StatusUpdate();
//
//		//position
		PositionUpdate();
//
//
//		//attitude
		AttitudeUpdate();

	}
	void RequestDataStream(){
		Mdatastream.req_message_rate = 2;
		Mdatastream.req_stream_id = (byte)MAV_DATA_STREAM.MAV_DATA_STREAM_ALL;
		Mdatastream.start_stop = 1;
		Mdatastream.target_component = (byte)MAV_COMPONENT.MAV_COMP_ID_ALL;
		Mdatastream.target_system = (byte)SystemId;
	}
	void HeartBeat(){
		Mheratbeat.mavlink_version = (byte)(3.0);
		Mheratbeat.autopilot = (byte)MAV_AUTOPILOT.MAV_AUTOPILOT_GENERIC;
		Mheratbeat.type = (byte)MAV_TYPE.MAV_TYPE_QUADROTOR;
		Mheratbeat.base_mode = (byte)MAV_MODE.MAV_MODE_GUIDED_ARMED;
		Mheratbeat.custom_mode = 0;
		Mheratbeat.system_status = (byte)MAV_STATE.MAV_STATE_ACTIVE;

		SendPacket (Mheratbeat);
	}
	void StatusUpdate(){
		Msts.battery_remaining = 100;
		Msts.current_battery = -1;
		Msts.drop_rate_comm = 0;
		Msts.errors_comm = 0;
		Msts.errors_count1 = 0;
		Msts.errors_count2 = 0;
		Msts.errors_count3 = 0;
		Msts.errors_count4 = 0;
		Msts.load = 50;
		Msts.onboard_control_sensors_enabled = 1;
		Msts.onboard_control_sensors_health = 0;
		Msts.onboard_control_sensors_present = 1;
		Msts.voltage_battery = 11000;

		SendPacket (Msts);
	}
	void PositionUpdate(){
		mlpn.x = player.transform.position.x;
		mlpn.y = player.transform.position.y;
		mlpn.z = player.transform.position.z;
		mlpn.vx = DM.curSpeedX; 
		mlpn.vy = DM.curSpeedY; 
		mlpn.vz = DM.curSpeedZ;
		mlpn.time_boot_ms = (uint)(time*1000);

		SendPacket (mlpn);		
	}
	void AttitudeUpdate(){
		Matt.time_boot_ms = (uint)(time*1000);
		Matt.pitch = player.transform.eulerAngles.x;
		Matt.yaw = player.transform.eulerAngles.y;
		Matt.roll = player.transform.eulerAngles.z;
		Matt.pitchspeed = 20;
		Matt.yawspeed = 20;
		Matt.rollspeed = 20;

		SendPacket (Matt);
	}

	void OnApplicationQuit()
	{
		SocketQuit();
	}
	private void SendPacket(MavlinkMessage m){
		MavlinkPacket p = new MavlinkPacket();
		p.Message = m;
		p.SequenceNumber = (byte)seq;
		p.SystemId = 255;
		p.ComponentId = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
		byte[] by = Mv.Send(p);
		//Serial.Write(by, 0, by.Length);
		socket.SendTo(by,by.Length,SocketFlags.None,ipEnd);
	}

}

