using System;
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
        private readonly GameEngine gameEngine;

        private readonly DoubleAnimation fadeOutAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(100),
            From = 1,
            To = 0.1
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
            CompositionTargetEx.FrameUpdating += OnRender;
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnCharacter;
            gameEngine.TriggerRemoveModel += RemoveModel;
            SetUpGame();
        }

        private void SetUpGame()
        {
            GameCanvas.Focus();
            gameEngine.SpawnCharacters();
        }

        public void OnRender(object? sender, EventArgs e)
        {
            gameEngine.UpdateObjects();

            DrawObjects();
            DrawLinesOfFire();
            UpdateLabels();
            gameEngine.CleanGameObjects();
        }

        private void UpdateLabels()
        {
            Angle.Content = $"Angle: {gameEngine.Player.Angle}";
        }

        private void SpawnCharacter(Character character)
        {
            Rectangle characterRect = new()
            {
                Uid = character.Guid.ToString(),
                Tag = "Character",
                Name = character.Name,
                Height = character.Height,
                Width = character.Width,
                Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/soldier.png")) },
                Stroke = character is Enemy ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Blue),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            GameCanvas.Children.Add(characterRect);
            Canvas.SetLeft(characterRect, character.CenterPosition.X);
            Canvas.SetTop(characterRect, character.CenterPosition.Y);
        }

        private void SpawnBullet(Bullet bulletObj, Character character)
        {
            Rectangle newBullet = new()
            {
                Uid = bulletObj.Guid.ToString(),
                Tag = "Bullet",
                Height = bulletObj.Height,
                Width = bulletObj.Width,
                RenderTransformOrigin = new Point(0.0, 0.0),
                Fill = bulletObj.IsTracer ? Brushes.Gray : Brushes.Red,
                Stroke = Brushes.LightYellow,
                RenderTransform = (RotateTransform)(new(character.Angle))
            };
            Canvas.SetLeft(newBullet, character.CenterPosition.X);
            Canvas.SetTop(newBullet, character.CenterPosition.Y);

            GameCanvas.Children.Add(newBullet);
        }

        private void DrawObjects()
        {
            foreach (UIElement rect in GameCanvas.Children.OfType<UIElement>())
            {
                GameObject obj = gameEngine.GameObjects.Find(b => b.Guid.ToString() == rect.Uid)!;
                if (obj != null)
                {
                    Canvas.SetLeft(rect, obj.PositionLT.X);
                    Canvas.SetTop(rect, obj.PositionLT.Y);
                    if (obj is Character character)
                    {
                        rect.RenderTransform = (RotateTransform)(new()
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
                    Opacity = 0.3,
                    Stroke = character is Player ? Brushes.LightGreen : Brushes.Red,
                };

                foreach (Line item in GameCanvas.Children.OfType<Line>().Where(l => (string)l.Tag == "LineOfFire" && l.Uid == character.Guid.ToString()).ToList())
                {
                    GameCanvas.Children.Remove(item);
                }
                GameCanvas.Children.Add(lineOfFire);
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetAim((position.X, position.Y));
        }

        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            GameControl.KeyDown(this, gameEngine.Player, e);
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            GameControl.KeyUp(gameEngine.Player, e);
        }

        private void RemoveModel(GameObject gameObject)
        {
            GameCanvas.Children.Remove(GameCanvas.Children.OfType<Rectangle>().FirstOrDefault(rect => (string)rect.Uid == gameObject.Guid.ToString()));
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gameEngine.Player.Shoot();
        }

        private void GameCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetPath((position.X, position.Y));
        }

        public async Task OpenGamePanel()
        {
            if (GameMenu.Visibility == Visibility.Hidden)
            {
                CompositionTarget.Rendering -= OnRender;
                await TransitionToEndScreen();
            }
            else
            {
                await TransitionToGameScreen();
                GameCanvas.Focus();
                CompositionTarget.Rendering += OnRender;
            }
        }

        private async Task TransitionToGameScreen()
        {
            await FadeOut(GameMenu);
            await FadeIn(GameCanvas);
        }

        private async Task TransitionToEndScreen()
        {
            await FadeOut(GameCanvas);
            await FadeIn(GameMenu);
        }

        private async Task FadeOut(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            if (e == GameMenu)
            {
                e.Visibility = Visibility.Hidden;
            }
            else
            {
                e.Opacity = 0.5;
            }
        }

        private async Task FadeIn(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);
            if (e == GameMenu)
            {
                e.Visibility = Visibility.Visible;
            }
            else
            {
                e.Opacity = 0.5;
            }
        }

        private async void Continue_Click(object sender, RoutedEventArgs e)
        {
            await OpenGamePanel();
        }
    }
}