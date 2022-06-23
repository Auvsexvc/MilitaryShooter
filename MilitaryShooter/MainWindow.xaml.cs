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
        private GameEngine gameEngine;
        private const int maxDelay = 16;
        private const int minDelay = 16;

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
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            SetUpGame();
        }

        private void SetUpGame()
        {
            GameCanvas.Children.Clear();
            //GameCanvas.Background = new ImageBrush()
            //{
            //    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/agrass.jpg")),
            //    Opacity = 2,
            //};
            GameCanvas.Focus();
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            gameEngine.GameRestarted += OnGameRestarted;
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnCharacter;
            gameEngine.TriggerRemoveModel += RemoveModel;
            gameEngine.SpawnCharacters();
            SpawnLabels();
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
                Content = $"Health: {gameEngine.Player.Health}",
                Tag = "Player",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 20, 0),
            };
            GameCanvas.Children.Add(healthLabel);
            Canvas.SetRight(healthLabel, 0);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async Task GameLoop()
        {
            while (!gameEngine.Paused)
            {
                int delay = Math.Max(minDelay, maxDelay);
                await Task.Delay(delay);

                gameEngine.UpdateObjects();

                DrawObjects();
                DrawLinesOfFire();
                UpdateLabels();
                gameEngine.CleanGameObjects();
            }
        }

        private void UpdateLabels()
        {
            foreach (Label label in GameCanvas.Children.OfType<Label>())
            {
                label.Content = $"{label.Name}: {gameEngine.Player.Health}";
            }
        }

        private void SpawnCharacter(Character character)
        {
            List<UIElement> uIElements = new()
            {
                new Ellipse()
                {
                    Uid = character.Guid.ToString(),
                    Tag = "Character",
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
                GameCanvas.Children.Add(element);

                Canvas.SetLeft(element, character.PositionLT.X);
                Canvas.SetTop(element, character.PositionLT.Y);
            }
        }

        private void SpawnBullet(Bullet bulletObj, Character character)
        {
            List<UIElement> uIElements = new()
            {
                new Ellipse()
                {
                    Uid = bulletObj.Guid.ToString(),
                    Tag = "Bullet",
                    Height = bulletObj.Height*10,
                    Width = bulletObj.Width*10,
                    RenderTransformOrigin = new Point(0.0, 0.0),
                    Fill = Brushes.LightYellow,
                    Opacity = 0.1,
                    RenderTransform = (RotateTransform)(new(character.Angle))
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
                    RenderTransform = (RotateTransform)(new(character.Angle))
                }
            };

            foreach (UIElement element in uIElements)
            {
                GameCanvas.Children.Add(element);
                Canvas.SetLeft(element, character.CenterPosition.X);
                Canvas.SetTop(element, character.CenterPosition.Y);
            }
        }

        private void DrawObjects()
        {
            foreach (UIElement element in GameCanvas.Children.OfType<UIElement>())
            {
                GameObject obj = gameEngine.GameObjects.Find(b => b.Guid.ToString() == element.Uid)!;
                if (obj != null)
                {
                    Canvas.SetLeft(element, obj.PositionLT.X);
                    Canvas.SetTop(element, obj.PositionLT.Y);
                    if (obj is Character character)
                    {
                        element.RenderTransform = (RotateTransform)(new()
                        {
                            Angle = character.Angle
                        });
                    }
                }
            }
        }

        private void DrawLinesOfFire()
        {
            foreach (Character character in gameEngine.Characters.Where(c => c is Player))
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
            GameControl.KeyDown(this, gameEngine.Player, e);
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            GameControl.KeyUp(gameEngine.Player, e);
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gameEngine.Player.Shoot();
        }

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetPath((position.X, position.Y));
        }

        private void GameCanvas_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetAim((position.X, position.Y));
        }

        private async void GameMenu_Restart_Button(object sender, RoutedEventArgs e)
        {
            gameEngine.Reset();
            await GameLoop();
        }

        private void GameMenu_Quit_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveModel(GameObject gameObject)
        {
            GameCanvas.Children.Remove(GameCanvas.Children.OfType<Rectangle>().FirstOrDefault(rect => (string)rect.Uid == gameObject.Guid.ToString()));
        }

        public async Task GameMenuOpen()
        {
            gameEngine.Pause();
            await TransitionToGameMenu();
        }

        public async Task GameMenuClose()
        {
            await TransitionToGameCanvas();
            gameEngine.UnPause();
            GameCanvas.Focus();
            await GameLoop();
        }

        private async Task TransitionToGameCanvas()
        {
            await FadeOut(GameMenu);
            GameMenu.Visibility = Visibility.Hidden;
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
            await GameMenuClose();
        }

        private async void OnGameRestarted()
        {
            await TransitionToGameCanvas();
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            SetUpGame();
            await GameLoop();
        }
    }
}