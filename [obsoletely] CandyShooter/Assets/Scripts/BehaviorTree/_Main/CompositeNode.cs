using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myBehaviorTree {

    [System.Serializable]
    public abstract class CompositeNode : Node {

        [HideInInspector] 
        [SerializeReference]
        public List<Node> children = new List<Node>();
    }
}