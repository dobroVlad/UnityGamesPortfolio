using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace myBehaviorTree {
    [System.Serializable]
    public class Parallel : CompositeNode {
        List<NodeState> childrenLeftToExecute = new List<NodeState>();

        protected override void OnStart() {
            childrenLeftToExecute.Clear();
            children.ForEach(a => {
                childrenLeftToExecute.Add(NodeState.Running);
            });
        }

        protected override void OnStop() {
        }

        protected override NodeState OnUpdate() {
            bool stillRunning = false;
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (childrenLeftToExecute[i] == NodeState.Running) {
                    var status = children[i].Update();
                    if (status == NodeState.Failure) {
                        AbortRunningChildren();
                        return NodeState.Failure;
                    }

                    if (status == NodeState.Running) {
                        stillRunning = true;
                    }

                    childrenLeftToExecute[i] = status;
                }
            }

            return stillRunning ? NodeState.Running : NodeState.Success;
        }

        void AbortRunningChildren() {
            for (int i = 0; i < childrenLeftToExecute.Count(); ++i) {
                if (childrenLeftToExecute[i] == NodeState.Running) {
                    children[i].Abort();
                }
            }
        }
    }
}