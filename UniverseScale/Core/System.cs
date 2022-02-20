using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale.Core
{
    public class System
    {
        public readonly string Name;
        public readonly float ReferenceBodyARSize;
        public readonly string PhysicalUnit;
        public readonly BodyData ReferenceBody;
        public readonly float TimePerSecond;

        public readonly decimal Scale;
        public List<BodyData> Bodies;

        public System(string _Name, float _ReferenceBodyARSize, string _PhysicalUnit, float _TimePerSecond, BodyData _ReferenceBody)
        {
            Name = _Name;
            ReferenceBodyARSize = _ReferenceBodyARSize;
            PhysicalUnit = _PhysicalUnit;
            TimePerSecond = _TimePerSecond;
            ReferenceBody = _ReferenceBody;

            Scale = 1 / ReferenceBody.PhysicalSize * (decimal)ReferenceBodyARSize;
            ReferenceBody.ParentSystem = this;

            Bodies = new List<BodyData>();
            Bodies.Add(ReferenceBody);
        }

        public System AddBody(BodyData SystemBody)
        {
            SystemBody.ParentSystem = this;
            Bodies.Add(SystemBody);
            return this;
        }
        public System AddBodies(List<BodyData> SystemBodies)
        {
            foreach(BodyData SystemBody in SystemBodies)
            {
                SystemBody.ParentSystem = this;
            }
            Bodies.AddRange(SystemBodies);
            return this;
        }
    }
}