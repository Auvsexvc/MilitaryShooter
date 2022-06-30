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

            UIElements = new List<UIElement>()
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

            foreach (UIElement e in UIElements)
            {
                TransformGroup transformGroup = new();
                transformGroup.Children.Add(moveTransform);
                transformGroup.Children.Add(rotateTransform);
                e.RenderTransform = transformGroup;
            }
        }

        private void TransfromExplosion(Grenade grenadeOBJ)
        {
            foreach (UIElement e in UIElements)
            {
                TransformGroup transformGroup = new();
                ScaleTransform sizeTransform = new(grenadeOBJ.CurrentBlastRadius / grenadeOBJ.Width, grenadeOBJ.CurrentBlastRadius / grenadeOBJ.Height);
                transformGroup.Children.Add(sizeTransform);
                e.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
        }
    }
}