using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MilitaryShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer timer = new();
        private readonly GameEngine gameEngine;

        public MainWindow()
        {
            InitializeComponent();
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnModel;
            gameEngine.TriggerRemoveModel += RemoveModel;

            SetUpGame();
            InitializeGameTimer();
        }

        private void SetUpGame()
        {
            GameCanvas.Focus();
            gameEngine.SpawnCharacters();
        }

        private void SpawnModel(Character character)
        {
            Rectangle characterRect = new()
            {
                Name = character.Name,
                Tag = "Character",
                Uid = character.Guid.ToString(),
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

        private void GameLoop(object? sender, EventArgs e)
        {
            DrawCharacters();
            DrawLinesOfFire();
            DrawBullets();
        }

        private void DrawLinesOfFire()
        {
            foreach (Character character in gameEngine.Characters)
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

        private void InitializeGameTimer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += GameLoop;
            timer.Start();
        }

        private void SpawnBullet(Bullet bulletObj, Character character)
        {
            Rectangle newBullet = new()
            {
                Tag = "Bullet",
                Height = 4,
                Width = 50,
                RenderTransformOrigin = new Point(0.0, 0.0),
                Fill = Brushes.Red,
                Stroke = Brushes.Yellow,
                Uid = bulletObj.Guid.ToString(),
                RenderTransform = (RotateTransform)(new(character.Direction))
            };
            Canvas.SetLeft(newBullet, character.CenterPosition.X);
            Canvas.SetTop(newBullet, character.CenterPosition.Y);

            GameCanvas.Children.Add(newBullet);
        }

        private void DrawBullets()
        {
            foreach (Rectangle bulletRect in GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Tag == "Bullet").ToList())
            {
                Bullet bulletObj = (Bullet)gameEngine.GameObjects.Find(b => b.Guid.ToString() == bulletRect.Uid && b.GetType() == typeof(Bullet))!;

                gameEngine.UpdateBulletPos(bulletObj);

                Canvas.SetLeft(bulletRect, bulletObj.PositionLT.X);
                Canvas.SetTop(bulletRect, bulletObj.PositionLT.Y);
            }
        }

        private void DrawCharacters()
        {
            foreach (Rectangle characterRect in GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Tag == "Character").ToList())
            {
                Character character = (Character?)gameEngine.GameObjects.Find(b => b.Guid.ToString() == characterRect.Uid)!;
                (double X, double Y) = character.Move();

                Canvas.SetLeft(characterRect, X);
                Canvas.SetTop(characterRect, Y);
                RotateTransform rotateTransform = new()
                {
                    Angle = character.Direction
                };
                characterRect.RenderTransform = rotateTransform;
            }
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetAim((position.X, position.Y));
        }

        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    gameEngine.Player.MoveLeft = true;
                    break;

                case Key.D:
                    gameEngine.Player.MoveRight = true;
                    break;

                case Key.W:
                    gameEngine.Player.MoveUp = true;
                    break;

                case Key.S:
                    gameEngine.Player.MoveDown = true;
                    break;
            }
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    gameEngine.Player.MoveLeft = false;
                    break;

                case Key.D:
                    gameEngine.Player.MoveRight = false;
                    break;

                case Key.W:
                    gameEngine.Player.MoveUp = false;
                    break;

                case Key.S:
                    gameEngine.Player.MoveDown = false;
                    break;
            }
        }

        private void RemoveModel(GameObject gameObject)
        {
            GameCanvas.Children.Remove(GameCanvas.Children.OfType<Rectangle>().FirstOrDefault(rect => (string)rect.Uid == gameObject.Guid.ToString()));
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
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
    }
}