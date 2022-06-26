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
            GameStatic.resolution = (GameCanvas.Width, GameCanvas.Height);
            _gameEngine = new GameEngine(false);
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
            _gameEngine = new GameEngine(true);
            _gameEngine.DrawObjects += DrawObjects;
            _gameEngine.DrawLinesOfFire += DrawLinesOfFire;
            _gameEngine.UpdateLabels += UpdateLabels;
            _gameEngine.GameRestarted += OnGameRestarted;
            _gameEngine.TriggerSpawnBulletModel += SpawnBullet;
            _gameEngine.TriggerSpawnModel += SpawnGameObject;
            _gameEngine.TriggerRemoveModel += RemoveModel;
            _gameEngine.SpawnCharacters();
            SpawnLabels();
            await _gameEngine.GameLoop();
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
                Content = $"Health: {_gameEngine!.Player.Health}",
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
                label.Content = $"{label.Name}: {_gameEngine!.Player.Health}";
            }
        }

        //private void SpawnCharacter(Character character)
        //{
        //    CharacterModel characterModel = new(character);
        //    foreach (UIElement element in characterModel.Shapes)
        //    {
        //        GameCanvas.Children.Add(element);
        //        Canvas.SetLeft(element, character.PositionLT.X);
        //        Canvas.SetTop(element, character.PositionLT.Y);
        //    }
        //}

        private void SpawnBullet(Bullet bulletObj, GameObject gameObject)
        {
            BulletModel bulletModel = new(bulletObj, gameObject);
            foreach (UIElement element in bulletModel.Shapes)
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
                GameObject? obj = _gameEngine!.GameObjects.Find(b => b.Guid == objectModel.Guid);

                if (obj != null)
                {
                    if (objectModel is CharacterModel characterModel)
                    {
                        characterModel.RotateTransform((Character)obj);
                    }
                    foreach (UIElement element in objectModel.Shapes)
                    {
                        Canvas.SetLeft(element, obj.PositionLT.X);
                        Canvas.SetTop(element, obj.PositionLT.Y);
                    }
                }
            }
        }

        private void DrawLinesOfFire()
        {
            //foreach (GameObjectModel objectModel in GameObjectModel.Models.OfType<LineOfFireModel>())
            //{
            //    GameObject? obj = gameEngine!.GameObjects.Find(b => b.Guid == objectModel.Guid);
            //    if (obj != null)
            //    {
            //        CharacterModel characterModel = (CharacterModel)objectModel;
            //        characterModel.RotateTransform((Character)obj);
            //        foreach (UIElement element in objectModel.Shapes)
            //        {
            //            Canvas.SetLeft(element, obj.PositionLT.X);
            //            Canvas.SetTop(element, obj.PositionLT.Y);
            //        }
            //    }
            //}

            foreach (Character character in _gameEngine!.Characters/*.Where(c => c is Character)*/)
            {
                foreach (GameObjectModel model in GameObjectModel.Models.OfType<LineOfFireModel>().Where(m => m.Guid == character.Guid))
                {
                    GameCanvas.Children.Remove(model.Shapes[0]);
                }
                GameObjectModel.Models.RemoveAll(m => m.GetType() == typeof(LineOfFireModel) && m.Guid == character.Guid);
                GameCanvas.Children.Add(new LineOfFireModel(character).Shapes.FirstOrDefault());
            }
        }

        private void GameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            GameControl.KeyDown(this, _gameEngine!.Player, e);
        }

        private void GameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            GameControl.KeyUp(_gameEngine!.Player, e);
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _gameEngine!.Player.Shoot();
        }

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            _gameEngine!.Player.SetPath((position.X, position.Y));
        }

        private void GameCanvas_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            if (_gameEngine.IsGameStarted)
            {
                _gameEngine.Player.SetAim((position.X, position.Y));
                _gameEngine.Player.Rotate();
            }
        }

        private void GameMenu_Restart_Button(object sender, RoutedEventArgs e)
        {
            if (_gameEngine.IsGameStarted)
            {
                _gameEngine.Reset();
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
            //shapesToRemove.AddRange(GameCanvas.Children.OfType<Rectangle>().Where(rect => (string)rect.Uid == gameObject.Guid.ToString()));
            //shapesToRemove.AddRange(GameCanvas.Children.OfType<Ellipse>().Where(rect => (string)rect.Uid == gameObject.Guid.ToString()));

            _shapesToRemove.AddRange(GameObjectModel.Models.Where(gom => gom.Guid == gameObject.Guid).SelectMany(gom => gom.Shapes).ToList());
            GameObjectModel.Models.RemoveAll(model => model.Guid.ToString() == gameObject.Guid.ToString());

            foreach (var item in _shapesToRemove)
            {
                GameCanvas.Children.Remove(item);
            }
            _shapesToRemove.Clear();
        }

        public async Task GameMenuOpen()
        {
            if (_gameEngine.IsGameStarted)
            {
                _gameEngine.Pause();
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
            if (_gameEngine.IsGameStarted)
            {
                _gameEngine.UnPause();
                GameCanvas.Focus();
                await _gameEngine.GameLoop();
            }
        }

        public async Task GamePause()
        {
            if (_gameEngine.IsGameStarted)
            {
                if (_gameEngine.Paused)
                {
                    await TransitionToGameCanvas(GamePauseMask);
                    GameCanvas.Focus();
                    _gameEngine.UnPause();
                    _gameEngine.GameLoop();
                }
                else
                {
                    GamePauseMask.Focus();
                    _gameEngine.Pause();
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

        private async void GameMenu_Continue_Button(object sender, RoutedEventArgs e)
        {
            if (_gameEngine.IsGameStarted)
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