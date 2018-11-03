using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAgentOutDoor : Agent {
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
    int goalCounter = 0;
    int counter = 0;
    float t = 0;
    float TotalTime = 0;
	void Start () {
		rBody = GetComponent<Rigidbody>();
        DM = this.gameObject.GetComponent<DroneMove>();

        resetTarget();
        StartX = this.transform.localPosition.x;
        StartZ = this.transform.localPosition.z;

        DM.AutoMove = true;
	}

	public Transform Target;
    public Transform goal;
    public Transform [] Block;
	public override void AgentReset()
	{
        this.transform.localPosition = new Vector3(StartX, 1f, StartZ);
        DM.SetRotation(0, Random.value * 360, 0);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        previousDistance = 110;

	}
	public override void CollectObservations()
	{
		
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
        Vector3 relativePosition = Target.localPosition - this.transform.localPosition;
        Vector3 Rotation = transform.rotation.eulerAngles / 180.0f - Vector3.one;
        float arcTanToTar = Mathf.Atan(relativePosition.z / relativePosition.x) * 180 / Mathf.PI;
        float relativeRotation = 0;

        if (relativePosition.x > 0 && relativePosition.z > 0)
        {
            relativeRotation = (90 - (arcTanToTar));
        }
        else if (relativePosition.x < 0 && relativePosition.z > 0)
        {
            relativeRotation = (90 - (arcTanToTar)) + 180;
        }
        else if (relativePosition.x < 0 && relativePosition.z < 0)
        {
            relativeRotation = (90 - (arcTanToTar)) + 180;
        }
        else if (relativePosition.x > 0 && relativePosition.z < 0)
        {
            relativeRotation = (90 - (arcTanToTar));
        }
        relativeRotation = relativeRotation - this.transform.rotation.eulerAngles.y;
        if (relativeRotation > 180)
        {
            relativeRotation = relativeRotation - 360;
        }
        else if (relativeRotation < -180)
        {
            relativeRotation = relativeRotation + 360;
        }
        relativeRotation = Mathf.Abs(relativeRotation) / 180 - 1;

        AddVectorObs((transform.localPosition.x)/100);
        AddVectorObs((transform.localPosition.y)/100);
        AddVectorObs((transform.localPosition.z)/100);

        AddVectorObs(relativePosition.x/100);
        AddVectorObs(relativePosition.y/100);
        AddVectorObs(relativePosition.z/100);


        AddVectorObs(velocity.x/100);
        AddVectorObs(velocity.y/100);
        AddVectorObs(velocity.z/100);
        AddVectorObs(Rotation);
        AddVectorObs(relativeRotation);
        //for (int i = 0; i < Block.Length; i++)
        //{
        //    AddVectorObs(Block[i].localPosition.x/200);
        //    AddVectorObs(Block[i].localPosition.y/200);
        //    AddVectorObs(Block[i].localPosition.z/200);
        //    AddVectorObs(Block[i].localScale.x/200);
        //    AddVectorObs(Block[i].localScale.y/200);
        //    AddVectorObs(Block[i].localScale.z/200);
        //}
        //print(relativeRotation + " " + ((relativeRotation+1)*180));
        //AddVectorObs(DM.rotationXVelocity/10);
        //AddVectorObs(DM.rotationYVelocity/10);
        //AddVectorObs(DM.rotationZVelocity/10);
	}
	public override void AgentAction(float[] vectorAction, string textAction)
	{

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
        timer += Time.deltaTime;


        float directionX = 0;
        float directionZ = 0;
        float directionY = 0;
        float rotataionY = 0;

        rotataionY = vectorAction[0];
        directionY = Mathf.Clamp(vectorAction[2], -1f, 1f);
        directionZ = Mathf.Clamp(vectorAction[1], -1f, 1f);
        directionX = Mathf.Clamp(vectorAction[3], -1f, 1f);

        //gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(directionZ*100, directionY* 100, directionX* 100));
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, 
                                                  goal.localPosition);


        //print(relativeRotation);
        if (relativeRotation < 0 && Mathf.Abs(relativeRotation) > 3)
        {
            DM.Rotation(-0.3f);
        }
        else if (relativeRotation > 0 && Mathf.Abs(relativeRotation) > 3)
        {
            DM.Rotation(0.3f);
        }
        if (Target.transform.position.y > this.transform.position.y)
        {
            DM.PropellerForce(directionZ, 0, 0.5f);
        }
        else if (Target.transform.position.y < this.transform.position.y)
        {
            DM.PropellerForce(directionZ, 0, 0.3f);
        }
        //print(DM.AutoMove + " " + directionZ + " " + directionX);



        DM.Rotation(rotataionY);

        DM.PropellerForce(directionZ, directionX, directionY);
        DM.MovemenForward(directionZ, directionX, directionY);
        DM.Swerve(directionZ, directionX);
       

        AddReward(0.1f / (distanceToTarget + 1));



        AddReward(-0.0001f*timer);

       
     
        //if (this.transform.position.y < 0.1f)
        //{
        //    AddReward(-0.5f);
        //}
        if(absReRo > 90){
            AddReward(-0.001f*absReRo);
        }

         
        //if(distanceToTarget > previousDistance){
        //    AddReward(-0.003f);
        //}

        if (this.transform.position.y < -1.0 ||
            this.transform.position.y > 60
            ||
            Mathf.Abs(this.transform.localPosition.z) > 50 ||
            Mathf.Abs(this.transform.localPosition.x) > 50 
           )
        {

            SetReward(-30f);
            resetTarget();
            timer = 0;
            t = 0;
            Done();
        }
        if (timer > 30)
        {
            //AddReward(-0.00001f * Mathf.Pow((timer - 30),2));

        }
        //print(rBody.velocity);
		previousDistance = distanceToTarget;
        previousRotation = absReRo;
        //print(timer);
        //print(GetReward());
        print(counter + " " + goalCounter + " " + TotalTime+" "+GetReward());
	}
    void resetTarget(){
        goal.localPosition = new Vector3(Random.Range(-35, 35), Random.Range(1, 10), Random.Range(-35, 35));
        goal.rotation = Quaternion.Euler(new Vector3(-90, 0, Random.value * 360));
        Target.localPosition = new Vector3(goal.localPosition.x , goal.localPosition.y+0.8f, goal.localPosition.z );
        for (int i = 0; i < Block.Length; i++){
            Block[i].localPosition = new Vector3(Random.Range(-50, 50),0,Random.value * 100);
            Block[i].rotation = Quaternion.Euler(new Vector3(0, Random.value * 360, 0));
        }
        previousDistance = Vector3.Distance(this.transform.localPosition,Target.localPosition);
        counter += 1;


    }
	private void OnCollisionEnter(Collision other)
	{
        if (other.gameObject.tag == "Block")
        {

            SetReward(-30f);
            resetTarget();
            Done();
            timer = 0;
        }
        if (other.gameObject.tag == "Ground")
        {

            AddReward(-0.5f);
            //resetTarget();
            //Done();
            //timer = 0;
        }
	}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            DM.PropellerForce(0, 0, -1);
            DM.MovemenForward(0, 0, -1);
            DM.Swerve(0, 0);

        }
        if (other.gameObject.tag == "goal")
        {
            DM.PropellerForce(0, 0, 0);
            DM.MovemenForward(0, 0, 0);
            DM.Swerve(0, 0);
                t += Time.deltaTime;
                AddReward(0.1f*t);

                if (t > 5)
                {
                    AddReward(30.0f);
                    resetTarget();
                    Done();
                    TotalTime += timer;
                    timer = 0;
                    t = 0;
                    goalCounter += 1;
                }

        }
    }
	//private void OnTriggerEnter(Collider other)
	//{
       
 //       if (other.gameObject.tag == "Block")
 //       {
            
 //           SetReward(-5.0f);

 //           resetTarget();
 //           Done();
 //           timer = 0;
 //           t = 0;
 //       }
	//}
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "goal")
        {
            AddReward(-0.01f);

        }
    }
}
