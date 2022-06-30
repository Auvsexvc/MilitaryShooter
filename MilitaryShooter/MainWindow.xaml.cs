using MilitaryShooter.Models;
using System;
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

        public MainWindow()
        {
            InitializeComponent();
            _gameEngine = new GameEngine();
        }

        private async Task SetUpGame()
        {
            GameCanvas.Children.Clear();
            GameCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28));
            GameCanvas.Focus();
            _gameEngine = default!;
            _gameEngine = new GameEngine(GameCanvas.Width, GameCanvas.Height);
            _gameEngine.UpdateModels += DrawObjects;
            _gameEngine.UpdateLinesOfFire += DrawLinesOfFire;
            _gameEngine.UpdateLabels += OnUpdateLabels;
            _gameEngine.MakeProjectileModel += OnMakeProjectileModel;
            _gameEngine.MakeCharacterModel += OnMakeCharacterModel;
            _gameEngine.RemoveModel += OnRemoveModel;
            _gameEngine.PlayerDeath += OnPlayerDeath;
            _gameEngine.PauseGame += OnGamePaused;
            _gameEngine.UnpauseGame += OnGameUnpaused;
            _gameEngine.OpenGameMenu += GameMenuOpen;
            _gameEngine.CloseGameMenu += GameMenuClose;
            _gameEngine.SpawnCharacters();
            SpawnLabels();
            await _gameEngine.GameLoop();
        }

        private void DrawLinesOfFire()
        {
            foreach (CharacterModel characterModel in _gameEngine.GetModels().OfType<CharacterModel>())
            {
                foreach (UIElement e in GameCanvas.Children.OfType<Line>().Where(e => e.Uid == characterModel.Guid.ToString()).ToList())
                {
                    GameCanvas.Children.Remove(e);
                }
                Character character = (Character)characterModel.GetGameObject();
                if (character.LaserAssistance)
                {
                    GameCanvas.Children.Add(new LineOfFireModel(character).UIElements.FirstOrDefault());
                }
            }
        }

        private void DrawObjects()
        {
            foreach (GameObjectModel objectModel in _gameEngine.GetModels())
            {
                objectModel.Transform();
                foreach (UIElement e in objectModel.UIElements)
                {
                    Canvas.SetLeft(e, objectModel.GetGameObject().PositionLT.X);
                    Canvas.SetTop(e, objectModel.GetGameObject().PositionLT.Y);
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
                GameMenuText.Text = "Game Over\nYou've been killed.";
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
            _gameEngine.Player!.AlternativeControls = !_gameEngine.Player.AlternativeControls;
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

        private void OnRemoveModel(GameObjectModel model)
        {
            foreach (UIElement e in model.UIElements)
            {
                GameCanvas.Children.Remove(e);
            }
        }

        private async Task RestartGame()
        {
            _gameEngine.OnGameRestartedByPlayer();
            await TransitionFrom(GameMenu);
            await TransitionFrom(GamePauseMask);
            await SetUpGame();
            await TransitionTo(GameCanvas);
        }

        private void OnMakeCharacterModel(GameObjectModel model)
        {
            foreach (UIElement e in model.UIElements)
            {
                GameCanvas.Children.Add(e);
                Canvas.SetLeft(e, model.GetGameObject().PositionLT.X);
                Canvas.SetTop(e, model.GetGameObject().PositionLT.Y);
            }
        }

        private void SpawnLabels()
        {
            Label healthLabel = new()
            {
                Name = "CanvasHealth",
                Content = $"Health: {_gameEngine.Player!.Health}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 10, 20, 0),
            };
            Status.Children.Add(healthLabel);
            Canvas.SetRight(healthLabel, 0);
        }

        private void OnMakeProjectileModel(GameObjectModel model)
        {
            foreach (UIElement e in model.UIElements)
            {
                GameCanvas.Children.Add(e);
                Projectile projectile = (Projectile)model.GetGameObject();
                Canvas.SetLeft(e, projectile.Source.X);
                Canvas.SetTop(e, projectile.Source.Y);
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
            DoubleAnimation fadeOutAnimation = new()
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

        private void OnUpdateLabels()
        {
            foreach (Label label in Status.Children.OfType<Label>())
            {
                switch (label.Name)
                {
                    case "CanvasHealth":
                        {
                            label.Content = $"{label.Name}: {_gameEngine.Player!.Health}";
                            break;
                        }
                }
            }
            foreach (CharacterModel character in _gameEngine.GetModels().OfType<CharacterModel>())
            {
                foreach (Label characterLabel in character.UIElements.OfType<Label>())
                {
                    switch (characterLabel.Name)
                    {
                        case "Health":
                            characterLabel.Content = $"{character.GetGameObject().Health}";
                            break;

                        case "Name":
                            characterLabel.Content = $"{character.GetGameObject().Name}";
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}