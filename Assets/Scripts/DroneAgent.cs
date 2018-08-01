using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAgent : Agent {
	float planeSize = 30f;
    float planeW = 35;
    float planeL = 100;
	private float previousDistance = float.MaxValue;
    private float previousRotation = 0;
    DroneMove DM;
	Rigidbody rBody;
    float StartX;
    float StartZ;
    float timer = 0f;


	void Start () {
		rBody = GetComponent<Rigidbody>();
        DM = this.gameObject.GetComponent<DroneMove>();

        resetTarget();
        StartX = this.transform.localPosition.x;
        StartZ = this.transform.localPosition.z;

        DM.MLAgent = true;
	}

	public Transform Target;
    public Transform [] Block;
	public override void AgentReset()
	{
        this.transform.localPosition = new Vector3(StartX, 7f, StartZ);
        DM.SetRotation(0, (Random.Range(-2, 2))*90, 0);
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

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
        //directionX = Mathf.Clamp(vectorAction[0], -1f, 1f);
        //print(DM.MLAgent +" "+ directionZ + " " + directionX);
        DM.Rotation(rotataionY);
        //DM.Swerve(directionZ, directionX, directionY);
        DM.PropellerForce(directionZ, directionX, directionY);
        DM.MovemenForward(directionZ, directionX, directionY);
        //gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(directionZ*100, directionY* 100, directionX* 100));
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, 
                                                  Target.localPosition);


        //print(relativeRotation);
		


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

        if (distanceToTarget < previousDistance)
        {
            AddReward(0.01f);
            if (1f < previousDistance - distanceToTarget)
            {
                AddReward(0.05f);
            }
            if(Mathf.Pow(Mathf.Pow(rBody.velocity.z, 2) + Mathf.Pow(rBody.velocity.x, 2),0.5f)  > 5){
                AddReward(0.01f);
            }
        }



        AddReward(-0.003f);

     
        if (this.transform.position.y < 1)
        {
            AddReward(-0.5f);
        }
       


        //if (absReRo > previousRotation)
        //{
        //    AddReward(-0.01f);
        //}

         
        if(distanceToTarget > previousDistance){
            AddReward(-0.08f);
        }


        if (this.transform.position.y < -1.0 ||
            this.transform.position.y > 100 ||
            Mathf.Abs(this.transform.localPosition.z) > planeL||
            Mathf.Abs(this.transform.localPosition.x) > planeW||
            this.transform.localPosition.z < -10)
		{
            
            AddReward(-1f);
            resetTarget();
            timer = 0;
            Done();
		}
        if (timer > 30)
        {
            resetTarget();
            timer = 0;
            AddReward(-1f);
            Done();

        }
        //print(rBody.velocity);
		previousDistance = distanceToTarget;
        previousRotation = absReRo;
        print(timer);

	}
    void resetTarget(){
        Target.localPosition = new Vector3(Random.value * planeW - 17, Random.value * 50 + 5, planeL-15);
        for (int i = 0; i < Block.Length; i++){          
            Block[i].localPosition = new Vector3(Random.Range(-35, 35), 0,Random.value * 50 + 20);
            Block[i].localScale = new Vector3(Random.value * 10 + 3, Random.value * 200, Random.value * 10 + 3);
        }
    }
	private void OnTriggerEnter(Collider other)
	{
        if(other.gameObject.tag == "Target"){
            AddReward(1.0f);
            resetTarget();
            Done();
            timer = 0;
        }
        if (other.gameObject.tag == "Block")
        {
            AddReward(-1.0f);
            resetTarget();
            Done();
            timer = 0;
        }
	}

}
