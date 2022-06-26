﻿using System;
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
        protected const double DefaultRangeOfFire = 700;
        protected const double DefaultRotationMultiplier = 1.5;

        private bool _rotationCheck;

        public (double X, double Y) Aim { get; set; }
        public double CurrentAngle { get; set; }
        public override double Angle
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
        public List<(double X, double Y)> PointsToMoveTo { get; set; }
        //public (double X, double Y) CurrentMoveToPoint => PointsToMoveTo.FirstOrDefault();
        public double RotationSpeed => Speed * DefaultRotationMultiplier;
        //public (double X, double Y) MaxRange => MaxRangePointTowardTarget(CenterPosition, Aim, RangeOfView);
        //public bool IsRotated => (Angle - RotationSpeed) > CurrentAngle && (Angle + RotationSpeed) < CurrentAngle;
        public double RangeOfView { get; protected set; }
        public double RangeOfFire { get; protected set; }
        public double AimDistance => DistanceMeter(CenterPosition, Aim);
        public int BulletsFired { get; private set; }
        public int RateOfFire { get; }
        public Stopwatch Stopwatch { get; }

        public event Action<Character>? FireBullet;

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
        }

        public void SetAim((double X, double Y) aim)
        {
            Aim = (aim.X > GameEngine.ResX ? GameEngine.ResX : aim.X, aim.Y > GameEngine.ResY ? GameEngine.ResY : aim.Y);
        }

        public void Shoot()
        {
            if (_rotationCheck)
            {
                FireBullet?.Invoke(this);
                BulletsFired++;
            }
        }

        public void Rotate()
        {
            CurrentAngle %= 360;
            if ((Angle - RotationSpeed) > CurrentAngle)
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
                _rotationCheck = false;
            }
            else if ((Angle + RotationSpeed) < CurrentAngle)
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
                _rotationCheck = false;
            }
            else
            {
                CurrentAngle = Angle;
                _rotationCheck = true;
            }
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

        public (double X, double Y) MaxRangePointTowardTarget((double X, double Y) source, (double X, double Y) target, double distance)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = distance;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            return (CenterPosition.X + aPrim, CenterPosition.Y + bPrim);
        }

        protected static double DistanceMeter((double X, double Y) source, (double X, double Y) target) => Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));

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

        public bool IsTargetInTheRangeOfFire(GameObject gameObject)
        {
            return Math.Sqrt(Math.Pow(gameObject.CenterPosition.X - CenterPosition.X, 2) + Math.Pow(gameObject.CenterPosition.Y - CenterPosition.Y, 2)) <= RangeOfFire;
        }
    }
}