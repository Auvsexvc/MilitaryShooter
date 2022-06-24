using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MilitaryShooter
{
    internal abstract class Character : GameObject
    {
        private const double DefaultSpeed = 2.0;
        private const double DefaultCharacterSide = 32;
        protected const int DefaultRateOfFire = 1000;
        protected const double DefaultRangeOfView = 500;

        public (double X, double Y) Aim { get; set; }

        public double Angle
        {
            get
            {
                double angle = Math.Atan((Aim.Y - CenterPosition.Y) / (Aim.X - CenterPosition.X)) * 180 / Math.PI;
                if (Aim.X - CenterPosition.X < 0)
                {
                    angle += 180;
                }
                else if (angle < 0)
                {
                    angle = 360 + angle;
                }

                return angle;
            }
        }

        public double Radians => Math.Atan((Aim.Y - CenterPosition.Y) / (Aim.X - CenterPosition.X));
        public List<(double X, double Y)> PointsToMoveTo { get; set; } = new();
        public (double X, double Y) CurrentMoveToPoint => PointsToMoveTo.FirstOrDefault();
        public override double Speed { get; protected set; }
        public override double Width { get; protected set; }
        public override double Height { get; protected set; }
        public double RangeOfView { get; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public int BulletsFired { get; set; }
        public int RateOfFire { get; set; }
        public Stopwatch Stopwatch { get; protected set; } = new();

        public virtual event Action<Character>? FireBullet;

        protected Character()
        {
            Speed = DefaultSpeed;
            Width = DefaultCharacterSide;
            Height = DefaultCharacterSide;
            RateOfFire = DefaultRateOfFire;
            RangeOfView = DefaultRangeOfView;
        }

        public void SetAim((double X, double Y) aim)
        {
            Aim = (aim.X > GameEngine.ResX ? GameEngine.ResX : aim.X, aim.Y > GameEngine.ResY ? GameEngine.ResY : aim.Y);
        }

        public void Shoot()
        {
            
            FireBullet?.Invoke(this);
            BulletsFired++;
        }

        protected override (double X, double Y) Displacement((double X, double Y) source, (double X, double Y) target)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = Speed;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            return (PositionLT.X + aPrim, PositionLT.Y + bPrim);
        }

        protected (double X, double Y) MaxRangePointTowardTarget((double X, double Y) source, (double X, double Y) target, double distance)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = distance;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            return (PositionLT.X + aPrim, PositionLT.Y + bPrim);
        }

        public override void Move()
        {
            (double x, double y) = Displacement(PositionLT, Aim);
            double moveAngle = 0;
            double moveRadians = moveAngle * Math.PI / 180;
            (double X, double Y) NewPositionLT;

            if (MoveLeft)
            {
                moveAngle = (-90);
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
                moveAngle = (-45);
                moveRadians += moveAngle * Math.PI / 180;
            }
            if (MoveRight && MoveDown)
            {
                moveAngle = (-135);
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
        }

        public void AltMove()
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
            if (PointsToMoveTo.Count > 0)
            {
                (double x, double y) d = Displacement(PositionLT, PointsToMoveTo[0]);

                if (Math.Abs(PositionLT.X - PointsToMoveTo[0].X) <= Width / 2 && Math.Abs(PositionLT.Y - PointsToMoveTo[0].Y) <= Height / 2)
                {
                    PointsToMoveTo.RemoveAt(0);
                }

                if (IsMoveOutOfBounds(d))
                {
                    return;
                }
                PositionLT = d;
            }
        }

        public void MoveToPoint((double X, double Y) p)
        {
            (double x, double y) d = Displacement(PositionLT, p);

            if (IsMoveOutOfBounds(d))
            {
                return;
            }
            PositionLT = d;
        }

        public void MoveToPoint(Character target)
        {
            (double x, double y) d = Displacement(PositionLT, target.CenterPosition);

            if (IsMoveOutOfBounds(d))
            {
                return;
            }
            PositionLT = d;
        }

        public void SetPath((double, double) p)
        {
            PointsToMoveTo.Clear();
            PointsToMoveTo.Add(p);
        }

        public bool IsMoveOutOfBounds((double X, double Y) valueTuple) =>
            valueTuple.X < 0 || valueTuple.X > GameEngine.ResX - Width || (valueTuple.Y < 0 || valueTuple.Y > GameEngine.ResY - Height);

        public bool IsTargetInTheRangeOfView(GameObject gameObject)
        {
            return Math.Sqrt(Math.Pow(gameObject.CenterPosition.X - CenterPosition.X, 2) + Math.Pow(gameObject.CenterPosition.Y - CenterPosition.Y, 2)) <= RangeOfView;
        }
    }
}