using UnityEngine;
using System.Collections;

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
	Msg_heartbeat Mheratbeat;
	Msg_sys_status Msts;
	Msg_attitude Matt;
	Msg_attitude_quaternion Mattq;
	Msg_request_data_stream Mdatastream;
	Msg_gps_raw_int Mgpsr;
	Msg_vfr_hud Mvhud;
	Msg_attitude_target MattTar;
	Msg_set_mode setMode;
	Msg_estimator_status EstimatorStatus;
	Msg_vibration Mvirbration;
	Msg_extended_sys_state ExtendState;

	//position
	Msg_home_position MHome;
	Msg_set_home_position setMHome;
	Msg_local_position_ned mlpn;
	Msg_global_position_int mgpi;
	Msg_set_position_target_local_ned setPositionL;
	Msg_position_target_local_ned positionL;
	Msg_set_position_target_global_int setPositionG;
	Msg_position_target_global_int PositionG;
	Msg_altitude Malt;

	Msg_highres_imu MhiIMU;
	Msg_battery_status BatteryStatus;
	Mavlink Mv = new Mavlink();
	const ushort UInt16_Max = 65535;
	GameObject player;
	DroneMove DM;

	int updateCount = 0;

	byte SystemId;
	byte ComponentId;

	float PitchS = 0;
	float YawS = 0;
	float RollS = 0;

	float time = 0;
	[HideInInspector]public string ConnectString = "false";
	int seq = 0;
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




	// Use this for initialization
	void Start()
	{
		
		SystemId = (byte)MAV_MODE.MAV_MODE_MANUAL_ARMED;
		ComponentId = (byte)MAV_COMPONENT.MAV_COMP_ID_AUTOPILOT1;
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
		setMHome = new Msg_set_home_position();
		setMode = new Msg_set_mode ();
		EstimatorStatus = new Msg_estimator_status ();
		Mvirbration = new Msg_vibration ();
		ExtendState = new Msg_extended_sys_state ();


		Malt = new Msg_altitude ();
		positionL = new Msg_position_target_local_ned ();
		setPositionL = new Msg_set_position_target_local_ned ();
		setPositionG = new Msg_set_position_target_global_int();
		PositionG = new Msg_position_target_global_int();

		MhiIMU = new Msg_highres_imu ();
		BatteryStatus = new Msg_battery_status ();

		latlonalt = GameObject.FindGameObjectWithTag ("System").GetComponent<LatLonAlt> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		DM = player.GetComponent<DroneMove> ();
		InitSocket(); 

		setHomePosition ();
		setPositionTargetLocal ();
		setPositionTargetGlobal ();

	}

	// Update is called once per frame
	void Update()
	{
		print (latlonalt.lon+ " "+latlonalt.lat);
		PitchS = DM.tiltVelocityForward;
		YawS = DM.rotationYVelocity;
		RollS = DM.tiltVelocitySwerve;

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
		time += (Time.deltaTime)*1000000;
		//heartbeat
		HeartBeat();//#0
		StatusUpdate();//#1
		GPSUpdate ();//#24
		AttitudeUpdate();//#30
		UpdateAttitudeQuaternion ();//#31
		LocalPositionUpdate();//#32
		GlobalPositionUpdate ();//#33
		UpdateHud ();//#74
		UpdateAttitudeTarget ();//#83
		PositionTargetLocal();//#85 POSITION_TARGET_LOCAL_NED
		PositionTargetGlobal ();//#87 POSITION_TARGET_GLOBAL_INT
		UpdateIMU ();//#105
		UpdateAltitude();//#141  ALTITUDE
		UpdateBatteryStatus();//#147
		UpdataEstimatorStatus();//#230 ESTIMATOR_STATUS
		//UpdateVirbration();//#241 VIBRATION
		HomePosition ();//#242
		UpdateExtendState();//#245 Extend




//		#36 SERVO_OUTPUT_RAW
//		RequestDataStream ();//#66
//		setMavMode ();//#11
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
		Mdatastream.target_component = ComponentId;
		Mdatastream.target_system = SystemId;

		SendPacket (Mdatastream);
	}
	void GPSUpdate(){
		Mgpsr.alt = (int)(latlonalt.alt*1000);
		Mgpsr.cog = UInt16_Max;//??????
		Mgpsr.eph = UInt16_Max;//?????
		Mgpsr.epv = UInt16_Max;//???????
		Mgpsr.fix_type = (byte)GPS_FIX_TYPE.GPS_FIX_TYPE_3D_FIX;//???????
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

		//unity rotation didnt match mav rotation
		Mattq.q1 = player.transform.rotation.w;
		Mattq.q2 = player.transform.rotation.z;
		Mattq.q3 = -player.transform.rotation.x;
		Mattq.q4 = player.transform.rotation.y;
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
		MattTar.time_boot_ms = (uint)time;

		//unity rotation didnt match mav rotation
		float[] qA = new float [4];
		qA[0] = player.transform.rotation.w;
		qA[1] = player.transform.rotation.z;
		qA[2] = -player.transform.rotation.x;
		qA[3] = player.transform.rotation.y;

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
		setMHome.target_system = SystemId;
		SendPacket (setMHome);
	}

	void setMavMode(){
		setMode.base_mode = (byte)MAV_MODE.MAV_MODE_MANUAL_ARMED;
		setMode.custom_mode = 0;
		setMode.target_system = SystemId;

		SendPacket (setMode);
	}

	void UpdateIMU(){
		updateCount = (1 + updateCount)%13;

		MhiIMU.time_usec = (uint)(time);

		MhiIMU.xacc = 0;
		MhiIMU.xgyro = 0;
		MhiIMU.xmag = 0;
		MhiIMU.yacc = 0;
		MhiIMU.ygyro = 0;
		MhiIMU.ymag = 0;
		MhiIMU.zacc = 0;
		MhiIMU.zgyro = 0;
		MhiIMU.zmag = 0;

		MhiIMU.temperature = 20;
		MhiIMU.pressure_alt = 0;
		MhiIMU.abs_pressure = 0;
		MhiIMU.diff_pressure = 0;
		MhiIMU.fields_updated = (byte)updateCount;

		SendPacket (MhiIMU);
	}

	void UpdateBatteryStatus(){
		ushort[] voltages = new ushort [1];
		voltages [0] = 11000;

		BatteryStatus.id = 0;
		BatteryStatus.energy_consumed = -1;
		BatteryStatus.current_consumed = -1;
		BatteryStatus.current_battery = -1;
		BatteryStatus.battery_remaining = 100;
		BatteryStatus.temperature = 0;
		BatteryStatus.voltages = voltages;

		BatteryStatus.type = (byte)MAV_BATTERY_TYPE.MAV_BATTERY_TYPE_UNKNOWN;
		BatteryStatus.battery_function = (byte)MAV_BATTERY_FUNCTION.MAV_BATTERY_FUNCTION_UNKNOWN;

		SendPacket (BatteryStatus);
	}

	void setPositionTargetLocal(){
		setPositionL.time_boot_ms = (uint)time;
		setPositionL.afx = 0;
		setPositionL.afy = 0;
		setPositionL.afz = 0;
		setPositionL.coordinate_frame = (byte)MAV_FRAME.MAV_FRAME_LOCAL_NED;
		setPositionL.target_component = ComponentId;
		setPositionL.target_system = SystemId;
		setPositionL.type_mask = 0;
		setPositionL.vx = DM.curSpeedX;
		setPositionL.vy = DM.curSpeedY;
		setPositionL.vz = DM.curSpeedZ;
		setPositionL.x = player.transform.position.x;
		setPositionL.y = player.transform.position.y;
		setPositionL.z = player.transform.position.z;
		setPositionL.yaw = player.transform.eulerAngles.y;
		setPositionL.yaw_rate = YawS;

		SendPacket (setPositionL);
	}

	void PositionTargetLocal(){
		positionL.time_boot_ms = (uint)time;
		positionL.afx = 0;
		positionL.afy = 0;
		positionL.afz = 0;
		positionL.coordinate_frame = (byte)MAV_FRAME.MAV_FRAME_LOCAL_NED;
		positionL.type_mask = 0;
		positionL.vx = DM.curSpeedX;
		positionL.vy = DM.curSpeedY;
		positionL.vz = DM.curSpeedZ;
		positionL.x = player.transform.position.x;
		positionL.y = player.transform.position.y;
		positionL.z = player.transform.position.z;
		positionL.yaw = player.transform.eulerAngles.y;
		positionL.yaw_rate = YawS;

		SendPacket (positionL);
	}

	void setPositionTargetGlobal(){
		setPositionG.time_boot_ms = (uint)time;

		setPositionG.afx = 0;
		setPositionG.afy = 0;
		setPositionG.afz = 0;
		setPositionG.coordinate_frame = (byte)MAV_FRAME.MAV_FRAME_GLOBAL_TERRAIN_ALT_INT;
		setPositionG.lat_int = (int)(latlonalt.lat*10000000);
		setPositionG.lon_int = (int)(latlonalt.lon*10000000);
		setPositionG.alt = latlonalt.alt;
		setPositionG.target_component = ComponentId;
		setPositionG.target_system = SystemId;
		setPositionG.type_mask = 0;
		setPositionG.vx = DM.curSpeedX;
		setPositionG.vy = DM.curSpeedY;
		setPositionG.vz = DM.curSpeedZ;
		setPositionG.yaw = player.transform.eulerAngles.y;
		setPositionG.yaw_rate = YawS;

		SendPacket (setPositionG);


	}

	void PositionTargetGlobal(){
		PositionG.time_boot_ms = (uint)time;

		PositionG.afx = 0;//???????
		PositionG.afy = 0;//???????
		PositionG.afz = 0;//???????
		PositionG.coordinate_frame = (byte)MAV_FRAME.MAV_FRAME_GLOBAL_TERRAIN_ALT_INT;
		PositionG.lat_int = (int)(latlonalt.lat*10000000);
		PositionG.lon_int = (int)(latlonalt.lon*10000000);
		PositionG.alt = latlonalt.alt;
		PositionG.type_mask = 0;
		PositionG.vx = DM.curSpeedX;
		PositionG.vy = DM.curSpeedY;
		PositionG.vz = DM.curSpeedZ;
		PositionG.yaw = player.transform.eulerAngles.y;
		PositionG.yaw_rate = YawS;

		SendPacket (PositionG);
	}

	void UpdateAltitude(){
		Malt.time_usec = (uint)time;
		Malt.altitude_amsl = latlonalt.alt;//??????
		Malt.altitude_local = player.transform.position.y;//????????
		Malt.altitude_monotonic = 0;
		Malt.altitude_relative = player.transform.position.y;
		Malt.altitude_terrain = player.transform.position.y;//??????

		SendPacket (Malt);

	}

	void UpdataEstimatorStatus(){//????
		EstimatorStatus.time_usec = (uint)time;
		EstimatorStatus.flags = (ushort)ESTIMATOR_STATUS_FLAGS.ESTIMATOR_STATUS_FLAGS_ENUM_END;
		EstimatorStatus.hagl_ratio = player.transform.position.y;
		EstimatorStatus.mag_ratio = 1;
		EstimatorStatus.pos_horiz_accuracy = player.transform.position.z;
		EstimatorStatus.pos_horiz_ratio = 1;
		EstimatorStatus.pos_vert_accuracy = player.transform.position.z;
		EstimatorStatus.pos_vert_ratio = 1;
		EstimatorStatus.tas_ratio = 1;
		EstimatorStatus.vel_ratio = DM.curSpeed;

		SendPacket (EstimatorStatus);
	}

	void UpdateVirbration(){//??????
		Mvirbration.time_usec = (uint)time;
		Mvirbration.clipping_0 = 0;
		Mvirbration.clipping_1 = 0;
		Mvirbration.clipping_2 = 0;
		Mvirbration.vibration_x = 0;
		Mvirbration.vibration_y = 0;
		Mvirbration.vibration_z = 0;

		SendPacket (Mvirbration);
	}

	void UpdateExtendState(){
		ExtendState.landed_state = (byte)MAV_LANDED_STATE.MAV_LANDED_STATE_UNDEFINED;
		ExtendState.vtol_state = (byte)MAV_VTOL_STATE.MAV_VTOL_STATE_UNDEFINED;

		SendPacket (ExtendState);
	}

	void OnApplicationQuit()
	{
		SocketQuit();
	}
	private void SendPacket(MavlinkMessage m){
		MavlinkPacket p = new MavlinkPacket();
		p.Message = m;
		p.SequenceNumber = (byte)seq;
		p.SystemId = SystemId;
		p.ComponentId = ComponentId;
		byte[] by = Mv.Send(p);
//		Serial.Write(by, 0, by.Length);
		socket.SendTo(by,by.Length,SocketFlags.None,ipEnd);


	}

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
}

