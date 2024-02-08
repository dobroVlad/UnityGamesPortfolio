using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    [System.Serializable]
    public class RandomFailure : ActionNode {

        [Range(0,1)]
        public float chanceOfFailure = 0.5f;

        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override NodeState OnUpdate() {
            float value = Random.value;
            if (value > chanceOfFailure) {
                return NodeState.Failure;
            }
            return NodeState.Success;
        }
    }
}