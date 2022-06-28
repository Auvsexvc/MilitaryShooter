using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class GrenadeModel : GameObjectModel
    {
        private const string UriString = "Assets/grenade.png";

        public GrenadeModel(Grenade grenadeObj, GameObject gameObject) : base(grenadeObj)
        {
            TranslateTransform moveTransform = new(grenadeObj.Width / 2, grenadeObj.Height / 2);
            RotateTransform rotateTransform = new(gameObject.Angle);
            grenadeObj.TriggerModelResize += TransfromExplosion;

            Shapes = new List<Shape>()
            {
                new Ellipse()
                {
                    Uid = grenadeObj.Guid.ToString(),
                    Tag = "Grenade",
                    Height = grenadeObj.Height,
                    Width = grenadeObj.Width,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(UriString, UriKind.Relative)) },
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.1,
                }
            };

            foreach (Shape shape in Shapes)
            {
                TransformGroup transformGroup = new();
                transformGroup.Children.Add(moveTransform);
                transformGroup.Children.Add(rotateTransform);
                shape.RenderTransform = transformGroup;
            }
        }

        private void TransfromExplosion(Grenade grenadeOBJ)
        {
            foreach (Shape element in Shapes)
            {
                TransformGroup transformGroup = new();
                ScaleTransform sizeTransform = new ScaleTransform(grenadeOBJ.CurrentBlastRadius / 10, grenadeOBJ.CurrentBlastRadius / 10, 0.5, 0.5);
                transformGroup.Children.Add(sizeTransform);
                element.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
        }
    }
}