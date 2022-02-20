using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace UniverseScale
{
    public static class Settings
    {
        public static int HomebarMargin = 60; // UI px above home bar if device has one
        public static int PlaneUpdatesNeeded = 5; // plane updates needed for system to attach to plane
        public static float MinSystemScale = 0.1f; // min system size from pinching in
        public static float MaxSystemScale = 3f; // max system size from pinching out
        public static float MinBodyScale = 0.1f; // min body size from pinching in
        public static float MaxBodyScale = 3f; // max body size from pinching out
        public static int MinSystemHeightForceTouchPercent = 25; // percent of force needed for 3D touch to recognize MinSystemHeightForceTouch
        public static float MinSystemHeightForceTouch = 0.1f; // min height of system when 3D touch percent is at MinSystemHeightForceTouch
        public static float MaxSystemHeightForceTouch = 0.5f; // max height of system when 100% 3D touch
    }
}