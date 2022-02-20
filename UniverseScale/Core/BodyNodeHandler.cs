using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale.Core
{
    public class BodyNodeHandler
    {
        public readonly BodyData Body;
        public readonly SCNNode Node;
        public readonly SystemNodeHandler NodeHandler;
        public float DistanceScale = 1;
        public StateHandler PinchState;
        public StateHandler RotateState;

        public BodyNodeHandler(SystemNodeHandler _NodeHandler, BodyData _Body, SCNNode _Node)
        {
            NodeHandler = _NodeHandler;
            Body = _Body;
            Node = _Node;

            PinchState = new StateHandler(1, 1);
            RotateState = new StateHandler(-1, 0);
        }

        public void BeginMovement()
        {
            Body.Movement.PerformOnNode(Node);
        }

        public void RepositionNode()
        {
            Node.Position = new SCNVector3(0, 0, Body.ARLength(Body.PhysicalDistance) * DistanceScale);
            Body.SetBillboardPosition(Node);
        }

        public void ResizeNode(float NewScaleMultiplier)
        {
            var Mult = Math.Min(Math.Max(PinchState.GetNew(NewScaleMultiplier), Settings.MinBodyScale), Settings.MaxBodyScale);
            Node.Scale = new SCNVector3(Mult, Mult, Mult);
            Body.SetBillboardPosition(Node);
        }

        public void RotateOrbitNode(float RadianRotation)
        {
            Node.ParentNode.EulerAngles = new SCNVector3(0, RotateState.GetNew(RadianRotation), 0);
        }
    }
}