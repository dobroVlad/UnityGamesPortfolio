using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    [System.Serializable]
    public class Log : ActionNode
    {
        public string message;

        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override NodeState OnUpdate() {
            Debug.Log($"{message}");
            return NodeState.Success;
        }
    }
}
