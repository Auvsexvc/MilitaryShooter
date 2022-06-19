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
        private readonly GameEngine gameEngine = new();
        private Rectangle playerRect = new();

        public MainWindow()
        {
            InitializeComponent();
            SetUpGame();
            SetPlayer();
            InitializeGameTimer();
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
        }

        private void SetUpGame()
        {
            GameCanvas.Focus();
            GameEngine.ResX = GameCanvas.Width;
            GameEngine.ResY = GameCanvas.Height;
        }

        private void SetPlayer()
        {
            playerRect = new()
            {
                Name = "playerRect",
                Uid = gameEngine.Player.Guid.ToString(),
                Height = gameEngine.Player.Height,
                Width = gameEngine.Player.Width,
                Fill = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/soldier.png")) },
                Stroke = new SolidColorBrush(Colors.White),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            GameCanvas.Children.Add(playerRect);
            Canvas.SetLeft(playerRect, gameEngine.Player.Position.X);
            Canvas.SetTop(playerRect, gameEngine.Player.Position.Y);
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            MoveCharacter(gameEngine.Player);

            DrawLineOfFire();
            MoveBullets();
        }

        private void DrawLineOfFire()
        {
            Line lineOfFire = new()
            {
                Tag = "Aim",
                X1 = gameEngine.Player.Position.X,
                Y1 = gameEngine.Player.Position.Y,
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
            timer.Interval = TimeSpan.FromMilliseconds(20);
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
                RenderTransform = (RotateTransform)(new(gameEngine.Player.Direction - 0/*, (playerRect.ActualWidth / 2), playerRect.ActualHeight / 2*/))
            };
            Canvas.SetLeft(newBullet, gameEngine.Player.Position.X);
            Canvas.SetTop(newBullet, gameEngine.Player.Position.Y);

            GameCanvas.Children.Add(newBullet);
        }

        private void MoveBullets()
        {
            foreach (Rectangle bullet in GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Tag == "Bullet").ToList())
            {
                var bulletObj = (Bullet?)gameEngine.GameObjects.Find(b => b.Guid.ToString() == bullet.Uid && b.GetType() == typeof(Bullet));

                (double X, double Y) = bulletObj!.Travel((Canvas.GetLeft(bullet), Canvas.GetTop(bullet)));
                Canvas.SetLeft(bullet, X);
                Canvas.SetTop(bullet, Y);

                if (Canvas.GetLeft(bullet) < 0 || Canvas.GetLeft(bullet) > GameCanvas.ActualWidth || Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) > GameCanvas.ActualHeight)
                {
                    ItemRemover(bullet);
                }
            }
        }

        private void MoveCharacter(Character character)
        {
            Canvas.SetLeft(playerRect, character.Move().X - (playerRect.Width / 2));
            Canvas.SetTop(playerRect, character.Move().Y - (playerRect.Height / 2));
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            
            double pX = position.X;
            if(position.X > GameEngine.ResX) pX = GameEngine.ResX;
            double pY = position.Y;
            if(position.Y > GameEngine.ResY) pY = GameEngine.ResY;

            Point playerCenter = new(Canvas.GetLeft(playerRect) + (playerRect.ActualWidth / 2), Canvas.GetTop(playerRect) + (playerRect.ActualHeight / 2));

            double radians = Math.Atan((pY - playerCenter.Y) /
                                       (pX - playerCenter.X));
            RotateTransform playerRotation = new();
            playerRect.RenderTransform = playerRotation;
            playerRotation.Angle = radians * 180 / Math.PI;

            if (position.X - playerCenter.X < 0)
            {
                playerRotation.Angle += 180;
            }
            gameEngine.Player.Direction = playerRotation.Angle;
            gameEngine.Player.Position = (playerCenter.X, playerCenter.Y);
            gameEngine.Player.Aim = (pX, pY);
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

        private void ItemRemover(UIElement element)
        {
            GameCanvas.Children.Remove(element);
            gameEngine.RemoveGameObject(gameEngine.GameObjects.First(o => o.Guid.ToString().Equals(element.Uid)));
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gameEngine.Player.Fire();
        }

        private void GameCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}