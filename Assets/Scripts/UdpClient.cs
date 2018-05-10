using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using MavLink;
using System.Linq;


public class UdpClient:MonoBehaviour
{
	
	LatLonAlt latlonalt;
	Msg_local_position_ned mlpn;
	Msg_heartbeat Mheratbeat;
	Msg_sys_status Msts;
	Msg_attitude Matt;
	Msg_attitude_quaternion Mattq;
	Msg_request_data_stream Mdatastream;
	Msg_gps_raw_int Mgpsr;
	Msg_global_position_int mgpi;
	Msg_vfr_hud Mvhud;
	Msg_attitude_target MattTar;
	Msg_home_position MHome;
	Msg_set_mode setMode;

	Mavlink Mv = new Mavlink();
	const ushort UInt16_Max = 65535;
	GameObject player;
	DroneMove DM;

	float PitchS = 20;//??????
	float YawS = 20;//?????
	float RollS = 20;//??????

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
	byte[] recvData=new byte[40];
	byte[] sendData=new byte[40]; 
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
		connectThread=new Thread(new ThreadStart(SocketReceive));
		connectThread.Start();
	}
		


	void SocketReceive()
	{

		while(true)
		{

			recvData=new byte[40];
			recvLen=socket.ReceiveFrom(recvData,ref serverEnd);

			Mv.ParseBytes (recvData);
			print (System.BitConverter.ToString(recvData));

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
		mgpi = new Msg_global_position_int ();
		Mheratbeat = new Msg_heartbeat ();
		Msts = new Msg_sys_status ();
		Matt = new Msg_attitude ();
		Mdatastream = new Msg_request_data_stream();
		Mgpsr = new Msg_gps_raw_int ();
		Mattq = new Msg_attitude_quaternion ();
		Mvhud = new Msg_vfr_hud ();
		MattTar = new Msg_attitude_target ();
		MHome = new Msg_home_position ();
		setMode = new Msg_set_mode ();
		latlonalt = new LatLonAlt ();


		player = GameObject.FindGameObjectWithTag ("Player");
		DM = player.GetComponent<DroneMove> ();
		InitSocket(); 

		setHomePosition ();
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
		time += (Time.deltaTime)*1000;
		//heartbeat
		HeartBeat();

//		//status
		StatusUpdate();
//
//		//position
		LocalPositionUpdate();
		GlobalPositionUpdate ();
//
//		//attitude
		AttitudeUpdate();

		RequestDataStream ();
		GPSUpdate ();
		UpdateAttitudeQuaternion ();
		UpdateHud ();
		UpdateAttitudeTarget ();
		HomePosition ();

		setMsgMode ();
	}

	void HeartBeat(){
		Mheratbeat.mavlink_version = (byte)(3.0);
		Mheratbeat.autopilot = (byte)MAV_AUTOPILOT.MAV_AUTOPILOT_GENERIC;
		Mheratbeat.type = (byte)MAV_TYPE.MAV_TYPE_QUADROTOR;
		Mheratbeat.base_mode = (byte)MAV_MODE.MAV_MODE_MANUAL_ARMED;
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
		Msts.onboard_control_sensors_health = 1;
		Msts.onboard_control_sensors_present = 1;
		Msts.voltage_battery = 11000;

		SendPacket (Msts);
	}
	void LocalPositionUpdate(){
		mlpn.x = player.transform.position.x;
		mlpn.y = player.transform.position.y;
		mlpn.z = player.transform.position.z;
		mlpn.vx = DM.curSpeedX; 
		mlpn.vy = DM.curSpeedY; 
		mlpn.vz = DM.curSpeedZ;
		mlpn.time_boot_ms = (uint)(time);

		SendPacket (mlpn);		
	}

	void GlobalPositionUpdate(){
		mgpi.alt = (int)(latlonalt.alt * 1000);
		mgpi.hdg = UInt16_Max;
		mgpi.lat = (int)(latlonalt.lat*10000000);
		mgpi.lon = (int)(latlonalt.lon*10000000);
		mgpi.time_boot_ms = (uint)time;
		mgpi.relative_alt = (int)(player.transform.position.y * 1000);
		mgpi.vx = (short)(DM.curSpeedX * 100);
		mgpi.vy = (short)(DM.curSpeedY * 100);
		mgpi.vz = (short)(DM.curSpeedZ * 100);

		SendPacket (mgpi);
	}

	void AttitudeUpdate(){
		Matt.time_boot_ms = (uint)(time);
		Matt.pitch = player.transform.eulerAngles.x;
		Matt.yaw = player.transform.eulerAngles.y;
		Matt.roll = player.transform.eulerAngles.z;
		Matt.pitchspeed = PitchS;
		Matt.yawspeed = YawS;
		Matt.rollspeed = RollS;

		SendPacket (Matt);
	}
	void RequestDataStream(){
		Mdatastream.req_message_rate = 2;
		Mdatastream.req_stream_id = (byte)MAV_DATA_STREAM.MAV_DATA_STREAM_ALL;
		Mdatastream.start_stop = 1;
		Mdatastream.target_component = (byte)MAV_COMPONENT.MAV_COMP_ID_ALL;
		Mdatastream.target_system = (byte)SystemId;

		SendPacket (Mdatastream);
	}
	void GPSUpdate(){
		Mgpsr.alt = (int)(latlonalt.alt*1000);
		Mgpsr.cog = UInt16_Max;//??????
		Mgpsr.eph = 1;//?????
		Mgpsr.epv = 1;//???????
		Mgpsr.fix_type = (byte)GPS_FIX_TYPE.GPS_FIX_TYPE_NO_FIX;//???????
		Mgpsr.lat = (int)(latlonalt.lat*10000000);
		Mgpsr.lon = (int)(latlonalt.lon*10000000);
		Mgpsr.satellites_visible = 255;//???????
		Mgpsr.time_usec = (ulong)(time);
		Mgpsr.vel = (ushort)(DM.curSpeed*100);

		SendPacket (Mgpsr);

	}
	void UpdateAttitudeQuaternion(){
		Mattq.pitchspeed = PitchS;
		Mattq.rollspeed = RollS;
		Mattq.yawspeed = YawS;

		Mattq.q1 = player.transform.rotation.w;
		Mattq.q2 = player.transform.rotation.x;
		Mattq.q3 = player.transform.rotation.y;
		Mattq.q4 = player.transform.rotation.z;
		Mattq.time_boot_ms = (uint)time;

		SendPacket (Mattq);

	}
	void UpdateHud(){
		Mvhud.airspeed = DM.curSpeed;
		Mvhud.groundspeed = DM.curSpeed;
		Mvhud.alt = latlonalt.alt;
		Mvhud.climb = DM.curSpeedY;
		Mvhud.heading = (short)player.transform.eulerAngles.y;
		Mvhud.throttle = 0;//?????

		SendPacket (Mvhud);

	}
	void UpdateAttitudeTarget(){
		float[] qA = new float [4];
		qA[0] = player.transform.rotation.w;
		qA[1] = player.transform.rotation.x;
		qA[2] = player.transform.rotation.y;
		qA[3] = player.transform.rotation.z;
		MattTar.time_boot_ms = (uint)time;
		MattTar.body_pitch_rate = PitchS;
		MattTar.body_roll_rate = RollS;
		MattTar.body_yaw_rate = YawS;
		MattTar.type_mask = 0;//??????
		MattTar.q = qA;
		MattTar.thrust = 0;//????????

		SendPacket (MattTar);
	}

	void HomePosition(){
		float[] qA = new float [4];
		qA[0] = 0;
		qA[1] = 0;
		qA[2] = 0;
		qA[3] = 0;


		MHome.altitude = (int)(latlonalt.Startalt*1000);
		MHome.latitude = (int)(latlonalt.Startlat*10000000);
		MHome.longitude = (int)(latlonalt.Startlon*10000000);
		MHome.q = qA;
		MHome.x = 0;
		MHome.y = 0;
		MHome.z = 0;

		MHome.approach_x = 0;//??????
		MHome.approach_y = 0;//???????
		MHome.approach_z = 0;//???????

		SendPacket (MHome);
	}

	void setHomePosition(){
		Msg_set_home_position setMHome = new Msg_set_home_position();

		float[] qA = new float [4];
		qA[0] = 0;
		qA[1] = 0;
		qA[2] = 0;
		qA[3] = 0;


		setMHome.altitude = (int)(latlonalt.Startalt*1000);
		setMHome.latitude = (int)(latlonalt.Startlat*10000000);
		setMHome.longitude = (int)(latlonalt.Startlon*10000000);
		setMHome.q = qA;
		setMHome.x = 0;
		setMHome.y = 0;
		setMHome.z = 0;

		setMHome.approach_x = 0;//??????
		setMHome.approach_y = 0;//???????
		setMHome.approach_z = 0;//???????

		SendPacket (setMHome);
	}

	void setMsgMode(){
		setMode.base_mode = (byte)MAV_MODE.MAV_MODE_MANUAL_ARMED;
		setMode.custom_mode = 0;
		setMode.target_system = (byte)MAV_MODE.MAV_MODE_MANUAL_ARMED;

		SendPacket (setMode);
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
//		Serial.Write(by, 0, by.Length);
		socket.SendTo(by,by.Length,SocketFlags.None,ipEnd);


	}

}

