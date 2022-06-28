using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal class CharacterModel : GameObjectModel
    {
        private const string UriString = "Assets/soldier.png";

        public CharacterModel(Character character) : base(character)
        {
            TranslateTransform moveTransform = new(character.Width / 2, character.Height / 2);
            Shapes = new List<Shape>()
            {
                new Ellipse()
                {
                    Uid = character.Guid.ToString(),
                    Tag = "Stand",
                    Name = character.Name,
                    Height = character.Height,
                    Width = character.Width,
                    Fill = character is Enemy ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White),
                    Opacity = 0.2,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                },
                new Ellipse()
                {
                    Uid = character.Guid.ToString(),
                    Tag = "Character",
                    Name = character.Name,
                    Height = character.Height,
                    Width = character.Width,
                    Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(UriString, UriKind.Relative)) },
                    Stroke = character is Enemy ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White),
                    StrokeThickness = 1.5,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                }
            };

            foreach (Shape shape in Shapes)
            {
                TransformGroup transformGroup = new();
                transformGroup.Children.Add(moveTransform);
                shape.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
            foreach (Shape element in Shapes)
            {
                TransformGroup transformGroup = new();
                RotateTransform rotateTransform = GameObject is Player ? new(GameObject.CurrentAngle) : new(GameObject.Angle);
                transformGroup.Children.Add(rotateTransform);
                element.RenderTransform = transformGroup;
            }
        }
    }
}