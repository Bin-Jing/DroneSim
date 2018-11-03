using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAgentOutDoor2 : Agent {
	float planeSize = 100f;
    float planeW = 100;
    float planeL = 100;
	private float previousDistance = 110;
    private float previousRotation = 0;
    DroneMove DM;
	Rigidbody rBody;
    float StartX;
    float StartZ;
    float timer = 0f;
    float timer2 = 0f;
    int goalCounter = 0;
    int counter = 0;
    float t = 0;
    float TotalTime = 0;

    DroneAgent4Out DDDD;
	void Start () {
		rBody = GetComponent<Rigidbody>();
        DM = this.gameObject.GetComponent<DroneMove>();

        resetTarget();
        StartX = this.transform.localPosition.x;
        StartZ = this.transform.localPosition.z;
        DDDD = this.gameObject.GetComponent<DroneAgent4Out>();
        DM.AutoMove = true;
	}

	public Transform Target;
    public Transform goal;
    public Transform [] Block;
	public override void AgentReset()
	{
        DDDD.AgentOnDone();
        DDDD.InitializeAgent();

        this.transform.localPosition = new Vector3(StartX, 1f, StartZ);
        DM.SetRotation(0, Random.value * 360, 0);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        previousDistance = 110;

        System.GC.Collect();
        timer2 = 0;
	}
	public override void CollectObservations()
	{
		
        Vector3 velocity = rBody.velocity;
        //Vector3 relativePosition = Target.localPosition - this.transform.localPosition;
        //Vector3 Rotation = transform.rotation.eulerAngles / 180.0f - Vector3.one;
        //float arcTanToTar = Mathf.Atan(relativePosition.z / relativePosition.x) * 180 / Mathf.PI;
        //float relativeRotation = 0;

        //if (relativePosition.x > 0 && relativePosition.z > 0)
        //{
        //    relativeRotation = (90 - (arcTanToTar));
        //}
        //else if (relativePosition.x < 0 && relativePosition.z > 0)
        //{
        //    relativeRotation = (90 - (arcTanToTar)) + 180;
        //}
        //else if (relativePosition.x < 0 && relativePosition.z < 0)
        //{
        //    relativeRotation = (90 - (arcTanToTar)) + 180;
        //}
        //else if (relativePosition.x > 0 && relativePosition.z < 0)
        //{
        //    relativeRotation = (90 - (arcTanToTar));
        //}
        //relativeRotation = relativeRotation - this.transform.rotation.eulerAngles.y;
        //if (relativeRotation > 180)
        //{
        //    relativeRotation = relativeRotation - 360;
        //}
        //else if (relativeRotation < -180)
        //{
        //    relativeRotation = relativeRotation + 360;
        //}
        //relativeRotation = Mathf.Abs(relativeRotation) / 180 - 1;

        //AddVectorObs((transform.localPosition.x)/100);
        //AddVectorObs((transform.localPosition.y)/100);
        //AddVectorObs((transform.localPosition.z)/100);

        //AddVectorObs(relativePosition.x/100);
        //AddVectorObs(relativePosition.y/100);
        //AddVectorObs(relativePosition.z/100);


        AddVectorObs(velocity.x/100);
        AddVectorObs(velocity.y/100);
        AddVectorObs(velocity.z/100);
        //AddVectorObs(Rotation);
        //AddVectorObs(relativeRotation);

	}
	public override void AgentAction(float[] vectorAction, string textAction)
	{
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  goal.localPosition);;
        Vector3 relativePosition = goal.transform.localPosition - this.transform.localPosition;
        float arcTanToTar = Mathf.Atan(relativePosition.z / relativePosition.x) * 180 / Mathf.PI;
        float relativeRotation = 0;

        if(relativePosition.x > 0 && relativePosition.z > 0){
            relativeRotation = (90 - (arcTanToTar));
        }
        else if(relativePosition.x < 0 && relativePosition.z > 0){
            relativeRotation = (90 - (arcTanToTar)) + 180;
        }else if (relativePosition.x < 0 && relativePosition.z < 0){
            relativeRotation = (90 - (arcTanToTar))+180;
        }else if (relativePosition.x > 0 && relativePosition.z < 0){
            relativeRotation = (90 - (arcTanToTar));
        }
        relativeRotation = relativeRotation - this.transform.rotation.eulerAngles.y;
        if(relativeRotation >180){
            relativeRotation = relativeRotation - 360;
        }else if(relativeRotation < -180){
            relativeRotation = relativeRotation + 360;
        }
        float absReRo = Mathf.Abs(relativeRotation);
        if(!DDDD.enabled){
            timer += Time.deltaTime;
        }
        timer2 += Time.deltaTime;


        float directionX = 0;
        float directionZ = 0;
        float directionY = 0;
        //float rotataionY = 0;

        //rotataionY = vectorAction[0];
        if(!DDDD.enabled){
            directionY = Mathf.Clamp(vectorAction[2], -1f, 1f);
            directionZ = Mathf.Clamp(vectorAction[1], -1f, 1f);
            directionX = Mathf.Clamp(vectorAction[3], -1f, 1f);


            if (this.transform.position.y > 50)
            {
                AddReward(-0.01f * (this.transform.position.y - 50 + 1));
            }

            AddReward(-0.001f * timer);

            //if (absReRo > 90)
            //{
            //    AddReward(-0.001f * absReRo);
            //}

            //if (this.transform.position.y < 0.5f)
            //{
            //    AddReward(-0.5f);
            //}

            //DM.Rotation(rotataionY);

            DM.PropellerForce(directionZ, directionX, directionY);
            DM.MovemenForward(directionZ, directionX, directionY);
            DM.Swerve(directionZ, directionX);


            AddReward(0.1f / (distanceToTarget + 1));

            if (this.transform.position.y < -3.0 ||
                this.transform.position.y > 60
                ||
                Mathf.Abs(this.transform.localPosition.z) > 70 ||
                Mathf.Abs(this.transform.localPosition.x) > 70
               )
            {

                SetReward(-30f);
                resetTarget();
                timer = 0;
                t = 0;
                DDDD.Done();

                Done();

            }
        }
        if (this.transform.position.y < -3.0 ||
                this.transform.position.y > 60
                ||
                Mathf.Abs(this.transform.localPosition.z) > 70 ||
                Mathf.Abs(this.transform.localPosition.x) > 70
               )
        {
            resetTarget();
            timer = 0;
            t = 0;
            DDDD.Done();

            Done();

        }

        //gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(directionZ*100, directionY* 100, directionX* 100));



        //print(relativeRotation);
        //if (relativeRotation < 0 && Mathf.Abs(relativeRotation) > 3)
        //{
        //    DM.Rotation(-0.3f);
        //}
        //else if (relativeRotation > 0 && Mathf.Abs(relativeRotation) > 3)
        //{
        //    DM.Rotation(0.3f);
        //}
        //if (Target.transform.position.y > this.transform.position.y)
        //{
        //    DM.PropellerForce(directionZ, 0, 0.5f);
        //}
        //else if (Target.transform.position.y < this.transform.position.y)
        //{
        //    DM.PropellerForce(directionZ, 0, 0.3f);
        //}
        //print(DM.AutoMove + " " + directionZ + " " + directionX);



        //if (timer > 30)
        //{
        //    AddReward(-0.0000001f * Mathf.Pow((timer - 30), 1.5f));

        //}


        //AddReward(-0.0001f*timer);

        if(timer2 > 60){
            timer2 = 0;
            Done();
            resetTarget();
        }

        //print(rBody.velocity);
		previousDistance = distanceToTarget;
        previousRotation = absReRo;
        //print(timer);
        //print(GetReward());
        print(counter + " " + goalCounter + " " + TotalTime+" "+GetReward());
	}
    void resetTarget(){
        DDDD.enabled = true;
        int xx = Random.Range(0, 10);
        if(xx % 2 == 0){
            goal.localPosition = new Vector3(Random.Range(-50, 50), Random.Range(1, 10), Random.Range(10, 50));
        }else{
            goal.localPosition = new Vector3(Random.Range(-50, 50), Random.Range(1, 10), Random.Range(-50, -10));
        }

        goal.rotation = Quaternion.Euler(new Vector3(-90, 0, Random.value * 360));
        Target.localPosition = new Vector3(goal.localPosition.x , goal.localPosition.y+1.5f, goal.localPosition.z );
        for (int i = 0; i < Block.Length; i++)
        {
            //Block[i].localScale = new Vector3(Random.value * 10 + 3, Random.value * 300, Random.value * 10 + 3);
            if (i % 2 == 0)
            {
                Block[i].localPosition = new Vector3(Random.Range(-50, 50), 0, Random.Range(20, 50));
            }
            else
            {
                Block[i].localPosition = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, -20));
            }

            Block[i].rotation = Quaternion.Euler(new Vector3(0, Random.value * 360, 0));

        }
        previousDistance = Vector3.Distance(this.transform.localPosition,Target.localPosition);


    }
	private void OnCollisionEnter(Collision other)
	{
        if (other.gameObject.CompareTag("Block"))
        {

            //SetReward(-30f);
            //resetTarget();
            DDDD.Done();
            Done();
            timer = 0;

        }
        if (other.gameObject.CompareTag("Ground"))
        {
            if(timer > 30){
                resetTarget();
                DDDD.Done();
                Done();
            }
            AddReward(-0.5f);
            //resetTarget();
            //Done();
            //timer = 0;
        }
	}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            //DM.PropellerForce(0, 0, -1);
            //DM.MovemenForward(0, 0, -1);
            //DM.Swerve(0, 0);

            //Destroy(DDDD);
            DDDD.enabled = false;

        }
        if (other.gameObject.CompareTag("goal"))
        {
            //DM.PropellerForce(0, 0, 0);
            //DM.MovemenForward(0, 0, 0);
            DM.Swerve(0, 0);
                t += Time.deltaTime;
                AddReward(0.1f*t);

                if (t > 5)
                {
                    AddReward(30.0f);
                    resetTarget();
                    DDDD.Done();
                    Done();
                    TotalTime += timer;
                    timer = 0;
                    t = 0;
                    //goalCounter += 1;
                }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            AddReward(-0.01f);

        }
    }
}
