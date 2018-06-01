using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : Agent {
	float planeSize = 16f;
	private float previousDistance = float.MaxValue;
	Rigidbody rBody;
	void Start () {
		rBody = GetComponent<Rigidbody>();
	}

	public Transform Target;
	public override void AgentReset()
	{
		if (this.transform.position.y < -1.0)
		{  
			// agent 掉落
			this.transform.position = Vector3.zero;
			this.rBody.angularVelocity = Vector3.zero;
			this.rBody.velocity = Vector3.zero;
		}
		else
		{ 
			// 将目标移动到新的位置
			Target.position = new Vector3(Random.value * planeSize - 4,
				Random.value * planeSize,
				Random.value * planeSize - 4);
		}
	}
	public override List<float> CollectState()
	{
		List<float> state = new List<float>();
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 relativePosition = Target.position - this.transform.position;
		state.Add((transform.position.x));
		state.Add((transform.position.y));
		state.Add((transform.position.z));

		state.Add(relativePosition.x);
		state.Add(relativePosition.y);
		state.Add(relativePosition.z);


		state.Add(velocity.x);
		state.Add(velocity.y);
		state.Add(velocity.z);

		return state;
	}
	public override void AgentStep(float[] act)
	{
		float distanceToTarget = Vector3.Distance(this.transform.position, 
			Target.position);

		// 已到达目标
		if (distanceToTarget < 1f)
		{
			done = true;
			reward = 1.0f;
		}

		// 进一步接近
		if (distanceToTarget < previousDistance)
		{
			reward = 0.1f;
		}

		// 时间惩罚
		reward = -0.05f;

		// 掉下平台
		if (this.transform.position.y < -1.0)
		{
			done = true;
			reward = -1f;
		}
		previousDistance = distanceToTarget;
	}

}
