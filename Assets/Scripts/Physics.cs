using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using JetBrains.Annotations;

public class CarAgent : Agent
{
    public Rigidbody rb;
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    private bool alreadyPenalized = false;

    public override void Initialize()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0, -6);
        alreadyPenalized = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float turn = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Előre/hátra mozgatás
        rb.MovePosition(rb.position + transform.forward * move * moveSpeed * Time.fixedDeltaTime);

        // Forgás (y tengelyen)
        Quaternion turnRotation = Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
        AddReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var contActions = actionsOut.ContinuousActions;
        contActions[0] = Input.GetAxis("Vertical");   // előre-hátra
        contActions[1] = Input.GetAxis("Horizontal"); // balra-jobbra
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!alreadyPenalized && (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Car")))
        {
            alreadyPenalized = true;
            Debug.Log("Penalized collision");
            AddReward(-15f);
            EndEpisode();
        }
        else if(collision.collider.CompareTag("Bollard"))
        {
            Debug.Log("Bollard Collision");
            AddReward(+5f);
            EndEpisode();
        }
    }
}
