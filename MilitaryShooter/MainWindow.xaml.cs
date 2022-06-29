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

        public MainWindow()
        {
            InitializeComponent();
            _gameEngine = new GameEngine();
            _shapesToRemove = new List<Shape>();
        }

        private async Task SetUpGame()
        {
            GameCanvas.Children.Clear();
            GameCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
            GameCanvas.Focus();
            _gameEngine = default!;
            _gameEngine = new GameEngine(GameCanvas.Width, GameCanvas.Height);
            _gameEngine.DrawObjects += DrawObjects;
            _gameEngine.DrawLinesOfFire += DrawLinesOfFire;
            _gameEngine.UpdateLabels += UpdateLabels;
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

        private void DrawLinesOfFire()
        {
            foreach (Character character in _gameEngine.GetCharacters().Where(c => c.LaserAssistance))
            {
                foreach (GameObjectModel model in GameObjectModel.Models.OfType<LineOfFireModel>().Where(m => m.Guid == character.Guid))
                {
                    GameCanvas.Children.Remove(model.Shapes[0]);
                }
                GameObjectModel.Models.RemoveAll(m => m.GetType() == typeof(LineOfFireModel) && m.Guid == character.Guid);
                GameCanvas.Children.Add(new LineOfFireModel(character).Shapes.FirstOrDefault());
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

        private void GameMenu_Continue_Button(object sender, RoutedEventArgs e)
        {
            _gameEngine.Controls?.ContinueGame();
        }

        private void GameMenu_Quit_Button(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void GameMenu_Restart_Button(object sender, RoutedEventArgs e)
        {
            await RestartGame();
        }

        private async void GameMenu_Settings_Button(object sender, RoutedEventArgs e)
        {
            await TransitionFrom(GameMenu);
            await TransitionTo(GameSettings);
        }

        private async void GameMenuClose()
        {
            GameCanvas.Focus();
            await TransitionFrom(GameMenu, 500);
        }

        private async void GameMenuOpen()
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

        private void GameSettings_AlternativeMovement_Button(object sender, RoutedEventArgs e)
        {
        }

        private async void GameSettings_BackToGameMenu_Button(object sender, RoutedEventArgs e)
        {
            await TransitionFrom(GameSettings);
            await TransitionTo(GameMenu);
        }

        private void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GameMenuOpen();
        }

        private async void OnGamePaused()
        {
            GamePauseMask.Focus();
            await TransitionTo(GamePauseMask);
        }

        private async void OnGameUnpaused()
        {
            await TransitionFrom(GamePauseMask);
            GameCanvas.Focus();
        }

        private void OnPlayerDeath()
        {
            GameMenuOpen();
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

        private async Task RestartGame()
        {
             _gameEngine.Reset();
            await TransitionFrom(GameMenu);
            await TransitionFrom(GamePauseMask);
            await SetUpGame();
            await TransitionTo(GameCanvas);
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

        private void SpawnProjectile(Projectile projectileObj, Character characterObj)
        {
            ModelFactory factory = new(projectileObj, characterObj);

            foreach (UIElement element in factory.GameObjectModel.Shapes)
            {
                GameCanvas.Children.Add(element);
                Canvas.SetLeft(element, characterObj.CenterPosition.X);
                Canvas.SetTop(element, characterObj.CenterPosition.Y);
            }
        }

        private static async Task TransitionFadeIn(UIElement e, int t)
        {
            DoubleAnimation fadeInAnimation = new()
            {
                Duration = TimeSpan.FromMilliseconds(t),
                From = 0,
                To = 1
            };
            e.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Visible;
        }

        private static async Task TransitionFadeOut(UIElement e, int t)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(t),
                From = 1,
                To = 0
            };
            e.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Hidden;
        }

        private static async Task TransitionFrom(UIElement e, int t = 100)
        {
            await TransitionFadeOut(e, t);
            e.Visibility = Visibility.Hidden;
        }

        private static async Task TransitionTo(UIElement e, int t = 100)
        {
            await TransitionFadeIn(e, t);
        }

        private void UpdateLabels()
        {
            foreach (Label label in GameCanvas.Children.OfType<Label>())
            {
                label.Content = $"{label.Name}: {_gameEngine.Player!.Health}";
            }
        }
    }
}