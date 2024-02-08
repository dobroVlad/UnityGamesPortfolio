using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {

    [System.Serializable]
    public class RootNode : Node {

        [SerializeReference]
        [HideInInspector] 
        public Node child;

        protected override void OnStart() {

        }

        protected override void OnStop() {

        }

        protected override NodeState OnUpdate() {

            return child.Update();
        }
    }
}