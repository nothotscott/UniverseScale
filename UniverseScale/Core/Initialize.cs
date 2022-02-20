using Foundation;
using System.Collections.Generic;
using System;
using UIKit;

using SpriteKit;
using SceneKit;
using ARKit;

namespace UniverseScale.Core
{
    public static class Initialize
    {
        public static BodyData Earth { get; private set; }
        public static BodyData Saturn { get; private set; }

        public static List<System> AllSystems;
        

        public static void Setup()
        {
            Earth = new BodyData("Earth", 6371, 0, 22.5f, new BodyMovement(TimeScale.Days), new TextureData("Textures/Earth/Earth.jpg", "Textures/Earth/EarthNormal.bmp", "Textures/Earth/EarthSpecular.jpg", "Textures/Earth/EarthLights.jpg"));
            var EarthAtm = new GeometryData(GeometryData.Shapes.Sphere);
            EarthAtm.NodeGeometry.FirstMaterial = SCNMaterial.Create();
            EarthAtm.NodeGeometry.FirstMaterial.Shininess = 1;
            EarthAtm.NodeGeometry.FirstMaterial.Transparency = 0.15f;//0.225f;
            EarthAtm.NodeGeometry.FirstMaterial.Diffuse.ContentImage = UIImage.FromFile("Textures/Earth/EarthClouds.jpg");
            Earth.AddAttachment(new BodyAttachment(EarthAtm, 80, true));

            Saturn = new BodyData("Saturn", 60268, 0, 26.7f, new BodyMovement(TimeScale.Hours * 10.656f), new TextureData("Textures/Saturn/Saturn.jpg"));
            var SaturnRings = new GeometryData(GeometryData.Shapes.Plane);
            SaturnRings.NodeGeometry.FirstMaterial = SCNMaterial.Create();
            SaturnRings.NodeGeometry.FirstMaterial.DoubleSided = true;
            SaturnRings.NodeGeometry.FirstMaterial.LightingModelName = SCNLightingModel.PhysicallyBased;
            SaturnRings.NodeGeometry.FirstMaterial.Diffuse.ContentImage = UIImage.FromFile("Textures/Saturn/SaturnRings.png");
            Saturn.AddAttachment(new BodyAttachment(SaturnRings, 250000, false));
        }

        public static void InitializeSystems()
        {
            AllSystems = new List<System>()
            {
                new System("Solar System", 0.08f, "km", TimeScale.Days,
                    new BodyData("Sun", 695700, 0, 0, new BodyMovement(TimeScale.Days * 24.47f), new TextureData("Textures/Sun.jpg"), true)
                ).AddBodies(new List<BodyData>(){
                    new BodyData("Mercury", 2440, 46001200, 2, new BodyMovement(TimeScale.Days * 58.646f, TimeScale.Days * 87.969f), new TextureData("Textures/Mercury.jpg")),
                    new BodyData("Venus", 6052, 108208000, 2, new BodyMovement(TimeScale.Days * -243, TimeScale.Days * 224.701f), new TextureData("Textures/Venus.jpg")),
                    Earth.ToOrbitalBody(149598023, TimeScale.Years),
                    new BodyData("Mars", 3389, 227939200, 25.25f, new BodyMovement(TimeScale.Days * 1.025f, TimeScale.Days * 686.97f), new TextureData("Textures/Mars.jpg")),
                    new BodyData("Jupiter", 69911, 778570000, 3.13f, new BodyMovement(TimeScale.Hours * 9.925f, TimeScale.Years * 11.862f), new TextureData("Textures/Jupiter.jpg")),
                    Saturn.ToOrbitalBody(1433530000, TimeScale.Years * 29.4571f),
                    new BodyData("Uranus", 25362, 2875040000, 97.77f, new BodyMovement(TimeScale.Days * 0.71833f, TimeScale.Years * 84.0205f), new TextureData("Textures/Uranus.jpg")),
                    new BodyData("Neptune", 24622, 4500000000, 28.3f, new BodyMovement(TimeScale.Days * 0.6713f, TimeScale.Years * 164.8f), new TextureData("Textures/Uranus.jpg"))
                }),

                new System("Saturn", 0.06f, "km", TimeScale.Hours, Saturn
                ).AddBodies(new List<BodyData>(){
                    new BodyData("Mimas", 198, 185539, 0, BodyMovement.Synchronous(TimeScale.Days * 0.942f), new TextureData("Textures/Saturn/Mimas.jpg")),
                    new BodyData("Dione", 561, 377396, 0, BodyMovement.Synchronous(TimeScale.Days * 2.736f), new TextureData("Textures/Saturn/Dione.jpg")),
                    new BodyData("Rhea", 764, 527108, 0, BodyMovement.Synchronous(TimeScale.Days * 4.518f), new TextureData("Textures/Saturn/Rhea.jpg")),
                    new BodyData("Titan", 2576, 1221870, 0, BodyMovement.Synchronous(TimeScale.Days * 15.945f), new TextureData("Textures/Saturn/Titan.png")),
                }),

                new System("Earth", 0.04f, "km", TimeScale.Hours, Earth).AddBody(
                    new BodyData("Moon", 1737, 238900, 1.5f, BodyMovement.Synchronous(TimeScale.Days * 27), new TextureData("Textures/Earth/Moon.png", "Textures/Earth/MoonNormal.jpg"))
                )
            };
        }

        public static int GetSystemIndexFromName(string Name)
        {
            for(int i = 0; i < AllSystems.Count; i++)
            {
                if(AllSystems[i].Name == Name)
                {
                    return i;
                }
            }
            return 0;
        }

    }
}