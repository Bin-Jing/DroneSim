using System.Collections.Generic;
using UnityEngine;

public class RollerAgent : Agent 
{
	Rigidbody rBody;
	void Start () {
		rBody = GetComponent<Rigidbody>();
	}

	public Transform Target;
	public override void AgentReset()
	{
		if (this.transform.position.y < -1.0)
		{  
			// The agent fell
			this.transform.position = Vector3.zero;
			this.rBody.angularVelocity = Vector3.zero;
			this.rBody.velocity = Vector3.zero;
		}
		else
		{ 
			// Move the target to a new spot
			Target.position = new Vector3(Random.value * 8 - 4,
				0.5f,
				Random.value * 8 - 4);
		}
	}
	public override List<float> CollectState()
	{
		List<float> state = new List<float>();
		Vector3 velocity = GetComponent<Rigidbody>().velocity;

		state.Add(velocity.x);
		state.Add(velocity.y);
		state.Add(velocity.z);


		return state;
	}

}