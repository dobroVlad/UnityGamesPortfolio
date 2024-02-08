using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    [System.Serializable]
    public class Repeat : DecoratorNode {

        public bool restartOnSuccess = true;
        public bool restartOnFailure = false;

        protected override void OnStart() {

        }

        protected override void OnStop() {

        }

        protected override NodeState OnUpdate() {
            switch (child.Update()) {
                case NodeState.Running:
                    break;
                case NodeState.Failure:
                    if (restartOnFailure) {
                        return NodeState.Running;
                    } else {
                        return NodeState.Failure;
                    }
                case NodeState.Success:
                    if (restartOnSuccess) {
                        return NodeState.Running;
                    } else {
                        return NodeState.Success;
                    }
            }
            return NodeState.Running;
        }
    }

    
}
