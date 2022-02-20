using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

using UniverseScale.Core;
using CoreGraphics;
using System.Linq;

namespace UniverseScale
{
    interface IGestureControlMethods
    {
        void Touched(NSSet Touches);
        void Pinched(UIPinchGestureRecognizer Pinch);
        void Rotated(UIRotationGestureRecognizer Rotate);
        void Selected();
        void Deselected();
    }
    public class GestureControl : IGestureControlMethods
    {
        public enum ControlTypes { System, Body };
        public ControlTypes Control;
        protected UIButton Button;
        protected readonly MainViewController MainViewControllerObj;
        protected readonly ARSCNView ARView;
        protected UIPinchGestureRecognizer Pinch;
        protected UIRotationGestureRecognizer Rotate;

        public virtual void Pinched(UIPinchGestureRecognizer Pinch) { }
        public virtual void Rotated(UIRotationGestureRecognizer Rotate) { }
        public virtual void Selected() { }
        public virtual void Deselected() { }
        public virtual void Touched(NSSet Touches) { }
        public void HandleTouch(NSSet Touches)
        {
            if(MainViewControllerObj.SelectedControl.Control == Control)
            {
                Touched(Touches);
            }
        }

        public GestureControl(MainViewController _MainViewController, UIButton _Button)
        {
            MainViewControllerObj = _MainViewController;
            ARView = MainViewControllerObj.ARView;

            Button = _Button;
            Button.Layer.CornerRadius = 10;
        }
        public void Initialize()
        {
            Pinch = new UIPinchGestureRecognizer(Pinched);
            Pinch.Name = Control.ToString();
            Pinch.Delegate = new GestureRecognizerDelegate(MainViewControllerObj);
            MainViewControllerObj.ARView.AddGestureRecognizer(Pinch);
            Rotate = new UIRotationGestureRecognizer(Rotated);
            Rotate.Name = Control.ToString();
            Rotate.Delegate = new GestureRecognizerDelegate(MainViewControllerObj);
            MainViewControllerObj.ARView.AddGestureRecognizer(Rotate);
        }

        public void SetPositionForPlane(SCNVector3 Position, float? YOffset)
        {
            MainViewControllerObj.SystemPosition = Position;
            MainViewControllerObj.CurrentNodeHandler.SetPositon(Position, YOffset);
        }

        protected SCNVector3 HitTestPlane(CGPoint Point)
        {
            ARHitTestResult[] Hits = ARView.HitTest(Point, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (Hits != null && Hits.Length > 0)
            {
                var Anchors = Hits.Where((r) => r.Anchor is ARPlaneAnchor);
                if (Anchors.Count() > 0)
                {
                    var FirstAnchor = Anchors.First();
                    var Transform = FirstAnchor.WorldTransform;
                    var Position = new SCNVector3(Transform.M14, Transform.M24, Transform.M34);
                    return Position;
                }
            }
            return SCNVector3.Zero;
        }
        protected SCNNode HitTestNode(CGPoint Point)
        {
            SCNHitTestResult[] Hits = ARView.HitTest(Point, new NSDictionary());
            if(Hits != null && Hits.Length > 0)
            {
                var Nodes = Hits.Where((r) => r.Node is SCNNode);
                if(Nodes.Count() > 0)
                {
                    var FirstNode = Nodes.First().Node;
                    if(FirstNode.Name == "Attachment" || FirstNode.Name == "Billboard")
                    {
                        FirstNode = FirstNode.ParentNode;
                    }
                    return FirstNode;
                }
            }
            return null;
        }
    }


    public class SystemGestureControl : GestureControl, IGestureControlMethods
    {
        public SystemGestureControl(MainViewController _MainViewController, UIButton Button) : base(_MainViewController, Button)
        {
            Control = ControlTypes.System;
            Button.SetImage(UIImage.FromFile("SystemTools.png"), UIControlState.Normal);
            ButtonColor.SetButtonSelect(Button);
        }
        public override void Touched(NSSet Touches)
        {
            var Touch = (UITouch)Touches.AnyObject;
            if (Touch != null)
            {
                CGPoint CGPoisition = Touch.LocationInView(MainViewControllerObj.ARView);
                SCNVector3 WorldPosition = HitTestPlane(CGPoisition);
                if (WorldPosition != SCNVector3.Zero)
                {
                    float TouchPercent = (float)Touch.Force / (float)Touch.MaximumPossibleForce;
                    float MinPercent = Settings.MinSystemHeightForceTouchPercent / 100f;
                    if (TouchPercent >= MinPercent)
                    {
                        TouchPercent = (TouchPercent - MinPercent) / (1 - MinPercent);
                        float ForceHeight = Math.Min(Math.Max(TouchPercent * Settings.MaxSystemHeightForceTouch, Settings.MinSystemHeightForceTouch), Settings.MaxSystemHeightForceTouch);
                        if (ForceHeight > MainViewControllerObj.CurrentNodeHandler.CurrentHeight || MainViewControllerObj.CurrentNodeHandler.HeightReleased == true)
                        {
                            MainViewControllerObj.CurrentNodeHandler.CurrentHeight = ForceHeight;
                            MainViewControllerObj.CurrentNodeHandler.HeightReleased = false;
                        }
                    }
                    SetPositionForPlane(WorldPosition, MainViewControllerObj.CurrentNodeHandler.CurrentHeight);
                    MainViewControllerObj.FirstPlaneDetection = true;
                }
            }
        }
        public override void Pinched(UIPinchGestureRecognizer Pinch)
        {
            float Scale = Math.Min(Math.Max((float)Pinch.Scale, Settings.MinSystemScale), Settings.MaxSystemScale);
            switch (Pinch.State)
            {
                case UIGestureRecognizerState.Began:
                    MainViewControllerObj.CurrentNodeHandler.PinchState.Initial = Scale;
                    break;
                case UIGestureRecognizerState.Changed:
                    MainViewControllerObj.CurrentNodeHandler.RescaleSystem(Scale);
                    break;
                case UIGestureRecognizerState.Ended:
                    MainViewControllerObj.CurrentNodeHandler.PinchState.SetLastToNew(Scale);
                    break;
            }
        }
        public override void Rotated(UIRotationGestureRecognizer Rotate)
        {
            float Rotation = (float)Rotate.Rotation;
            switch (Rotate.State)
            {
                case UIGestureRecognizerState.Began:
                    MainViewControllerObj.CurrentNodeHandler.RotateState.Initial = Rotation;
                    break;
                case UIGestureRecognizerState.Changed:
                    MainViewControllerObj.CurrentNodeHandler.RotateSystem(Rotation);
                    break;
                case UIGestureRecognizerState.Ended:
                    MainViewControllerObj.CurrentNodeHandler.RotateState.SetLastToNew(Rotation);
                    break;
            }
        }
    }
    public class BodyGestureControl : GestureControl, IGestureControlMethods
    {
        private BodyNodeHandler Selection;

