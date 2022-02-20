using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale.Core
{
    public class GeometryData
    {
        public enum Shapes { Custom, Sphere, Plane };

        public readonly Shapes Shape;
        public SCNGeometry NodeGeometry;
        private readonly string CustomShapeLocation;

        public GeometryData(Shapes _Shape, string _CustomShapeLocation = "")
        {
            Shape = _Shape;
            CustomShapeLocation = _CustomShapeLocation;

            switch (Shape)
            {
                case Shapes.Sphere:
                    NodeGeometry = SCNSphere.Create(1);
                    break;
                case Shapes.Plane:
                    NodeGeometry = SCNPlane.Create(1, 1);
                    break;
                default:
                    NodeGeometry = SCNScene.FromFile(CustomShapeLocation).RootNode.Geometry;
                    break;
            }
        }

        public float GetSize()
        {
            float Distance = 0;
            switch (Shape)
            {
                case Shapes.Sphere:
                    Distance = (float)((SCNSphere)NodeGeometry).Radius;
                    break;
                case Shapes.Plane:
                    Distance = (float)Math.Max(((SCNPlane)NodeGeometry).Width, ((SCNPlane)NodeGeometry).Height);
                    break;
            }
            return Distance;
        }
        public void SetSize(float Size)
        {
            switch (Shape)
            {
                case Shapes.Sphere:
                    ((SCNSphere)NodeGeometry).Radius = Size;
                    break;
                case Shapes.Plane:
                    ((SCNPlane)NodeGeometry).Width = Size;
                    ((SCNPlane)NodeGeometry).Height = Size;
                    break;
            }
        }
    }
}