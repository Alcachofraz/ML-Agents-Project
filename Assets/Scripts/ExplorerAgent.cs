using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ExplorerAgent : Agent
{
    public class RewardInfo
    {
        public float Wall = -0.02f;
        public float Checkpoint = 0.02f;
        public float Fixed = -0.0002f; // (1 / MaxStep)
        public float Success = 1;
    }

    Rigidbody rb;
    RewardInfo rewardInfo;
    public event EventHandler OnEpisodeBeginEvent;
    public float speed = 1f;
    public bool endEpisodeOnCollisionWithBall = false;

    public void Immobilize()
    {
        rb.velocity = Vector3.zero;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rewardInfo = new RewardInfo();
    }

    public override void OnEpisodeBegin()
    {
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        /*rb.AddForce(new Vector3(-actions.ContinuousActions[0], 0, actions.ContinuousActions[1]) * speed);
        AddReward(rewardInfo.Fixed);*/

        foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(0.15f, 0f, 0.15f)))
        {
            if (collider.TryGetComponent<Wall>(out Wall wall))
            {
                AddReward(rewardInfo.Wall);
                break;
            }
        }

        rb.velocity = Vector3.zero;
        transform.position += new Vector3(-actions.ContinuousActions[0], 0f, actions.ContinuousActions[1]) * speed;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        AddReward(rewardInfo.Fixed);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        continuousActions[0] = vertical;
        continuousActions[1] = horizontal;
        //Debug.Log(vertical + ", " + horizontal);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball!");
            AddReward(rewardInfo.Success);
            if (endEpisodeOnCollisionWithBall) EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall!");
            AddReward(rewardInfo.Wall);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint!");
            AddReward(rewardInfo.Checkpoint);
            Destroy(collider.gameObject);
        }
    }
}
