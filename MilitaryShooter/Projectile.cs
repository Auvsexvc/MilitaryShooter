using System;

namespace MilitaryShooter
{
    internal abstract class Projectile : GameObject
    {
        public (double X, double Y) Source { get; set; }
        public (double X, double Y) Target { get; set; }
        public double Damage { get; protected set; }
        public double Range { get; set; }
        public Character? Shooter { get; set; }
        public double DistanceCovered { get; set; }

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
            DistanceCovered += cPrim;

            return PositionLT = (PositionLT.X + aPrim, PositionLT.Y + bPrim);
        }

        protected (double X, double Y) MaxRangePointTowardTarget()
        {
            double c = Math.Sqrt(Math.Pow(Target.X - Source.X, 2) + Math.Pow(Target.Y - Source.Y, 2));
            double a = Target.X - Source.X;
            double b = Target.Y - Source.Y;
            double cPrim = Range;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            return (CenterPosition.X + aPrim, CenterPosition.Y + bPrim);
        }

        protected static double DistanceMeter((double X, double Y) source, (double X, double Y) target) =>
            Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
    }
}