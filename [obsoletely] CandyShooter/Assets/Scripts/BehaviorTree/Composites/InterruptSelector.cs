using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    [System.Serializable]
    public class InterruptSelector : Selector {
        protected override NodeState OnUpdate() {
            int previous = current;
            base.OnStart();
            var status = base.OnUpdate();
            if (previous != current) {
                if (children[previous].state == NodeState.Running) {
                    children[previous].Abort();
                }
            }

            return status;
        }
    }
}