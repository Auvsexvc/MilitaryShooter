using System;

namespace MilitaryShooter
{
    internal class Bullet : GameObject
    {
        public double Speed { get; set; } = 100;
        public double Damage { get; set; } = 25;
        public (double X, double Y) Source { get; set; }
        public (double X, double Y) Target { get; set; }

        public (double X, double Y) Travel((double X, double Y) p)
        {
            double x, y;
            double c = Math.Sqrt(Math.Pow(Target.X - Source.X, 2) + Math.Pow(Target.Y - Source.Y, 2));
            double a = Target.X - Source.X;
            double b = Target.Y - Source.Y;
            double cPrim = Speed;
            double aPrim = (a * cPrim) / c;
            double bPrim = (b * cPrim) / c;

            x = (p.X + aPrim);
            y = (p.Y + bPrim);

            return (x, y);
        }
    }
}