using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;


namespace UniverseScale.Core
{
    public class BodyAttachment
    {
        public readonly GeometryData AttachmentGeometry;
        public readonly float PhysicalSize;
        public readonly bool FromSurface = false;

        private float Scale = 1;

        public BodyAttachment(GeometryData Geometry, float _PhysicalSize, bool _FromSurface)
        {
            AttachmentGeometry = Geometry;
            PhysicalSize = _PhysicalSize;
            FromSurface = _FromSurface;
        }

        public void SetScaleFromAttachedBody(BodyData AttachedBody)
        {
            Scale = 0;
            if (FromSurface == true) Scale = 1;
            Scale += (float)((decimal)PhysicalSize / AttachedBody.PhysicalSize);
        }

        public SCNNode AppendAttachment(SCNNode ParentNode)
        {
            AttachmentGeometry.SetSize((float)((SCNSphere)ParentNode.Geometry).Radius * Scale);
            SCNNode Attachment = SCNNode.FromGeometry(AttachmentGeometry.NodeGeometry);
            if(AttachmentGeometry.Shape == GeometryData.Shapes.Plane)
            {
                Attachment.Transform = SCNMatrix4.CreateRotationX((float)(-Math.PI / 2));
            }
            Attachment.Name = "Attachment";
            ParentNode.AddChildNode(Attachment);
            return Attachment;
        }
    }
}