using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class WriteLoadJSON : MonoBehaviour {

    public LatLonAlt lla;
    public TargetGPS tarGPS;
    ObjectData myObject = new ObjectData();
    public int numberOfTar = 0;
    void WriteGPSJSON(){
        
        myObject.curLon = lla.lon;
        myObject.curLat = lla.lat;
        myObject.curAlt = lla.alt;

        string jsonString = JsonUtility.ToJson(myObject);
        //Debug.Log(jsonString);

        //System.IO.File.WriteAllText (Application.persistentDataPath + "/saveJSON.json");
        System.IO.File.WriteAllText(Application.dataPath + "/JSON/position.json", jsonString);
    }
    public void LoadTargetGPS(int i){
        
        string jsonString = File.ReadAllText(Application.dataPath + "/JSON/waypoint.json");

        ObjectArrayData load = JsonConvert.DeserializeObject<ObjectArrayData>(jsonString);
        numberOfTar = load.Items.Length;
        //print(load.Items[i].curLat+" "+ load.Items[i].curLon+" "+ load.Items[i].curAlt);
        if(i < load.Items.Length)
            tarGPS.NewGPSToLocal(load.Items[i].curLat, load.Items[i].curLon, load.Items[i].curAlt);
        //print(jsonString);
    }
	// Update is called once per frame
	void Update () {
        WriteGPSJSON();
        //LoadTargetGPS(1);

	}
   

    public class ObjectData
    {
        public double curLon;
        public double curLat;
        public double curAlt;
    }

    [Serializable]
    private class ObjectArrayData
    {
        public ObjectData[] Items;
    }
}
