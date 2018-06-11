using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : Agent {
	float planeSize = 20f;
	private float previousDistance = float.MaxValue;
    DroneMove DM;
	Rigidbody rBody;
    float StartX;
    float StartZ;
    float timer = 0f;
	void Start () {
		rBody = GetComponent<Rigidbody>();
        DM = GameObject.FindGameObjectWithTag("Player").GetComponent<DroneMove>();
        resetTarget();
        StartX = this.transform.position.x;
        StartZ = this.transform.position.z;
	}

	public Transform Target;
	public override void AgentReset()
	{
        if (this.transform.position.y < -1.0 ||
            this.transform.position.y > planeSize + 2 ||
            Mathf.Abs(this.transform.localPosition.x) > (planeSize / 2) + 1 ||
            Mathf.Abs(this.transform.localPosition.z) > (planeSize / 2) + 1)
        {  
            this.transform.position = new Vector3(StartX, 0, StartZ);
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
		}
	}
	public override List<float> CollectState()
	{
		List<float> state = new List<float>();
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 relativePosition = Target.position - this.transform.position;
        state.Add((transform.localPosition.x)/10);
        state.Add((transform.localPosition.y)/10);
        state.Add((transform.localPosition.z)/10);

        state.Add(relativePosition.x/10);
        state.Add(relativePosition.y/10);
        state.Add(relativePosition.z/10);


		state.Add(velocity.x/10);
		state.Add(velocity.y/10);
		state.Add(velocity.z/10);
        //state.Add(DM.rotationXVelocity/10);
        //state.Add(DM.rotationYVelocity/10);
        //state.Add(DM.rotationZVelocity/10);
		return state;
	}
	public override void AgentStep(float[] act)
	{
        timer += Time.deltaTime;

        int action = Mathf.FloorToInt(act[0]);
        float directionX = 0;
        float directionZ = 0;
        float directionY = 0;

        directionY = Mathf.Clamp(act[2], -1f, 1f);
        directionZ = Mathf.Clamp(act[1], -1f, 1f);
        directionX = Mathf.Clamp(act[0], -1f, 1f);

        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(directionZ*20, directionY* 20, directionX* 20));
		float distanceToTarget = Vector3.Distance(this.transform.position, 
			Target.position);
        
        
		
		if (distanceToTarget < 1f)
		{
			done = true;
			reward = 1.0f;
            resetTarget();
            timer = 0;
            //this.transform.position = new Vector3(StartX, 0, StartZ);
            //this.rBody.angularVelocity = Vector3.zero;
            //this.rBody.velocity = Vector3.zero;
		}


		if (distanceToTarget < previousDistance)
		{
			reward = 0.1f;
        }

        reward = -0.003f;
		
        if(distanceToTarget == previousDistance){
            reward = -0.1f;
        }else if(distanceToTarget > previousDistance){
            reward = -0.15f;
        }
		
        if (this.transform.position.y < -1.0 ||
            this.transform.position.y > planeSize + 2 ||
            Mathf.Abs(this.transform.localPosition.x) > (planeSize / 2) + 1 ||
            Mathf.Abs(this.transform.localPosition.z) > (planeSize / 2) + 1)
		{
			done = true;
			reward = -1f;
            resetTarget();
            timer = 0;
		}
        if (timer > 30)
        {
            resetTarget();
            timer = 0;
            reward = -1f;
        }
        //print(rBody.velocity);
		previousDistance = distanceToTarget;


	}
    void resetTarget(){
        Target.localPosition = new Vector3(Random.value * planeSize - 10, Random.value * planeSize/3 + 0.5f, Random.value * planeSize - 10);

    }

}
