using System;

namespace MilitaryShooter
{
    internal abstract class Character : GameObject
    {
        private const double DefaultSpeed = 3.0;
        private const double DefaultCharacterSide = 32;

        public (double X, double Y) Aim { get; set; }

        public double Direction
        {
            get
            {
                double angle = Math.Atan((Aim.Y - CenterPosition.Y) / (Aim.X - CenterPosition.X)) * 180 / Math.PI;
                if (Aim.X - CenterPosition.X < 0)
                {
                    angle += 180;
                }
                return angle;
            }
        }

        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }

        public event Action<Character>? TriggerSpawnBullet;

        protected Character()
        {
            Speed = DefaultSpeed;
            Width = DefaultCharacterSide;
            Height = DefaultCharacterSide;
        }

        public void SetAim((double X, double Y) aim)
        {
            Aim = (aim.X > GameEngine.ResX ? GameEngine.ResX : aim.X, aim.Y > GameEngine.ResY ? GameEngine.ResY : aim.Y);
        }

        public (double X, double Y) SetCenterPosition()
        {
            return (PositionLT.X + (Width / 2), PositionLT.Y + (Height / 2));
        }

        public void Shoot()
        {
            TriggerSpawnBullet?.Invoke(this);
        }

        public override void Move()
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
        }

        public override void MoveToPoint()
        {
            double x, y;
            double c = Math.Sqrt(Math.Pow(Aim.X - PositionLT.X, 2) + Math.Pow(Aim.Y - PositionLT.Y, 2));
            double a = Aim.X - PositionLT.X;
            double b = Aim.Y - PositionLT.Y;
            double cPrim = Speed;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            x = (PositionLT.X + aPrim);
            y = (PositionLT.Y + bPrim);
            PositionLT = (x, y);
        }

        public void MoveToPoint((double X, double Y) p)
        {
            double x, y;
            double c = Math.Sqrt(Math.Pow(p.X - PositionLT.X, 2) + Math.Pow(p.Y - PositionLT.Y, 2));
            double a = p.X - PositionLT.X;
            double b = p.Y - PositionLT.Y;
            double cPrim = Speed;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            x = (PositionLT.X + aPrim);
            y = (PositionLT.Y + bPrim);
            PositionLT = (x, y);
        }
    }
}