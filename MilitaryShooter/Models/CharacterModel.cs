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
    internal class CharacterModel : GameModel
    {
        private const string UriString = "Assets/soldier.png";

        public CharacterModel(GameObject gameObject) : base(gameObject)
        {
            TranslateTransform moveTransform = new(gameObject.Width / 2, gameObject.Height / 2);
            UIElements = new List<UIElement>()
            {
                new Ellipse()
                {
                    Uid = gameObject.Guid.ToString(),
                    Tag = "Stand",
                    Name = gameObject.Name,
                    Height = gameObject.Height,
                    Width = gameObject.Width,
                    Fill = gameObject is Enemy ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White),
                    Opacity = 0.2,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                },
                new Ellipse()
                {
                    Uid = gameObject.Guid.ToString(),
                    Tag = "Character",
                    Name = gameObject.Name,
                    Height = gameObject.Height,
                    Width = gameObject.Width,
                    Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri(UriString, UriKind.Relative)) },
                    Stroke = gameObject is Enemy ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White),
                    StrokeThickness = 1.5,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                },
                new Label()
                {
                    Uid=gameObject.Guid.ToString(),
                    Name = "Name",
                    Content = gameObject.Name,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    Width = 128,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                new Label()
                {
                    Uid=gameObject.Guid.ToString(),
                    Name = "Health",
                    Content = gameObject.Health,
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
                        transformGroup.Children.Add(new TranslateTransform(-gameObject.Width * 1.5, -gameObject.Height - 8));
                    }
                    else if (lbl.Name == "Health")
                    {
                        transformGroup.Children.Add(new TranslateTransform(-gameObject.Width * 1.5, -gameObject.Height + 8));
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