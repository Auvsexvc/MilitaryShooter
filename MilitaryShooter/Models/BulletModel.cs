using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class BulletModel : GameObjectModel
    {
        public BulletModel(Bullet bulletObj, GameObject gameObject) : base(bulletObj)
        {
            (double Width, double Height) = bulletObj.Trail;
            TranslateTransform trailMoveTransform = new(bulletObj.Width - Width, (bulletObj.Height / 2) - (Height / 2));
            TranslateTransform moveTransform = new(bulletObj.Width / 2, bulletObj.Height / 2);
            RotateTransform rotateTransform = new(gameObject.Angle);

            Shapes = new List<Shape>()
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

            foreach (Shape shape in Shapes)
            {
                TransformGroup transformGroup = new();

                if (shape is Ellipse ellipse && (string)ellipse.Tag == "BulletTrail")
                {
                    transformGroup.Children.Add(trailMoveTransform);
                }
                else
                {
                    transformGroup.Children.Add(moveTransform);
                }
                transformGroup.Children.Add(rotateTransform);
                shape.RenderTransform = transformGroup;
            }
        }
    }
}