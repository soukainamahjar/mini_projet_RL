using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class RobotArmAgent : Agent
{
    public Transform targetCube;

    public override void OnEpisodeBegin()
    {
        // Remettre la position du bras au début
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        
        // Déplacer le cube à une position aléatoire
        targetCube.localPosition = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observer la position du bras
        sensor.AddObservation(this.transform.localPosition);

        // Observer la position du cube
        sensor.AddObservation(targetCube.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Déplacement du bras
        transform.localPosition += new Vector3(moveX, 0, moveZ) * 0.05f;

        // Récompense si proche du cube
        float distance = Vector3.Distance(this.transform.localPosition, targetCube.localPosition);
        if (distance < 0.3f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Pénalité si trop loin
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
