using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DrawRouteLine : MonoBehaviour {
    
    LineRenderer lineRenderer;
    List<Vector3> points = new List<Vector3>();
    public GameObject player;

    DroneAgent DA;
    //public Action<IEnumerable<Vector3>> OnVewPathCreated = delegate;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //player = GameObject.FindWithTag("Player");
        DA = player.gameObject.GetComponent<DroneAgent>();
    }
	private void Update()
	{
        
        if(true){
            points.Add(player.transform.position);

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
            //if(DA.IsDone()){
            //    lineRenderer.material.color = new Color(0,0,0);
            //}
        }
	}
    private float DistanceToLastPoint(Vector3 point){
        if (!points.Any())
            return Mathf.Infinity;
        return Vector3.Distance(points.Last(), point);
    }
}
