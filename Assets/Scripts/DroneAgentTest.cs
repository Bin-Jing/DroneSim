﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DroneAgentTest : Agent
{
    float planeSize = 30f;
    float planeW = 100;
    float planeL = 100;
    private float previousDistance = 110;
    private float previousRotation = 0;
    DroneMove DM;
    Rigidbody rBody;
    float StartX;
    float StartZ;
    float timer = 0f;
    float t = 0;
    public WriteLoadJSON dataList;
    public bool notTraining = false;
    int tarpos = 0;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        DM = this.gameObject.GetComponent<DroneMove>();

        resetTarget();
        StartX = this.transform.localPosition.x;
        StartZ = this.transform.localPosition.z;

        DM.AutoMove = true;
    }

    public Transform Target;
    public Transform[] Block;
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

        AddVectorObs((transform.localPosition.x) / 100);
        AddVectorObs((transform.localPosition.y) / 100);
        AddVectorObs((transform.localPosition.z) / 100);

        AddVectorObs(relativePosition.x / 100);
        AddVectorObs(relativePosition.y / 100);
        AddVectorObs(relativePosition.z / 100);


        AddVectorObs(velocity.x / 15);
        AddVectorObs(velocity.y / 15);
        AddVectorObs(velocity.z / 15);
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
        float absReRo = Mathf.Abs(relativeRotation);



        float directionX = 0;
        float directionZ = 0;
        float directionY = 0;
        float rotataionY = 0;

        rotataionY = vectorAction[0];
        directionY = Mathf.Clamp(vectorAction[2], -1f, 1f);
        directionZ = Mathf.Clamp(vectorAction[1], -1f, 1f);
        //directionX = Mathf.Clamp(vectorAction[3], -1f, 1f);
        //print(" "+ directionZ + " " + directionX);

        //gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(directionZ*100, directionY* 100, directionX* 100));
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target.localPosition);


        //print(relativeRotation);
        DM.Rotation(rotataionY);

        DM.PropellerForce(directionZ, directionX, directionY);
        DM.Swerve(directionZ, directionX, directionY);
        DM.MovemenForward(directionZ, directionX, directionY);
        if (relativeRotation < 0 && Mathf.Abs(relativeRotation) > 3)
        {
            DM.Rotation(-0.3f);
        }
        else if (relativeRotation > 0 && Mathf.Abs(relativeRotation) > 3)
        {
            DM.Rotation(0.3f);
        }
        if (Target.transform.position.y > this.transform.position.y && Mathf.Abs(Target.transform.position.y - this.transform.position.y) > 0.3f)
        {
            DM.PropellerForce(directionZ, 0, 0.6f);
        }
        else if (Target.transform.position.y < this.transform.position.y && Mathf.Abs(Target.transform.position.y - this.transform.position.y) > 0.3f)
        {
            DM.PropellerForce(directionZ, 0, 0.3f);
        }else{
            DM.PropellerForce(directionZ, 0, 0.412f);
        }




        //if (distanceToTarget < previousDistance)
        //{
        //    AddReward(0.001f * Mathf.Pow(Mathf.Pow(rBody.velocity.z, 2) + Mathf.Pow(rBody.velocity.x, 2), 0.5f));
        //    //if (2f < previousDistance - distanceToTarget)
        //    //{
        //    //    AddReward(0.0005f);
        //    //}

        //}

        AddReward(0.01f / (distanceToTarget + 1));
        //print(distanceToTarget);

        AddReward(-0.001f * absReRo);

        if (this.transform.position.y < 1)
        {
            AddReward(-0.5f);
        }


       AddReward(-0.01f * (distanceToTarget - previousDistance));




        //if ((this.transform.position.y < -1.0 ||
        //    this.transform.position.y > 100
        //    ||
        //    Mathf.Abs(this.transform.localPosition.z) > planeL/2 + 5 ||
        //    Mathf.Abs(this.transform.localPosition.x) > planeW/2 + 5 
        //   )&& !notTraining)
        //{

        //    SetReward(-1f);
        //    resetTarget();
        //    timer = 0;
        //    Done();
        //}
        //if (t > 35 && !notTraining)
        //{
        //    resetTarget();
        //    timer = 0;
        //    t = 0;
        //    SetReward(-1f);
        //    Done();

        //}
        //print(rBody.velocity);
        previousDistance = distanceToTarget;
        previousRotation = absReRo;
        //print(timer);
        //print(GetReward());
        t += Time.deltaTime;
        //print(t + " " + distanceToTarget);
    }
    void resetTarget()
    {
        //Target.localPosition = new Vector3(Random.value * 100 - 50, Random.value * 50 + 5,Random.value * 100-50);
        //for (int i = 0; i < Block.Length; i++)
        //{
        //    Block[i].localPosition = new Vector3(Random.Range(-30, 30), 0, Random.value * 50 + 20);
        //    Block[i].localScale = new Vector3(Random.value * 10 + 3, Random.value * 300, Random.value * 10 + 3);
        //}
        dataList.LoadTargetGPS(tarpos);
        previousDistance = Vector3.Distance(this.transform.localPosition, Target.localPosition);
    }
	//private void OnCollisionEnter(Collision other)
	//{
	//    if (other.gameObject.tag == "Block")
	//    {

	//        AddReward(-1f);
	//        resetTarget();
	//        Done();
	//        timer = 0;
	//    }
	//}
	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "Target")
        {
            resetTarget();

            if(tarpos == dataList.numberOfTar-1){
                tarpos = 0;
            }else{
                tarpos += 1;
            }
        }
	}
	private void OnTriggerStay(Collider other)
	{
        if (other.gameObject.tag == "Target")
        {
            //if (Target.transform.position.y > this.transform.position.y)
            //{
            //    DM.PropellerForce(0, 0, 0.5f);
            //}
            //else if (Target.transform.position.y < this.transform.position.y)
            //{
            //    DM.PropellerForce(0, 0, 0.3f);
            //}
            DM.MovemenForward();
            DM.SetRotation(0, this.transform.rotation.eulerAngles.y, 0);
            AddReward(0.05f);
            timer += Time.deltaTime;
            //if (timer > 5 && !notTraining)
            //{
            //    AddReward(1.0f);
            //    resetTarget();
            //    timer = 0;
            //    t = 0;
            //}

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
