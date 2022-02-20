using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;


namespace UniverseScale.Core
{
    public class BodyMovement
    {
        public BodyData ParentBody;

        public readonly decimal Rotation;
        public readonly decimal Revolution;

        public BodyMovement(float _Rotation, float _Revolution = 0, bool AutoConvert = true)
        {
            if (AutoConvert == true)
            {
                Rotation = TimeScale.RadianPeriod(_Rotation);
                Revolution = TimeScale.RadianPeriod(_Revolution);
            }
            else
            {
                Rotation = (decimal)_Rotation;
                Revolution = (decimal)_Revolution;
            }
        }
        public static BodyMovement Synchronous(float RotationAndRevolution)
        {
            return new BodyMovement(RotationAndRevolution, RotationAndRevolution);
        }

        public void SetParentBody(BodyData _ParentBody)
        {
            ParentBody = _ParentBody;
        }

        public void PerformOnNode(SCNNode BodyNode)
        {
            BodyNode.RunAction(SCNAction.RepeatActionForever(SCNAction.RotateBy(0, (float)(Rotation * (decimal)ParentBody.ParentSystem.TimePerSecond), 0, 1)));
            BodyNode.ParentNode.RunAction(SCNAction.RepeatActionForever(SCNAction.RotateBy(0, (float)(Revolution * (decimal)ParentBody.ParentSystem.TimePerSecond), 0, 1)));
        }
    }
}