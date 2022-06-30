using MilitaryShooter.Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class LineOfFireModel : GameModel
    {
        public LineOfFireModel(Character character) : base(character)
        {
            UIElements = new List<UIElement>()
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

        public override void Transform()
        {
        }
    }
}