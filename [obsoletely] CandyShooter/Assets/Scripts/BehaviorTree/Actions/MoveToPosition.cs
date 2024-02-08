using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myBehaviorTree;

[System.Serializable]
public class MoveToPosition : ActionNode {
    public float speed = 5;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.destination = blackboard.moveToPosition;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() {
    }

    protected override NodeState OnUpdate() {
        if (context.agent.pathPending) {
            return NodeState.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return NodeState.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return NodeState.Failure;
        }

        return NodeState.Running;
    }
}
