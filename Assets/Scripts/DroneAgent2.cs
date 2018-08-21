using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAgent2 : Agent {
	float planeSize = 30f;
    float planeW = 35;
    float planeL = 110;
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
	void Start () {
		rBody = GetComponent<Rigidbody>();
        DM = this.gameObject.GetComponent<DroneMove>();

        resetTarget();
        StartX = this.transform.localPosition.x;
        StartZ = this.transform.localPosition.z;

        DM.AutoMove = true;
	}

	public Transform Target;
    public Transform [] Block;
	public override void AgentReset()
	{
        this.transform.localPosition = new Vector3(StartX, 7f, StartZ);
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


        AddVectorObs(velocity.x/15);
        AddVectorObs(velocity.y/15);
        AddVectorObs(velocity.z/15);
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

        Vector3 relativePosition = Target.transform.localPosition - this.transform.localPosition;
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
                                                  Target.localPosition);


        //print(relativeRotation);
        //if (relativeRotation < 0 && Mathf.Abs(relativeRotation) > 3)
        //{
        //    DM.Rotation(-0.1f);
        //}
        //else if (relativeRotation > 0 && Mathf.Abs(relativeRotation) > 3)
        //{
        //    DM.Rotation(0.1f);
        //}
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
        //if(absReRo < previousRotation){
        //    AddReward(0.01f);
        //}
        //if (absReRo < 1)
        //{
        //    AddReward(0.1f);
        //}else if ((relativeRotation < 0 && relativeRotation > -180) || relativeRotation > 180)
        //{
        //    if(rotataionY < 0){
        //        DM.Rotation(rotataionY);
        //    }

        //}else if((relativeRotation > 0 && relativeRotation < 180) || relativeRotation < -180){
        //    if (rotataionY > 0)
        //    {
        //        DM.Rotation(rotataionY);
        //    }
        //}


        //if (distanceToTarget < previousDistance)
        //{
        //    AddReward(0.001f * Mathf.Pow(Mathf.Pow(rBody.velocity.z, 2) + Mathf.Pow(rBody.velocity.x, 2), 0.5f));
        //    if (5f < previousDistance - distanceToTarget)
        //    {
        //        AddReward(0.005f);
        //    }

        //}

        if(this.transform.position.y > 200){
            AddReward(-0.01f * (this.transform.position.y - 200 + 1));
        }

        AddReward(0.1f / (distanceToTarget + 1));

        //if(distanceToTarget > previousDistance){
        //    AddReward(-0.1f * (distanceToTarget - previousDistance));
        //}
        AddReward(-0.01f * (distanceToTarget - previousDistance));


        AddReward(-0.0001f*timer);

        //if(absReRo > 90){
        //    AddReward(-0.001f * absReRo);
        //}
     
        if (this.transform.position.y < 1)
        {
            AddReward(-0.5f);
        }



        //if (absReRo < previousRotation)
        //{
        //    AddReward(0.0001f);
        //}
        //if (absReRo > previousRotation)
        //{
        //    AddReward(-0.0001f);
        //}
         
        //if(distanceToTarget > previousDistance){
        //    AddReward(-0.003f);
        //}


        if (this.transform.position.y < -1.0 ||
            this.transform.position.y > 200
            ||
            Mathf.Abs(this.transform.localPosition.z) > planeL||
            Mathf.Abs(this.transform.localPosition.x) > planeW+5||
            this.transform.localPosition.z < -10
           )
		{
            
            SetReward(-3f);
            resetTarget();
            timer = 0;
            t = 0;
            Done();
		}
        if (timer > 35)
        {
            resetTarget();
            timer = 0;
            t = 0;
            SetReward(-3f);
            Done();

        }
        //print(rBody.velocity);
		previousDistance = distanceToTarget;
        previousRotation = absReRo;
        //print(timer);
        //print(GetReward());
        print(counter + " " + goalCounter);
	}
    void resetTarget(){
        Target.localPosition = new Vector3(Random.value * planeW - 17, Random.value * 50 + 5, planeL-20);
        for (int i = 0; i < Block.Length; i++){
            Block[i].localScale = new Vector3(Random.value * 10 + 3, Random.value * 300, Random.value * 10 + 3);
            Block[i].localPosition = new Vector3(Random.Range(-35, 35), Block[i].localScale.y/2,Random.value * 50 + 20);

        }
        previousDistance = Vector3.Distance(this.transform.localPosition,Target.localPosition);
        counter += 1;
    }
	//private void OnCollisionEnter(Collision other)
	//{
 //       if (other.gameObject.tag == "Block")
 //       {

 //           SetReward(-1f);
 //           resetTarget();
 //           Done();
 //           timer = 0;
 //       }
	//}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            
            if (other.gameObject.tag == "Target")
            {
                t += Time.deltaTime;
                AddReward(0.01f*t);

                if (t > 5)
                {
                    SetReward(3.0f);
                    resetTarget();
                    Done();
                    timer = 0;
                    t = 0;
                    goalCounter += 1;
                }

            }
        }
    }
	private void OnTriggerEnter(Collider other)
	{
       
        if (other.gameObject.tag == "Block")
        {
            
            SetReward(-3.0f);

            resetTarget();
            Done();
            timer = 0;
            t = 0;
        }
	}
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            AddReward(-0.1f);

        }
    }
}
