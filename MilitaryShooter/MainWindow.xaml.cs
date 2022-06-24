using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MilitaryShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameEngine? gameEngine;
        private readonly List<Shape> shapesToRemove = new();

        private readonly DoubleAnimation fadeOutAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(100),
            From = 1,
            To = 0
        };

        private readonly DoubleAnimation fadeInAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(100),
            From = 0,
            To = 1
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SetUpGame()
        {
            GameCanvas.Children.Clear();
            //GameCanvas.Background = new ImageBrush()
            //{
            //    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/grass.jpg")),
            //};
            GameCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
            GameCanvas.Focus();
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            gameEngine.DrawObjects += DrawObjects;
            gameEngine.DrawLinesOfFire += DrawLinesOfFire;
            gameEngine.UpdateLabels += UpdateLabels;
            gameEngine.GameRestarted += OnGameRestarted;
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnCharacter;
            gameEngine.TriggerRemoveModel += RemoveModel;
            gameEngine.SpawnCharacters();
            SpawnLabels();
            await gameEngine.GameLoop();
        }

        private void SpawnLabels()
        {
            //Label scoreLabel = new()
            //{
            //    Name = "Score",
            //    Content = "Score: 0",
            //    FontSize = 18,
            //    FontWeight = FontWeights.Bold,
            //    Foreground = Brushes.White,
            //    Margin = new Thickness(20, 10, 0, 0),
            //};
            //Canvas.SetLeft(scoreLabel, 0);

            Label healthLabel = new()
            {
                Name = "Health",
                Content = $"Health: {gameEngine!.Player.Health}",
                Tag = "Player",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 20, 0),
            };
            GameCanvas.Children.Add(healthLabel);
            Canvas.SetRight(healthLabel, 0);
        }

        private async void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await GameMenuOpen();
        }

        private void UpdateLabels()
        {
            foreach (Label label in GameCanvas.Children.OfType<Label>())
            {
                label.Content = $"{label.Name}: {gameEngine!.Player.Health}";
            }
        }

        private void SpawnCharacter(Character character)
        {
            TranslateTransform moveTransform = new(character.Width / 2, character.Height / 2);
            List<UIElement> uIElements = new()
            {
                new Ellipse()
                {
                    Uid = character.Guid.ToString(),
                    Tag = character,
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

            foreach (UIElement element in uIElements)
            {
                TransformGroup transformGroup = new();
                transformGroup.Children.Add(moveTransform);
                element.RenderTransform = transformGroup;
                GameCanvas.Children.Add(element);

                Canvas.SetLeft(element, character.PositionLT.X);
                Canvas.SetTop(element, character.PositionLT.Y);
            }
        }

        private void SpawnBullet(Bullet bulletObj, Character character)
        {
            (double Width, double Height) trailSize = (bulletObj.Trail.W, bulletObj.Trail.H);
            TranslateTransform trailMoveTransform = new(bulletObj.Width - trailSize.Width, (bulletObj.Height / 2) - (trailSize.Height / 2));
            TranslateTransform moveTransform = new(bulletObj.Width / 2, bulletObj.Height / 2);
            RotateTransform rotateTransform = new(character.Angle);
            List<UIElement> uIElements = new()
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

            foreach (UIElement element in uIElements)
            {
                TransformGroup transformGroup = new();

                if (element is Ellipse ellipse && (string)ellipse.Tag == "BulletTrail")
                {
                    transformGroup.Children.Add(trailMoveTransform);
                }
                else
                {
                    transformGroup.Children.Add(moveTransform);
                }
                transformGroup.Children.Add(rotateTransform);
                element.RenderTransform = transformGroup;
                GameCanvas.Children.Add(element);
                Canvas.SetLeft(element, character.CenterPosition.X);
                Canvas.SetTop(element, character.CenterPosition.Y);
            }
        }

        private void DrawObjects()
        {
            foreach (UIElement element in GameCanvas.Children.OfType<UIElement>())
            {
                GameObject? obj = gameEngine!.GameObjects.Find(b => b.Guid.ToString() == element.Uid);
                if (obj != null)
                {
                    if (obj is Character character)
                    {
                        TransformGroup transformGroup = new();
                        TranslateTransform moveTransform = new(character.Width / 2, character.Height / 2);
                        RotateTransform rotateTransform = character is Player ? new(character.CurrentAngle) : new(character.Angle);
                        //transformGroup.Children.Add(moveTransform);
                        transformGroup.Children.Add(rotateTransform);
                        element.RenderTransform = transformGroup;
                    }
                    Canvas.SetLeft(element, obj.PositionLT.X);
                    Canvas.SetTop(element, obj.PositionLT.Y);
                }
            }
        }

        private void DrawLinesOfFire()
        {
            foreach (Character character in gameEngine!.Characters.Where(c => c is Player))
            {
                Line lineOfFire = new()
                {
                    Uid = character.Guid.ToString(),
                    Tag = "LineOfFire",
                    X1 = character.CenterPosition.X,
                    Y1 = character.CenterPosition.Y,
                    X2 = character.Aim.X,
                    Y2 = character.Aim.Y,
                    StrokeThickness = 2,
                    Opacity = 0.1,
                    Stroke = character is Player ? Brushes.LightGreen : Brushes.Red,
                };

                foreach (Line item in GameCanvas.Children.OfType<Line>().Where(l => (string)l.Tag == "LineOfFire" && l.Uid == character.Guid.ToString()).ToList())
                {
                    GameCanvas.Children.Remove(item);
                }
                GameCanvas.Children.Add(lineOfFire);
            }
        }

        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            GameControl.KeyDown(this, gameEngine!.Player, e);
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            GameControl.KeyUp(gameEngine!.Player, e);
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gameEngine!.Player.Shoot();
        }

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine!.Player.SetPath((position.X, position.Y));
        }

        private void GameCanvas_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            if (gameEngine != null)
            {
                gameEngine.Player.SetAim((position.X, position.Y));
                gameEngine.Player.Rotate();
            }
        }

        private void GameMenu_Restart_Button(object sender, RoutedEventArgs e)
        {
            if (gameEngine != null)
            {
                gameEngine.Reset();
            }
            else
            {
                OnGameRestarted();
            }
        }

        private void GameMenu_Quit_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveModel(GameObject gameObject)
        {
            shapesToRemove.AddRange(GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Uid == gameObject.Guid.ToString()));
            shapesToRemove.AddRange(GameCanvas.Children.OfType<Ellipse>().Where(rect => (string)rect.Uid == gameObject.Guid.ToString()));

            foreach (var item in shapesToRemove)
            {
                GameCanvas.Children.Remove(item);
            }
            shapesToRemove.Clear();
        }

        public async Task GameMenuOpen()
        {
            if (gameEngine != null)
            {
                gameEngine.Pause();
                Restart_Button.Content = "Restart";
                Continue_Button.IsEnabled = true;
                Continue_Button.Background = new SolidColorBrush(Color.FromArgb(123, 225, 219, 158));
            }
            else
            {
                Restart_Button.Content = "New";
                Continue_Button.IsEnabled = false;
            }
            await TransitionToGameMenu();
        }

        public async Task GameMenuClose()
        {
            await TransitionToGameCanvas(GameMenu);
            if (gameEngine != null)
            {
                gameEngine.UnPause();
                GameCanvas.Focus();
                await gameEngine.GameLoop();
            }
        }

        public async Task GamePause()
        {
            if (gameEngine != null)
            {
                if (gameEngine.Paused)
                {
                    //GameCanvas.Opacity = 1;
                    //GameCanvas.OpacityMask = null;
                    await TransitionToGameCanvas(GamePauseMask);
                    GameCanvas.Focus();
                    gameEngine.UnPause();
                    await gameEngine.GameLoop();
                }
                else
                {
                    //GameCanvas.Opacity = 0.8;
                    //GameCanvas.OpacityMask = new SolidColorBrush(Color.FromArgb(251, 0, 0, 0));
                    GamePauseMask.Focus();
                    gameEngine.Pause();
                    await TransitionTo(GamePauseMask);
                }
            }
        }

        private async Task TransitionTo(UIElement element)
        {
            await FadeIn(element);
        }

        private async Task TransitionToGameCanvas(UIElement element)
        {
            await FadeOut(element);
            element.Visibility = Visibility.Hidden;
        }

        private async Task TransitionToGameMenu()
        {
            await FadeIn(GameMenu);
        }

        private async Task FadeOut(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Hidden;
        }

        private async Task FadeIn(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Visible;
        }

        private async void GameMenu_Continue_Button(object sender, RoutedEventArgs e)
        {
            if (gameEngine != null)
            {
                await GameMenuClose();
            }
        }

        private async void OnGameRestarted()
        {
            await TransitionToGameCanvas(GameMenu);
            SetUpGame();
        }
    }
}