        public BodyGestureControl(MainViewController _MainViewController, UIButton Button) : base(_MainViewController, Button)
        {
            Control = ControlTypes.Body;
            Button.SetImage(UIImage.FromFile("BodyTools.png"), UIControlState.Normal);
            ButtonColor.SetButtonDeselect(Button);
            Selected();
        }
        public override void Touched(NSSet Touches)
        {
            bool Selected = false;
            var Touch = (UITouch)Touches.AnyObject;
            if (Touch != null)
            {
                CGPoint CGPoisition = Touch.LocationInView(MainViewControllerObj.ARView);
                SCNNode Node = HitTestNode(CGPoisition);
                if (Node != null)
                {
                    var BodyNode = MainViewControllerObj.CurrentNodeHandler.FindBodyNodeHandlerFromNode(Node);
                    if (BodyNode != null)
                    {
                        Selected = true;
                        Selection = BodyNode;
                        MainViewControllerObj.InvokeOnMainThread(delegate { MainViewControllerObj.SetTitleLabel(Selection.Node.Name); });
                    }
                }
                if (Selected == false && Selection != null)
                {
                    SCNVector3 WorldPosition = HitTestPlane(CGPoisition);
                    if (WorldPosition != SCNVector3.Zero)
                    {
                        var ARNodeDistance = Selection.Body.ARLength(Selection.Body.PhysicalDistance) * MainViewControllerObj.CurrentNodeHandler.SystemScale;
                        if(ARNodeDistance > 0)
                        {
                            var ReferenceNode = MainViewControllerObj.CurrentNodeHandler.FindBodyNodeHandlerFromBody(MainViewControllerObj.CurrentNodeHandler.SystemHandle.ReferenceBody).Node;
                            var PlanarDistance = Math.Sqrt(Math.Pow(Math.Abs(WorldPosition.X - ReferenceNode.WorldPosition.X), 2) + Math.Pow(Math.Abs(WorldPosition.Z - ReferenceNode.WorldPosition.Z), 2));
                            var PlanarScale = PlanarDistance / ARNodeDistance;
                            Selection.DistanceScale = (float)PlanarScale;
                            Selection.RepositionNode();
                        }
                    }
                }
            }
        }
        public override void Pinched(UIPinchGestureRecognizer Pinch)
        {
            if(Selection != null)
            {
                float Scale = Math.Min(Math.Max((float)Pinch.Scale, Settings.MinSystemScale), Settings.MaxSystemScale);
                switch (Pinch.State)
                {
                    case UIGestureRecognizerState.Began:
                        Selection.PinchState.Initial = Scale;
                        break;
                    case UIGestureRecognizerState.Changed:
                        Selection.ResizeNode(Scale);
                        break;
                    case UIGestureRecognizerState.Ended:
                        Selection.PinchState.SetLastToNew(Scale);
                        break;
                }
            }
        }
        public override void Rotated(UIRotationGestureRecognizer Rotate)
        {
            if (Selection != null)
            {

                float Rotation = (float)Rotate.Rotation;
                switch (Rotate.State)
                {
                    case UIGestureRecognizerState.Began:
                        Selection.RotateState.Initial = Rotation;
                        break;
                    case UIGestureRecognizerState.Changed:
                        Selection.RotateOrbitNode(Rotation);
                        break;
                    case UIGestureRecognizerState.Ended:
                        Selection.RotateState.SetLastToNew(Rotation);
                        break;
                }
            }
        }

        public override void Selected()
        {
            if(Selection == null || MainViewControllerObj.CurrentNodeHandler.SystemHandle.ReferenceBody != Selection.Body)
            {
                Selection = MainViewControllerObj.CurrentNodeHandler.FindBodyNodeHandlerFromBody(MainViewControllerObj.CurrentNodeHandler.SystemHandle.ReferenceBody);
            }
        }
    }



    class GestureRecognizerDelegate : UIGestureRecognizerDelegate
    {
        private MainViewController MainViewControllerObj;
        
        public GestureRecognizerDelegate(MainViewController _MainViewController)
        {
            MainViewControllerObj = _MainViewController;
        }
        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }
        public override bool ShouldBegin(UIGestureRecognizer recognizer)
        {
            if(recognizer.Name == MainViewControllerObj.SelectedControl.Control.ToString())
            {
                return true;
            }
            return false;
        }
    }
}
