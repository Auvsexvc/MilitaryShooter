using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class GrenadeModel : GameObjectModel
    {
        public GrenadeModel(Grenade grenadeObj, Character characterObj) : base(grenadeObj)
        {
            TranslateTransform moveTransform = new(grenadeObj.Width / 2, grenadeObj.Height / 2);
            RotateTransform rotateTransform = new(characterObj.Angle);
            grenadeObj.TriggerModelResize += TransfromExplosion;

            Shapes = new List<Shape>()
            {
                new Ellipse()
                {
                    Uid = grenadeObj.Guid.ToString(),
                    Tag = "Grenade",
                    Height = grenadeObj.Height,
                    Width = grenadeObj.Width,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1,
                    RenderTransformOrigin = new Point(0.5, 0.5)
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
                ScaleTransform sizeTransform = new ScaleTransform(grenadeOBJ.CurrentBlastRadius / grenadeOBJ.Width, grenadeOBJ.CurrentBlastRadius / grenadeOBJ.Height);
                transformGroup.Children.Add(sizeTransform);
                element.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
        }
    }
}