using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    [System.Serializable]
    public class Succeed : DecoratorNode {
        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override NodeState OnUpdate() {
            var state = child.Update();
            if (state == NodeState.Failure) {
                return NodeState.Success;
            }
            return state;
        }
    }
}