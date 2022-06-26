using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class LineOfFireModel : GameObjectModel
    {
        public LineOfFireModel(Character character) : base(character)
        {
            Shapes = new List<Shape>()
            {
                 new Line()
                    {
                        Uid = character.Guid.ToString(),
                        Tag = "LineOfFire",
                        X1 = character.CenterPosition.X,
                        Y1 = character.CenterPosition.Y,
                        X2 = character.AimDistance < character.RangeOfView/2 ? character.Aim.X : character.MaxRangePointTowardTarget(character.CenterPosition, character.Aim, character.RangeOfView/2).X,
                        Y2 = character.AimDistance < character.RangeOfView/2 ? character.Aim.Y : character.MaxRangePointTowardTarget(character.CenterPosition, character.Aim, character.RangeOfView/2).Y,
                        StrokeThickness = 1,
                        Opacity = 0.1,
                        Stroke = character is Player ? Brushes.LightGreen : Brushes.Red,
                    }
            };
        }

        public void RotateTransform(Character character)
        {
            foreach (Shape element in Shapes)
            {
                TransformGroup transformGroup = new();
                //TranslateTransform moveTransform = new(character.Width / 2, character.Height / 2);
                RotateTransform rotateTransform = character is Player ? new(character.CurrentAngle) : new(character.Angle);
                //transformGroup.Children.Add(moveTransform);
                transformGroup.Children.Add(rotateTransform);
                element.RenderTransform = transformGroup;
            }
        }
    }
}