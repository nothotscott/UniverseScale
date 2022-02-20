using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;


namespace UniverseScale.Core
{
    public class TextureData
    {
        private string Color;
        private string Normal;
        private string Specular;
        private string Emission;

        public TextureData(string ColorImage, string NormalImage = null, string SpecularImage = null, string EmissionImage = null)
        {
            Color = ColorImage;
            Normal = NormalImage;
            Specular = SpecularImage;
            Emission = EmissionImage;
        }

        public SCNMaterial GetMaterial()
        {
            SCNMaterial Material = SCNMaterial.Create();
            Material.LightingModelName = SCNLightingModel.Phong;
            Material.LocksAmbientWithDiffuse = true;
            Material.Diffuse.ContentColor = UIColor.DarkGray;
            Material.Diffuse.ContentImage = UIImage.FromFile(Color);
            Material.Shininess = 1;
            if (Normal != null) Material.Normal.ContentImage = UIImage.FromFile(Normal);
            if (Specular != null) Material.Specular.ContentImage = UIImage.FromFile(Specular);
            if (Emission != null) Material.Emission.ContentImage = UIImage.FromFile(Emission);
            return Material;
        }
    }
}