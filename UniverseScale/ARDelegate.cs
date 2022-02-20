using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale
{
    class ARDelegate : ARSCNViewDelegate
    {
        private MainViewController MainViewControllerObj;
        private Dictionary<ARPlaneAnchor, SCNNode> Planes = new Dictionary<ARPlaneAnchor, SCNNode>();
        private int PlaneUpdates = 0;

        public ARDelegate(MainViewController _MainViewController)
        {
            MainViewControllerObj = _MainViewController;
        }

        public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if(anchor is ARPlaneAnchor)
            {
                ARPlaneAnchor PlaneAnchor = (ARPlaneAnchor)anchor;

                SCNNode PlaneNode = SCNNode.FromGeometry(CreatePlaneGeometry(PlaneAnchor));
                PlaneNode.Position = new SCNVector3(PlaneAnchor.Center.X, 0, PlaneAnchor.Center.Z);
                PlaneNode.Transform = SCNMatrix4.CreateRotationX((float)(-Math.PI / 2));
                node.AddChildNode(PlaneNode);
                Planes[PlaneAnchor] = PlaneNode;
            }
        }

        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor is ARPlaneAnchor)
            {
                ARPlaneAnchor PlaneAnchor = (ARPlaneAnchor)anchor;
                if (Planes.ContainsKey(PlaneAnchor) == true)
                {
                    SCNNode PlaneNode = Planes[PlaneAnchor];
                    PlaneNode.Geometry = CreatePlaneGeometry(PlaneAnchor);
                    PlaneNode.Position = new SCNVector3(PlaneAnchor.Center.X, 0, PlaneAnchor.Center.Z);

                    PlaneUpdates++;
                    if (MainViewControllerObj.FirstPlaneDetection == false && PlaneUpdates >= Settings.PlaneUpdatesNeeded)
                    {
                        MainViewControllerObj.FirstPlaneDetection = true;
                        MainViewControllerObj.SelectedControl.SetPositionForPlane(PlaneNode.WorldPosition, Settings.MinSystemHeightForceTouch);
                    }
                }
            }
        }

        public override void DidRemoveNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor is ARPlaneAnchor)
            {
                ARPlaneAnchor PlaneAnchor = (ARPlaneAnchor)anchor;
                if (Planes.ContainsKey(PlaneAnchor) == true)
                {
                    Planes[PlaneAnchor].RemoveFromParentNode();
                    Planes.Remove(PlaneAnchor);
                }
            }
        }



        private SCNPlane CreatePlaneGeometry(ARPlaneAnchor PlaneAnchor)
        {
            SCNPlane Plane = SCNPlane.Create(PlaneAnchor.Extent.X, PlaneAnchor.Extent.Z);
            Plane.CornerRadius = 0.1f;
            Plane.FirstMaterial = SCNMaterial.Create();
            Plane.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
            Plane.FirstMaterial.Diffuse.ContentImage = UIImage.FromFile("Grid.png");
            Plane.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Scale(PlaneAnchor.Extent.X, PlaneAnchor.Extent.Z, 1);
            Plane.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
            Plane.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;

            return Plane;
        }

    }
}
