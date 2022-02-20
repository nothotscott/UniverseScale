using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace UniverseScale
{
    public static class ButtonColor
    {
        public static UIColor SelectBackgroundColor = UIColor.FromRGBA(127, 127, 127, 127);
        public static UIColor DeselectBackgroundColor = UIColor.FromRGBA(255, 255, 255, 127);
        public static UIColor SelectButtonColor = UIColor.LightGray;//UIColor.FromRGBA(127, 127, 127, 127);
        public static UIColor DeselectButtonColor = UIColor.Black;

        public static void SetButtonSelect(UIButton Button)
        {
            Button.BackgroundColor = SelectBackgroundColor;
            Button.TintColor = SelectButtonColor;
            Button.Enabled = false;
        }
        public static void SetButtonDeselect(UIButton Button)
        {
            Button.BackgroundColor = DeselectBackgroundColor;
            Button.TintColor = DeselectButtonColor;
            Button.Enabled = true;
        }
    }
}