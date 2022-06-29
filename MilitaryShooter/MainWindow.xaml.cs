using MilitaryShooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MilitaryShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameEngine _gameEngine;

        private readonly List<Shape> _shapesToRemove;
        private readonly DoubleAnimation _fadeOutAnimation;
        private readonly DoubleAnimation _fadeInAnimation;

        public MainWindow()
        {
            InitializeComponent();
            _shapesToRemove = new List<Shape>();
            _fadeOutAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(100),
                From = 1,
                To = 0
            };
            _fadeInAnimation = new()
            {
                Duration = TimeSpan.FromMilliseconds(100),
                From = 0,
                To = 1
            };
            _gameEngine = new GameEngine();
        }

        private async Task SetUpGame()
        {
            GameCanvas.Children.Clear();
            GameCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
            GameCanvas.Focus();
            _gameEngine = new GameEngine(GameCanvas.Width, GameCanvas.Height);
            _gameEngine.DrawObjects += DrawObjects;
            _gameEngine.DrawLinesOfFire += DrawLinesOfFire;
            _gameEngine.UpdateLabels += UpdateLabels;
            _gameEngine.GameRestarted += OnGameRestarted;
            _gameEngine.TriggerSpawnBulletModel += SpawnProjectile;
            _gameEngine.TriggerSpawnModel += SpawnGameObject;
            _gameEngine.TriggerRemoveModel += RemoveModel;
            _gameEngine.TriggerPlayerDeath += OnPlayerDeath;
            _gameEngine.TriggerGamePause += OnGamePaused;
            _gameEngine.TriggerGameUnpause += OnGameUnpaused;
            _gameEngine.TriggerGameMenuOpen += GameMenuOpen;
            _gameEngine.TriggerGameMenuClose += GameMenuClose;
            _gameEngine.SpawnCharacters();
            SpawnLabels();
            await _gameEngine.GameLoop();
        }

        private void SpawnLabels()
        {
            Label healthLabel = new()
            {
                Name = "Health",
                Content = $"Health: {_gameEngine.Player!.Health}",
                Tag = "Player",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 20, 0),
            };
            GameCanvas.Children.Add(healthLabel);
            Canvas.SetRight(healthLabel, 0);
        }

        private void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GameMenuOpen();
        }

        private void UpdateLabels()
        {
            foreach (Label label in GameCanvas.Children.OfType<Label>())
            {
                label.Content = $"{label.Name}: {_gameEngine.Player!.Health}";
            }
        }

        private void SpawnProjectile(Projectile projectileObj, GameObject gameObject)
        {
            ModelFactory factory = new(projectileObj, gameObject);

            foreach (UIElement element in factory.GameObjectModel.Shapes)
            {
                GameCanvas.Children.Add(element);
                Canvas.SetLeft(element, gameObject.CenterPosition.X);
                Canvas.SetTop(element, gameObject.CenterPosition.Y);
            }
        }

        private void SpawnGameObject(GameObject gameObject)
        {
            ModelFactory factory = new(gameObject);

            foreach (UIElement element in factory.GameObjectModel.Shapes)
            {
                GameCanvas.Children.Add(element);
                Canvas.SetLeft(element, gameObject.PositionLT.X);
                Canvas.SetTop(element, gameObject.PositionLT.Y);
            }
        }

        private void DrawObjects()
        {
            foreach (GameObjectModel objectModel in GameObjectModel.Models)
            {
                objectModel.Transform();
                foreach (UIElement element in objectModel.Shapes)
                {
                    Canvas.SetLeft(element, objectModel.GameObject.PositionLT.X);
                    Canvas.SetTop(element, objectModel.GameObject.PositionLT.Y);
                }
            }
        }

        private void DrawLinesOfFire()
        {
            foreach (Character character in _gameEngine!.GetCharacters().Where(c => c.Laser))
            {
                foreach (GameObjectModel model in GameObjectModel.Models.OfType<LineOfFireModel>().Where(m => m.Guid == character.Guid))
                {
                    GameCanvas.Children.Remove(model.Shapes[0]);
                }
                GameObjectModel.Models.RemoveAll(m => m.GetType() == typeof(LineOfFireModel) && m.Guid == character.Guid);
                GameCanvas.Children.Add(new LineOfFireModel(character).Shapes.FirstOrDefault());
            }
        }

        private void RemoveModel(GameObject gameObject)
        {
            _shapesToRemove.AddRange(GameObjectModel.Models.Where(gom => gom.Guid == gameObject.Guid).SelectMany(gom => gom.Shapes).ToList());
            GameObjectModel.Models.RemoveAll(model => model.Guid.ToString() == gameObject.Guid.ToString());

            foreach (var item in _shapesToRemove)
            {
                GameCanvas.Children.Remove(item);
            }
            _shapesToRemove.Clear();
        }

        public async void GameMenuOpen()
        {
            if (_gameEngine.GameOver)
            {
                GameMenuText.Text = "Game Over\nYou ve been killed.";
            }
            else
            {
                GameMenuText.Text = "Game Menu";
            }

            if (_gameEngine.IsGameStarted)
            {
                Restart_Button.Content = "Restart";
                Continue_Button.IsEnabled = true;
                Continue_Button.Background = new SolidColorBrush(Color.FromArgb(123, 225, 219, 158));
            }
            else
            {
                Restart_Button.Content = "New";
                Continue_Button.IsEnabled = false;
            }
            await TransitionTo(GameMenu);
        }

        public async void GameMenuClose()
        {
            GameCanvas.Focus();
            await TransitionToGameCanvas(GameMenu);
        }

        public async void OnGameUnpaused()
        {
            await TransitionToGameCanvas(GamePauseMask);
            GameCanvas.Focus();
        }

        public async void OnGamePaused()
        {
            GamePauseMask.Focus();
            await TransitionTo(GamePauseMask);
        }

        private async void OnGameRestarted()
        {
            await TransitionToGameCanvas(GameMenu);
            OnGameUnpaused();
            await SetUpGame();
        }

        public void OnPlayerDeath()
        {
            GameMenuOpen();
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

        private async Task FadeOut(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, _fadeOutAnimation);
            await Task.Delay(_fadeOutAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Hidden;
        }

        private async Task FadeIn(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, _fadeInAnimation);
            await Task.Delay(_fadeInAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Visible;
        }

        private void GameMenu_Continue_Button(object sender, RoutedEventArgs e)
        {
            _gameEngine.Controls?.ContinueGame();
        }

        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            _gameEngine.Controls?.KeyDown(sender, e);
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            _gameEngine.Controls?.KeyUp(sender, e);
        }

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _gameEngine.Controls?.MouseDown(sender, e);
        }

        private void GameCanvas_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            _gameEngine.Controls?.MouseMove(sender, e);
        }

        private void GameMenu_Restart_Button(object sender, RoutedEventArgs e)
        {
            OnGameRestarted();
        }

        private void GameMenu_Quit_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}