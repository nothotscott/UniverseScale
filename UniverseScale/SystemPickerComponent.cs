using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

using UniverseScale.Core;

namespace UniverseScale
{
    class SystemPickerDataSource : UIPickerViewDataSource
    {
        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return Initialize.AllSystems.Count;
        }
    }


    class SystemPickerDelegate : UIPickerViewDelegate
    {
        private MainViewController MainViewControllerObj;

        public SystemPickerDelegate(MainViewController _MainViewController)
        {
            MainViewControllerObj = _MainViewController;
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            UILabel Label = (UILabel)view;
            if(Label == null)
            {
                Label = new UILabel();
                Label.Font = UIFont.FromName("Menlo", 18f);
                Label.TextAlignment = UITextAlignment.Center;
                Label.TextColor = UIColor.White;
            }
            Label.Text = Initialize.AllSystems[(int)row].Name;
            return Label;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            MainViewControllerObj.SetSystem((int)row);
        }
    }
}