using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myBehaviorTree;

[System.Serializable]
public class Breakpoint : ActionNode
{
    protected override void OnStart() {
        Debug.Log("Trigging Breakpoint");
        Debug.Break();
    }

    protected override void OnStop() {
    }

    protected override NodeState OnUpdate() {
        return NodeState.Success;
    }
}
