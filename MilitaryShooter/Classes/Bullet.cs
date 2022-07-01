namespace MilitaryShooter.Classes
{
    internal class Bullet : Projectile
    {
        private const double DefaultDamage = 25;
        private const double DefaultHeight = 2;
        private const double DefaultSpeed = 50;
        private const int DefaultSpray = 6;
        private const double DefaultTrailH = 2 * DefaultHeight;
        private const double DefaultTrailW = 5 * DefaultWidth;
        private const double DefaultWidth = 10;
        private (int x, int y) _spray;

        public bool IsTracer { get; private set; }

        public (double W, double H) Trail { get; private set; }

        public Bullet()
        {
            Speed = DefaultSpeed + (GameStatic.rand.NextDouble() * 10);
            Damage = DefaultDamage;
            Width = DefaultWidth + GameStatic.rand.Next(0, 11);
            Height = DefaultHeight;
            Trail = (DefaultTrailW, DefaultTrailH);
            _spray = (GameStatic.rand.Next(-DefaultSpray, DefaultSpray + 1), GameStatic.rand.Next(-DefaultSpray, DefaultSpray + 1));
            SetToTracerRound();
        }

        public void SetToTracerRound()
        {
            if (Shooter?.BulletsFired > 0 && Shooter.BulletsFired % GameStatic.rand.Next(3, 6) == 0)
            {
                IsTracer = true;
                Trail = (Trail.W * 4, Trail.H * 2);
                Speed *= 1 - 0.15;
                _spray = (_spray.x * 2, _spray.y * 2);
            }
        }

        protected override void MoveToPoint()
        {
            if (!IsOutOfBounds())
            {
                Displacement(Source, (Target.X + _spray.x, Target.Y + _spray.y));
            }
            else
            {
                RemoveGameObject();
            }
        }
    }
}