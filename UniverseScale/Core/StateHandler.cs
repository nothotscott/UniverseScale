using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;


namespace UniverseScale.Core
{
    public class StateHandler
    {
        public float Initial = 0;
        public float Last = 0;
        public float Multiplier = 1;

        public StateHandler(float _Multiplier, float LastDefault = 0)
        {
            Multiplier = _Multiplier;
            Last = LastDefault;
        }

        private float GetNewRaw(float Current)
        {
            return Current - Initial + Last;
        }

        public float GetNew(float Current)
        {
            return Multiplier * GetNewRaw(Current);
        }

        public void SetLastToNew(float Current)
        {
            Last = GetNewRaw(Current);
        }
    }
}