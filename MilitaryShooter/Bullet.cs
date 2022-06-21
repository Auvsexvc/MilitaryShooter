using System;

namespace MilitaryShooter
{
    internal class Bullet : GameObject
    {
        private const double DefaultSpeed = 100;
        private const double DefaultDamage = 25;

        public new double Speed { get; }
        public double Damage { get; }
        public (double X, double Y) Source { get; set; }
        public (double X, double Y) Target { get; set; }

        public Bullet()
        {
            Speed = DefaultSpeed;
            Damage = DefaultDamage;
        }

        public override void Move()
        {
        }

        public override void MoveToPoint()
        {
            Displacement(Source, Target);
        }

        protected override (double X, double Y) Displacement((double X, double Y) source, (double X, double Y) target)
        {
            double c = Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
            double a = target.X - source.X;
            double b = target.Y - source.Y;
            double cPrim = Speed;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            return PositionLT = (PositionLT.X + aPrim, PositionLT.Y + bPrim);
        }
    }
}