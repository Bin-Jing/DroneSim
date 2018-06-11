using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineScript : MonoBehaviour {
    
    public Transform t2;
    GameObject player;
    Transform tar;
    Renderer rend;
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        rend = GetComponent<Renderer>();
        tar = t2.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if(tar == null)
            tar = player.transform;
        DrawLine(tar.position, this.transform.position, Color.green, Time.deltaTime/2);

	}
	private void OnTriggerEnter(Collider other)
	{
        rend.enabled = false;

        Destroy(this.gameObject);
        
	}
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.2f, 0.2f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
