using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale.Core
{
    public class BodyData
    {
        public readonly string Name;
        public readonly decimal PhysicalSize;
        public readonly long PhysicalDistance;
        public readonly float AxisTilt;
        public readonly BodyMovement Movement;
        public readonly GeometryData Geometry;
        public readonly bool Selflit;

        public System ParentSystem;

        private readonly TextureData FirstTexture;
        private List<BodyAttachment> Attachments;

        public BodyData(string _Name, decimal _PhysicalSize, long _PhysicalDistance, float _AxisTilt, BodyMovement _Movement, TextureData _FirstTexture, bool _Selflit = false)
        {
            Name = _Name;
            PhysicalSize = _PhysicalSize;
            PhysicalDistance = _PhysicalDistance;
            AxisTilt = _AxisTilt;
            Movement = _Movement;
            FirstTexture = _FirstTexture;
            Selflit = _Selflit;

            Attachments = new List<BodyAttachment>();
            Movement.SetParentBody(this);
            Geometry = new GeometryData(GeometryData.Shapes.Sphere);
        }

        public BodyData ToOrbitalBody(long _PhysicalDistance, float Revolution)
        {
            return new BodyData(Name, PhysicalSize, _PhysicalDistance, AxisTilt, new BodyMovement((float)Movement.Rotation, (float)TimeScale.RadianPeriod(Revolution), false), FirstTexture, Selflit);
        }

        public void AddAttachment(BodyAttachment Attachment)
        {
            Attachment.SetScaleFromAttachedBody(this);
            Attachments.Add(Attachment);
        }

        public float ARLength(decimal PhysicalLength)
        {
            return (float)ParentSystem.Scale * (float)PhysicalLength;
        }

        public void SetBillboardPosition(SCNNode Node)
        {
            SCNNode Billboard = null;
            foreach(SCNNode ChildNode in Node.ChildNodes)
            {
                if(ChildNode.Name == "Billboard")
                {
                    Billboard = ChildNode;
                    break;
                }
            }
            if(Billboard != null)
            {
                Billboard.Position = new SCNVector3(0, Geometry.GetSize() + (float)((SCNPlane)Billboard.Geometry).Height / 2, 0);
            }
        }
        private SCNNode BillboardText(string TextInput, float YScale)
        {
            float Scale = 0.005f;
            SCNText Text = SCNText.Create(TextInput, 0.1f);
            Text.Font = UIFont.FromName("Menlo", 2f);
            Text.FirstMaterial.Diffuse.ContentColor = UIColor.White;
            Text.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
            SCNNode TextNode = SCNNode.FromGeometry(Text);
            TextNode.Scale = new SCNVector3(Scale, Scale, Scale);
            SCNVector3 Min = SCNVector3.Zero;
            SCNVector3 Max = SCNVector3.Zero;
            Text.GetBoundingBox(ref Min, ref Max);
            TextNode.Pivot = SCNMatrix4.CreateTranslation((float)(Min.X + 0.5 * (Max.X - Min.X)), 0, (float)(Min.Z + 0.5 * (Max.Z - Min.Z))); // (float)(Min.Y + YScale * (Max.Y - Min.Y))
            return TextNode;
        }
        public SCNNode CreateNode(bool ReferenceBody)
        {
            var AxisAngle = new SCNVector3(0, 0, (float)(Math.PI * AxisTilt / 180));
            Geometry.SetSize(ARLength(PhysicalSize));
            SCNNode OrbitNode = SCNNode.Create();
            OrbitNode.Name = "Orbit|" + Name;
            SCNNode Node = SCNNode.FromGeometry(Geometry.NodeGeometry);
            Node.Name = Name;
            Node.Position = new SCNVector3(0, 0, -ARLength(PhysicalDistance));
            Node.Geometry.FirstMaterial = FirstTexture.GetMaterial();
            if(ReferenceBody == true)
            {
                OrbitNode.EulerAngles = AxisAngle;
            }
            else
            {
                Node.EulerAngles = AxisAngle;
            }
            foreach (BodyAttachment Attachment in Attachments)
            {
                Node.AddChildNode(Attachment.AppendAttachment(Node));
            }
            OrbitNode.AddChildNode(Node);

            SCNPlane BillboardPlane = SCNPlane.Create(0.1f, 0.05f);
            BillboardPlane.FirstMaterial.Diffuse.ContentColor = UIColor.FromRGBA(0, 0, 0, 64);
            BillboardPlane.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
            BillboardPlane.CornerRadius = 0.01f;
            SCNNode Billboard = SCNNode.FromGeometry(BillboardPlane);
            Billboard.Name = "Billboard";
            Billboard.Constraints = new SCNConstraint[] { SCNBillboardConstraint.Create() };
            Node.AddChildNode(Billboard);
            Billboard.AddChildNode(BillboardText(Name, 0));
            SetBillboardPosition(Node);

            if(Selflit == true)
            {
                Node.Geometry.FirstMaterial.SelfIllumination.ContentColor = UIColor.White;
                Node.Light = SCNLight.Create();
                Node.Light.LightType = SCNLightType.Omni;
            }

            return Node;
        }
    }
}
