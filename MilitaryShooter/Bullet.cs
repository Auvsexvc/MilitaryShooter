namespace MilitaryShooter
{
    internal class Bullet : Projectile
    {
        private const double DefaultSpeed = 50;
        private const double DefaultDamage = 25;
        private const double DefaultWidth = 10;
        private const double DefaultHeight = 2;
        private const double DefaultTrailW = 5 * DefaultWidth;
        private const double DefaultTrailH = 2 * DefaultHeight;

        public double Damage { get; }

        public override double Speed { get; protected set; }
        public override double Width { get; protected set; }
        public override double Height { get; protected set; }
        public (double W, double H) Trail { get; set; }
        public Character? Shooter { get; set; }
        public bool IsTracer { get; private set; }

        public Bullet()
        {
            Speed = DefaultSpeed + (GameStatic.rand.NextDouble() * 10);
            Damage = DefaultDamage;
            Width = DefaultWidth + GameStatic.rand.Next(0, 11);
            Height = DefaultHeight;
            Trail = (DefaultTrailW, DefaultTrailH);
        }

        public void SetToTracerRound()
        {
            IsTracer = true;
            Trail = (Trail.W * 6, Trail.H * 2);
            Speed *= (1 - 0.25);
        }
    }
}