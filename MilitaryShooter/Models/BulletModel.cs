using MilitaryShooter.Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class BulletModel : GameModel
    {
        public BulletModel(Bullet bulletObj, Character characterObj) : base(bulletObj)
        {
            (double Width, double Height) = bulletObj.Trail;
            TranslateTransform trailMoveTransform = new(bulletObj.Width - Width, (bulletObj.Height / 2) - (Height / 2));
            TranslateTransform moveTransform = new(bulletObj.Width / 2, bulletObj.Height / 2);
            RotateTransform rotateTransform = new(characterObj.Angle);

            UIElements = new List<UIElement>()
            {
                new Ellipse()
                {
                    Uid = bulletObj.Guid.ToString(),
                    Tag = "BulletTrail",
                    Height = bulletObj.Trail.H,
                    Width = bulletObj.Trail.W,
                    RenderTransformOrigin = new Point(0, 0),
                    Fill = new SolidColorBrush(Color.FromArgb(123, 225, 219, 158)),
                    Opacity = 0.1,
                },
                new Ellipse()
                {
                    Uid = bulletObj.Guid.ToString(),
                    Tag = "Bullet",
                    Height = bulletObj.Height,
                    Width = bulletObj.Width,
                    RenderTransformOrigin = new Point(0.0, 0.0),
                    Fill = bulletObj.IsTracer ? Brushes.Gray : Brushes.Red,
                    Stroke = Brushes.LightYellow,
                }
            };

            foreach (UIElement e in UIElements)
            {
                TransformGroup transformGroup = new();

                if (e is Ellipse ellipse && (string)ellipse.Tag == "BulletTrail")
                {
                    transformGroup.Children.Add(trailMoveTransform);
                }
                else
                {
                    transformGroup.Children.Add(moveTransform);
                }
                transformGroup.Children.Add(rotateTransform);
                e.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
        }
    }
}