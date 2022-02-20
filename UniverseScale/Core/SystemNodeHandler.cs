using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

using UniverseScale.Core;
using System.Linq;

namespace UniverseScale.Core
{
    public class SystemNodeHandler
    {
        public SCNNode SystemNode;
        public readonly System SystemHandle;
        public StateHandler PinchState;
        public StateHandler RotateState;
        public float CurrentHeight = 0;
        public bool HeightReleased = true;
        public float SystemScale = 1;

        private List<BodyNodeHandler> Nodes = new List<BodyNodeHandler>();

        public SystemNodeHandler(System _System)
        {
            SystemHandle = _System;

            SystemNode = SCNNode.Create();
            SystemNode.Name = "System|" + SystemHandle.Name;
            PinchState = new StateHandler(1, 1);
            RotateState = new StateHandler(-1, 0);

            SCNNode ReferenceBodyNode = SystemHandle.ReferenceBody.CreateNode(true);
            SCNNode ReferenceOrbitNode = ReferenceBodyNode.ParentNode;
            SystemNode.AddChildNode(ReferenceOrbitNode);
            Nodes.Add(new BodyNodeHandler(this, SystemHandle.ReferenceBody, ReferenceBodyNode));
            foreach (BodyData Body in SystemHandle.Bodies)
            {
                if(Body != SystemHandle.ReferenceBody)
                {
                    SCNNode Node = Body.CreateNode(false);
                    ReferenceOrbitNode.AddChildNode(Node.ParentNode);
                    Nodes.Add(new BodyNodeHandler(this, Body, Node));
                }
            }
            if(SystemHandle.ReferenceBody.Selflit == false)
            {
                SCNNode LightNode = new SCNNode();
                LightNode.Light = SCNLight.Create();
                LightNode.CastsShadow = true;
                LightNode.Light.LightType = SCNLightType.Directional;
                LightNode.Position = new SCNVector3(0, 0, -0.5f);
                SystemNode.AddChildNode(LightNode);
            }
        }

        public void BeginMovement()
        {
            foreach (BodyNodeHandler BodyNode in Nodes)
            {
                BodyNode.BeginMovement();
            }
        }

        public BodyNodeHandler FindBodyNodeHandlerFromNode(SCNNode Node)
        {
            foreach (BodyNodeHandler BodyNode in Nodes)
            {
                if(BodyNode.Node == Node)
                {
                    return BodyNode;
                }
            }
            return null;
        }
        public BodyNodeHandler FindBodyNodeHandlerFromBody(BodyData Body)
        {
            foreach (BodyNodeHandler BodyNode in Nodes)
            {
                if (BodyNode.Body == Body)
                {
                    return BodyNode;
                }
            }
            return null;
        }

        public void RescaleSystem(float NewScaleMultiplier)
        {
            var Mult = Math.Min(Math.Max(PinchState.GetNew(NewScaleMultiplier), Settings.MinSystemScale), Settings.MaxSystemScale);
            SystemNode.Scale = new SCNVector3(Mult, Mult, Mult);
            SystemScale = Mult;
        }
        public void RotateSystem(float RadianRotation)
        {
            SystemNode.EulerAngles = new SCNVector3(0, RotateState.GetNew(RadianRotation), 0);
        }

        public void SetPositon(SCNVector3 Position, float? YOffset)
        {
            if (YOffset != null)
            {
                Position = SCNVector3.Add(Position, new SCNVector3(0, FindBodyNodeHandlerFromBody(SystemHandle.ReferenceBody).Body.Geometry.GetSize() + (float)YOffset, 0));
                CurrentHeight = (float)YOffset;
            }
            SystemNode.Position = Position;
        }
    }
}