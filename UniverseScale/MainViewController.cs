using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

using UniverseScale.Core;
using CoreGraphics;

namespace UniverseScale
{
    public partial class MainViewController : UIViewController
    {
        public SCNVector3 SystemPosition = new SCNVector3(0, 0, -0.4f);
        public const string DefaultSystem = "Earth";

        public bool FirstPlaneDetection = false;
        public SystemNodeHandler CurrentNodeHandler;
        public ARSCNView ARView;
        public GestureControl SelectedControl;

        private ARWorldTrackingConfiguration Config;
        private List<GestureControl> Controls;
        
        public MainViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ARView = (ARSCNView)View.ViewWithTag(1);

            Initialize.Setup();
            Initialize.InitializeSystems();

            //ARView.DebugOptions = ARSCNDebugOptions.ShowWorldOrigin | ARSCNDebugOptions.ShowFeaturePoints;
            //GetView().AutoenablesDefaultLighting = true;
            ARView.Delegate = new ARDelegate(this);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Config = new ARWorldTrackingConfiguration();
            Config.PlaneDetection = ARPlaneDetection.Horizontal;
            ARView.Session.Run(Config, ARSessionRunOptions.RemoveExistingAnchors);

            SetSystem(Initialize.GetSystemIndexFromName(DefaultSystem), false);
            InitalizeControls();
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ARView.Session.Pause();
        }
        
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            SelectedControl.HandleTouch(touches);
        }
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            SelectedControl.HandleTouch(touches);
        }
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            CurrentNodeHandler.HeightReleased = true;
        }

        /////////////

        ////// private methods
        public void SetSystem(int SystemIndex, bool SetTitle = true)
        {
            if(CurrentNodeHandler != null)
            {
                CurrentNodeHandler.SystemNode.RemoveFromParentNode();
                CurrentNodeHandler.SystemNode.Dispose();
            }
            CurrentNodeHandler = new SystemNodeHandler(Initialize.AllSystems[SystemIndex]);
            CurrentNodeHandler.SetPositon(SystemPosition, null);
            CurrentNodeHandler.BeginMovement();
            ARView.Scene.RootNode.AddChildNode(CurrentNodeHandler.SystemNode);
            if (SetTitle == true) {
                InvokeOnMainThread(
                delegate {
                    SetTitleLabel(CurrentNodeHandler.SystemHandle.Name);
                });
            }
        }
        
        public void SetTitleLabel(string Title)
        {
            UILabel TitleLabel = (UILabel)View.ViewWithTag(5);
            TitleLabel.Text = Title;
            if (SelectedControl.Control == GestureControl.ControlTypes.System)
            {
                TitleLabel.Font = UIFont.FromName("Menlo-Bold", TitleLabel.Font.PointSize);
                TitleLabel.TextColor = UIColor.FromRGB(255, 255, 255);
            }
            else if (SelectedControl.Control == GestureControl.ControlTypes.Body)
            {
                TitleLabel.Font = UIFont.FromName("Menlo", TitleLabel.Font.PointSize);
                TitleLabel.TextColor = UIColor.FromRGB(51, 51, 51);
            }
        }
        private void InitalizeControls()
        {
            UIButton SystemControl = (UIButton)View.ViewWithTag(3);
            UIButton BodyControl = (UIButton)View.ViewWithTag(4);
            UIVisualEffectView Blur = (UIVisualEffectView)View.ViewWithTag(6);

            Controls = new List<GestureControl>() { new SystemGestureControl(this, SystemControl), new BodyGestureControl(this, BodyControl) };
            Controls.ForEach((gc) => { gc.Initialize(); });
            SelectedControl = Controls[0];

            UIPickerView SystemPicker = (UIPickerView)View.ViewWithTag(2);
            SystemPicker.DataSource = new SystemPickerDataSource();
            SystemPicker.Delegate = new SystemPickerDelegate(this);
            SystemPicker.Select(Initialize.GetSystemIndexFromName(DefaultSystem), 0, false);

            SystemControl.TouchUpInside += delegate
            {
                if (SelectedControl != Controls[0])
                {
                    SelectedControl.Deselected();
                    SelectedControl = Controls[0];
                    SelectedControl.Selected();
                    SetTitleLabel(CurrentNodeHandler.SystemHandle.Name);
                    ButtonColor.SetButtonSelect(SystemControl);
                    ButtonColor.SetButtonDeselect(BodyControl);
                    UIView.Animate(0.25, delegate
                    {
                        Blur.Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark);
                    });
                }
            };
            BodyControl.TouchUpInside += delegate
            {
                if (SelectedControl != Controls[1])
                {
                    SelectedControl.Deselected();
                    SelectedControl = Controls[1];
                    SelectedControl.Selected();
                    SetTitleLabel(CurrentNodeHandler.SystemHandle.ReferenceBody.Name);
                    ButtonColor.SetButtonSelect(BodyControl);
                    ButtonColor.SetButtonDeselect(SystemControl);
                    UIView.Animate(0.25, delegate
                    {
                        Blur.Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
                    });
                }
            };

            SetTitleLabel(CurrentNodeHandler.SystemHandle.Name);
        }
    }
}