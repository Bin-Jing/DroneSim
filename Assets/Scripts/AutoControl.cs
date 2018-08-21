using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoControl : MonoBehaviour {
    public GameObject Target;
    float relativeRotation = 0;
    Rigidbody _rigidbody;
    DroneMove DM;
    float ver = 0;
	// Use this for initialization
	void Awake () {
        DM = this.gameObject.GetComponent<DroneMove>();
        _rigidbody = GetComponent<Rigidbody>();
        DM.AutoMove = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateRelativeRotation();
        MoveDrone();
	}
	private void UpdateRelativeRotation()
	{
        Vector3 relativePosition = Target.transform.localPosition - this.transform.localPosition;
        float arcTanToTar = Mathf.Atan(relativePosition.z / relativePosition.x) * 180 / Mathf.PI;

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
        if (relativeRotation < 0 && Mathf.Abs(relativeRotation) > 1)
        {
            DM.Rotation(-0.1f);
        }
        else if (relativeRotation > 0 && Mathf.Abs(relativeRotation) > 1)
        {
            DM.Rotation(0.1f);
        }

	}
    private void MoveDrone(){
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,Target.transform.localPosition);
        if(Target.transform.position.y > this.transform.position.y){
            DM.PropellerForce(0, 0, 0.8f);
        }else if(Target.transform.position.y < this.transform.position.y){
            DM.PropellerForce(0, 0, 0.3f);
        }
        if(Mathf.Abs(Target.transform.position.y - this.transform.position.y) < 1 && Mathf.Abs(relativeRotation) < 1){
            DM.PropellerForce(0.5f);
            DM.MovemenForward(0.5f);
        }
        if(distanceToTarget < 10){
            brake();
        }

    }
    void brake(){
        if(this.transform.rotation.eulerAngles.x > 0){
            DM.MovemenForward(-0.1f);
        }else  if (this.transform.rotation.eulerAngles.x < 0){
            DM.MovemenForward(0.1f);
        }else{
            DM.MovemenForward(0);
        }
        if(Mathf.Pow(Mathf.Pow(_rigidbody.velocity.z, 2) + Mathf.Pow(_rigidbody.velocity.x, 2), 0.5f) > 1){
            DM.PropellerForce(-1f);
            DM.MovemenForward(-1f);
        }else{
            DM.PropellerForce(0, 0, 0);
            DM.MovemenForward(0);
        }

    }
}
