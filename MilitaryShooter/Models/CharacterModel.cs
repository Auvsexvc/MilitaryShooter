using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MilitaryShooter.Models
{
    internal class CharacterModel : GameObjectModel
    {
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
                    Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/soldier.png")) },
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