using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace myBehaviorTree {
    public class SplitView : TwoPaneSplitView {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}