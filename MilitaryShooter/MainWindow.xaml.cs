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
            SetUpGame();
            InitializeGameTimer();
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnModel;
            gameEngine.TriggerRemoveModel += RemoveModel;
        }

        private void SetUpGame()
        {
            GameCanvas.Focus();
            SpawnModel(gameEngine.Player);
        }

        private void SpawnModel(GameObject gameObject)
        {
            Rectangle playerRect = new()
            {
                Name = gameObject.Name,
                Tag = "Character",
                Uid = gameObject.Guid.ToString(),
                Height = gameObject.Height,
                Width = gameObject.Width,
                Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/soldier.png")) },
                Stroke = new SolidColorBrush(Colors.White),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            GameCanvas.Children.Add(playerRect);
            Canvas.SetLeft(playerRect, gameObject.CenterPosition.X);
            Canvas.SetTop(playerRect, gameObject.CenterPosition.Y);
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            DrawCharacters();

            DrawLineOfFire();
            DrawBullets();
        }

        private void DrawLineOfFire()
        {
            Line lineOfFire = new()
            {
                Tag = "Aim",
                X1 = gameEngine.Player.CenterPosition.X,
                Y1 = gameEngine.Player.CenterPosition.Y,
                X2 = gameEngine.Player.Aim.X,
                Y2 = gameEngine.Player.Aim.Y,
                StrokeThickness = 2,
                Opacity = 0.3,
                Stroke = Brushes.Red
            };
            foreach (var item in GameCanvas.Children.OfType<Line>().Where(rect => (string)rect.Tag == "Aim").ToList())
            {
                GameCanvas.Children.Remove(item);
            }
            GameCanvas.Children.Add(lineOfFire);
        }

        private void InitializeGameTimer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += GameLoop;
            timer.Start();
        }

        private void SpawnBullet(Bullet bulletObj)
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
                RenderTransform = (RotateTransform)(new(gameEngine.Player.Direction))
            };
            Canvas.SetLeft(newBullet, gameEngine.Player.CenterPosition.X);
            Canvas.SetTop(newBullet, gameEngine.Player.CenterPosition.Y);

            GameCanvas.Children.Add(newBullet);
        }

        private void DrawBullets()
        {
            foreach (Rectangle bulletRect in GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Tag == "Bullet").ToList())
            {
                var bulletObj = (Bullet)gameEngine.GameObjects.Find(b => b.Guid.ToString() == bulletRect.Uid && b.GetType() == typeof(Bullet))!;

                gameEngine.UpdateBulletPos(bulletObj);

                Canvas.SetLeft(bulletRect, bulletObj.PositionLT.X);
                Canvas.SetTop(bulletRect, bulletObj.PositionLT.Y);

                
            }
        }

        private void DrawCharacters()
        {
            foreach (var characterRect in GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Tag == "Character").ToList())
            {
                var character = (Character?)gameEngine.GameObjects.Find(b => b.Guid.ToString() == characterRect.Uid)!;
                (double X, double Y) = character.Move();

                Canvas.SetLeft(characterRect, X);
                Canvas.SetTop(characterRect, Y);
                RotateTransform rotateTransform = new()
                {
                    Angle = gameEngine.Player.Direction
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