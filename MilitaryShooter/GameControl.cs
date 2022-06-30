using MilitaryShooter.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace MilitaryShooter
{
    internal class GameControl
    {
        private readonly IPlayer _player;

        public GameControl(IPlayer player)
        {
            _player = player;
        }

        public void ContinueGame()
        {
            _player.ContinueGame();
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    _player.MoveLeft = true;
                    _player.ClearWaypoints();
                    break;

                case Key.D:
                    _player.MoveRight = true;
                    _player.ClearWaypoints();
                    break;

                case Key.W:
                    _player.MoveUp = true;
                    _player.ClearWaypoints();
                    break;

                case Key.S:
                    _player.MoveDown = true;
                    _player.ClearWaypoints();
                    break;

                case Key.F:
                    _player.SwitchLaserTargeting();
                    break;

                case Key.G:
                    _player.ThrowGrenade();
                    break;

                case Key.Escape:
                    _player.SwitchGameMenu();
                    break;

                case Key.Space:
                    _player.SwitchGamePause();
                    break;
            }
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    _player.MoveLeft = false;
                    break;

                case Key.D:
                    _player.MoveRight = false;
                    break;

                case Key.W:
                    _player.MoveUp = false;
                    break;

                case Key.S:
                    _player.MoveDown = false;
                    break;
            }
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _player.ShootROF();
                    break;

                case MouseButton.Middle:
                    break;

                case MouseButton.Right:
                    Point position = e.GetPosition((IInputElement)sender);
                    _player.SetWaypoint((position.X, position.Y));
                    break;

                case MouseButton.XButton1:

                    break;

                case MouseButton.XButton2:

                    break;

                default:
                    break;
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition((IInputElement)sender);
            _player.AimAt((position.X, position.Y));
        }
        public void RestartGame()
        {
            _player.RestartGame();
        }
    }
}