using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MilitaryShooter.Classes
{
    internal abstract class Character : GameObject
    {
        protected const double DefaultRangeOfFire = 700;
        protected const double DefaultRangeOfView = 500;
        protected const int DefaultRateOfFire = 1000;
        protected const double DefaultRotationMultiplier = 1.5;
        private const double DefaultCharacterSide = 32;
        private const double DefaultSpeed = 2.0;
        public (double X, double Y) Aim { get; set; }

        public double AimDistance => DistanceMeter(CenterPosition, Aim);

        public double Angle => GetAngle();

        public int BulletsFired { get; protected set; }

        public double CurrentAngle { get; set; }

        public bool LaserAssistance { get; protected set; }

        public List<(double X, double Y)> PointsToMoveTo { get; set; }

        public double RangeOfFire { get; protected set; }

        public double RangeOfView { get; protected set; }

        public int RateOfFire { get; protected set; }

        public double RotationSpeed => Speed * DefaultRotationMultiplier;

        public Stopwatch Stopwatch { get; }

        public event Action? Death;

        public event Action<Character, Projectile>? Fire;

        protected Character()
        {
            Speed = DefaultSpeed;
            Width = DefaultCharacterSide;
            Height = DefaultCharacterSide;
            RateOfFire = DefaultRateOfFire;
            RangeOfView = DefaultRangeOfView;
            RangeOfFire = DefaultRangeOfFire;
            PointsToMoveTo = new List<(double X, double Y)>();
            Stopwatch = new Stopwatch();
            LaserAssistance = false;
        }

        public void AimAt((double, double) target)
        {
            SetAim(target);
            Rotate();
        }

        public void ClearWaypoints()
        {
            PointsToMoveTo.Clear();
        }

        public (double X, double Y) MaxRangePointTowardTarget((double X, double Y) source, (double X, double Y) target, double distance)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = distance;
            double aPrim = a * cPrim / c;
            double bPrim = b * cPrim / c;

            return (CenterPosition.X + aPrim, CenterPosition.Y + bPrim);
        }

        public void MoveToPoint()
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

        public void SetWaypoint((double, double) p)
        {
            PointsToMoveTo.Clear();
            PointsToMoveTo.Add(p);
        }

        public void ShootROF()
        {
            if (Stopwatch.ElapsedMilliseconds >= RateOfFire)
            {
                Stopwatch.Stop();
                Stopwatch.Reset();
            }
            if (!Stopwatch.IsRunning)
            {
                Shoot();
                Stopwatch.Start();
            }
        }

        public override void TakeDamage(double damage)
        {
            Health -= (int)damage;
            if (IsKilled())
            {
                Death?.Invoke();
            }
        }

        public void ThrowGrenade()
        {
            if (Rotate())
            {
                Fire?.Invoke(this, Creator.Make(new Grenade
                {
                    Target = Aim,
                    Source = CenterPosition,
                    PositionLT = CenterPosition,
                    Shooter = this
                }));
            }
        }

        protected (double X, double Y) Displacement((double X, double Y) source, (double X, double Y) target)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = Speed;
            double aPrim = a * cPrim / c;
            double bPrim = b * cPrim / c;

            return (PositionLT.X + aPrim, PositionLT.Y + bPrim);
        }

        protected bool IsMoveOutOfBounds((double X, double Y) valueTuple) =>
            valueTuple.X < 0 || valueTuple.X > GameEngine.ResX - Width || valueTuple.Y < 0 || valueTuple.Y > GameEngine.ResY - Height;

        protected bool IsTargetInTheRangeOfFire(GameObject gameObject) =>
            Math.Sqrt(Math.Pow(gameObject.CenterPosition.X - CenterPosition.X, 2) + Math.Pow(gameObject.CenterPosition.Y - CenterPosition.Y, 2)) <= RangeOfFire;

        protected bool IsTargetInTheRangeOfView(GameObject gameObject) =>
            Math.Sqrt(Math.Pow(gameObject.CenterPosition.X - CenterPosition.X, 2) + Math.Pow(gameObject.CenterPosition.Y - CenterPosition.Y, 2)) <= RangeOfView;

        protected bool Rotate()
        {
            CurrentAngle %= 360;
            if (Angle - RotationSpeed > CurrentAngle)
            {
                if (Angle - CurrentAngle < 180)
                {
                    CurrentAngle += RotationSpeed;
                    if (CurrentAngle >= 360)
                    {
                        CurrentAngle = 360 - CurrentAngle;
                    }
                }
                else
                {
                    CurrentAngle -= RotationSpeed;
                    if (CurrentAngle < 0)
                    {
                        CurrentAngle = 360 + CurrentAngle;
                    }
                }
                return false;
            }
            else if (Angle + RotationSpeed < CurrentAngle)
            {
                if (CurrentAngle - Angle < 180)
                {
                    CurrentAngle -= RotationSpeed;
                    if (CurrentAngle < 0)
                    {
                        CurrentAngle = 360 + CurrentAngle;
                    }
                }
                else
                {
                    CurrentAngle += RotationSpeed;
                    if (CurrentAngle >= 360)
                    {
                        CurrentAngle = 360 - CurrentAngle;
                    }
                }
                return false;
            }
            else
            {
                CurrentAngle = Angle;
                return true;
            }
        }

        protected void SetAim((double X, double Y) aim)
        {
            Aim = (aim.X > GameEngine.ResX ? GameEngine.ResX : aim.X, aim.Y > GameEngine.ResY ? GameEngine.ResY : aim.Y);
        }

        private double GetAngle()
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

        private bool IsKilled() =>
            Health <= 0;

        private void Shoot()
        {
            if (Rotate())
            {
                Fire?.Invoke(this, Creator.Make(new Bullet
                {
                    Target = Aim,
                    Source = CenterPosition,
                    PositionLT = CenterPosition,
                    Shooter = this
                }));
                BulletsFired++;
            }
        }
    }
}