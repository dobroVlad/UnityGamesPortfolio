using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {

    [System.Serializable]
    public abstract class Node {
        public enum NodeState {
            Running,
            Failure,
            Success
        }
        [HideInInspector] public NodeState state = NodeState.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid = System.Guid.NewGuid().ToString();
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Context context;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string description;
        public bool drawGizmos = false;

        public NodeState Update() {

            if (!started) {
                OnStart();
                started = true;
            }
            state = OnUpdate();

            if (state != NodeState.Running) {
                OnStop();
                started = false;
            }
            return state;
        }
        public void Abort() {
            BehaviourTree.Traverse(this, (node) => {
                node.started = false;
                node.state = NodeState.Running;
                node.OnStop();
            });
        }
        protected abstract void OnStart();
        protected abstract NodeState OnUpdate();
        protected abstract void OnStop();
        public virtual void OnDrawGizmos() { }
    }
}