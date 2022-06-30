using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            UIElements = new List<UIElement>()
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
                },
                new Label()
                {
                    Uid=character.Guid.ToString(),
                    Name = "Name",
                    Content = character.Name,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    Width = 128,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                new Label()
                {
                    Uid=character.Guid.ToString(),
                    Name = "Health",
                    Content = character.Health,
                    FontSize = 8,
                    Foreground = Brushes.White,
                    Width = 128,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };

            foreach (UIElement e in UIElements)
            {
                TransformGroup transformGroup = new();
                if (e is Label lbl)
                {
                    if (lbl.Name == "Name")
                    {
                        transformGroup.Children.Add(new TranslateTransform(-character.Width * 1.5, -character.Height - 8));
                    }
                    else if (lbl.Name == "Health")
                    {
                        transformGroup.Children.Add(new TranslateTransform(-character.Width * 1.5, -character.Height + 8));
                    }
                }
                else
                {
                    transformGroup.Children.Add(moveTransform);
                }
                e.RenderTransform = transformGroup;
            }
        }

        public override void Transform()
        {
            foreach (UIElement element in UIElements.Where(e => e.GetType() != typeof(Label)))
            {
                Character character = (Character)GetGameObject();
                TransformGroup transformGroup = new();
                RotateTransform rotateTransform = new(character.CurrentAngle);
                transformGroup.Children.Add(rotateTransform);
                element.RenderTransform = transformGroup;
            }
        }
    }
}