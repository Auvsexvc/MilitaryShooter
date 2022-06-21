using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            gameEngine = new(GameCanvas.Width, GameCanvas.Height);
            gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            gameEngine.TriggerSpawnModel += SpawnCharacter;
            gameEngine.TriggerRemoveModel += RemoveModel;
            gameEngine.GameTimer.Tick += GameLoop;
            SetUpGame();
        }

        private void SetUpGame()
        {
            GameCanvas.Focus();
            gameEngine.SpawnCharacters();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            gameEngine.UpdateObjects();
            //gameEngine.UpdateCharacters();
            //gameEngine.UpdateBullets();
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
                Height = 4,
                Width = 50,
                RenderTransformOrigin = new Point(0.0, 0.0),
                Fill = Brushes.Red,
                Stroke = Brushes.Yellow,
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
            switch (e.Key)
            {
                case Key.A:
                    gameEngine.Player.MoveLeft = true;
                    gameEngine.Player.PointsToMoveTo.Clear();
                    break;

                case Key.D:
                    gameEngine.Player.MoveRight = true;
                    gameEngine.Player.PointsToMoveTo.Clear();
                    break;

                case Key.W:
                    gameEngine.Player.MoveUp = true;
                    gameEngine.Player.PointsToMoveTo.Clear();
                    break;

                case Key.S:
                    gameEngine.Player.MoveDown = true;
                    gameEngine.Player.PointsToMoveTo.Clear();
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

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            gameEngine.Player.SetPath((position.X, position.Y));
        }
    }
}