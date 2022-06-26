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
        private const int DefaultSpray = 6;

        private (int x, int y) spray;

        public double Damage { get; }

        public (double W, double H) Trail { get; private set; }
        public bool IsTracer { get; private set; }
        public Character? Shooter { get; set; }

        public Bullet()
        {
            Speed = DefaultSpeed + (GameStatic.rand.NextDouble() * 10);
            Damage = DefaultDamage;
            Width = DefaultWidth + GameStatic.rand.Next(0, 11);
            Height = DefaultHeight;
            Trail = (DefaultTrailW, DefaultTrailH);
            spray = (GameStatic.rand.Next(-DefaultSpray, DefaultSpray + 1), GameStatic.rand.Next(-DefaultSpray, DefaultSpray + 1));
        }

        public override void MoveToPoint()
        {
            Displacement(Source, (Target.X + spray.x, Target.Y + spray.y));
        }

        public void SetToTracerRound()
        {
            IsTracer = true;
            Trail = (Trail.W * 4, Trail.H * 2);
            Speed *= (1 - 0.15);
            spray = (spray.x * 2, spray.y * 2);
        }
    }
}