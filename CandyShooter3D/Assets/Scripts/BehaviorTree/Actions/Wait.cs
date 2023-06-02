using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {

    [System.Serializable]
    public class Wait : ActionNode {

        public float duration = 1;
        float startTime;

        protected override void OnStart() {
            startTime = Time.time;
        }

        protected override void OnStop() {
        }

        protected override NodeState OnUpdate() {
            
            float timeRemaining = Time.time - startTime;
            if (timeRemaining > duration) {
                return NodeState.Success;
            }
            return NodeState.Running;
        }
    }
}
