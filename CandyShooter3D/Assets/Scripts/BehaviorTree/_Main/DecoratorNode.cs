using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {
    public abstract class DecoratorNode : Node {

        [SerializeReference]
        [HideInInspector] 
        public Node child;
    }
}
