using MilitaryShooter.Interfaces;
using System;

namespace MilitaryShooter.Classes
{
    internal class Player : Character, IPlayer
    {
        public Player()
        {
            Name = "PlayerOne";
            Speed = 6;
            PositionLT = (GameEngine.ResX / 2.0, GameEngine.ResY / 2.0);
            Health = 500;
            LaserAssistance = false;
            AlternativeControls = false;
        }

        public event Action? RestartedGame;

        public event Action? SwitchedGameMenu;

        public event Action? SwitchedGamePause;

        public bool AlternativeControls { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool MoveUp { get; set; }
        public void ContinueGame()
        {
            SwitchedGameMenu?.Invoke();
        }

        public void RestartGame()
        {
            RestartedGame?.Invoke();
        }

        public void SwitchGameMenu()
        {
            SwitchedGameMenu?.Invoke();
        }

        public void SwitchGamePause()
        {
            SwitchedGamePause?.Invoke();
        }

        public void SwitchLaserTargeting()
        {
            LaserAssistance = !LaserAssistance;
        }

        public override void Update()
        {
            if (PointsToMoveTo.Count > 0)
            {
                MoveToPoint();
            }
            else
            {
                if (AlternativeControls)
                {
                    AltMove();
                }
                else
                {
                    Move();
                }
            }
        }

        private void AltMove()
        {
            if (MoveLeft && PositionLT.X > 0)
            {
                PositionLT = (PositionLT.X - Speed, PositionLT.Y);
            }
            if (MoveRight && PositionLT.X < GameEngine.ResX - Width)
            {
                PositionLT = (PositionLT.X + Speed, PositionLT.Y);
            }
            if (MoveUp && PositionLT.Y > 0)
            {
                PositionLT = (PositionLT.X, PositionLT.Y - Speed);
            }
            if (MoveDown && PositionLT.Y < GameEngine.ResY - Height)
            {
                PositionLT = (PositionLT.X, PositionLT.Y + Speed);
            }
            Rotate();
        }

        private void Move()
        {
            (double x, double y) = Displacement(PositionLT, Aim);
            double moveAngle = 0;
            double moveRadians = moveAngle * Math.PI / 180;
            (double X, double Y) NewPositionLT;

            if (MoveLeft)
            {
                moveAngle = -90;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveLeft && MoveUp)
            {
                moveAngle = 45;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveLeft && MoveDown)
            {
                moveAngle = 135;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveRight)
            {
                moveAngle = 90;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveRight && MoveUp)
            {
                moveAngle = -45;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveRight && MoveDown)
            {
                moveAngle = -135;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveUp)
            {
                moveAngle = 0;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveDown)
            {
                moveAngle = 180;
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (!MoveDown && !MoveUp && !MoveLeft && !MoveRight)
            {
                return;
            }

            NewPositionLT = (((x - PositionLT.X) * Math.Cos(moveRadians)) - ((y - PositionLT.Y) * Math.Sin(moveRadians)), ((x - PositionLT.X) * Math.Sin(moveRadians)) + ((y - PositionLT.Y) * Math.Cos(moveRadians)));
            if (IsMoveOutOfBounds((PositionLT.X + NewPositionLT.X, PositionLT.Y + NewPositionLT.Y)))
            {
                return;
            }

            PositionLT = (PositionLT.X + NewPositionLT.X, PositionLT.Y + NewPositionLT.Y);
            Rotate();
        }
    }
}