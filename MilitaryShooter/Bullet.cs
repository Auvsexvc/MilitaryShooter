namespace MilitaryShooter
{
    internal class Bullet : Projectile
    {
        private const double DefaultSpeed = 100;
        private const double DefaultDamage = 25;
        private const double DefaultWidth = 10;
        private const double DefaultHeight = 2;

        public double Damage { get; }

        public override double Speed { get; protected set; }
        public override double Width { get; protected set; }
        public override double Height { get; protected set; }
        public bool IsTracer { get; private set; }

        public Bullet()
        {
            Speed = DefaultSpeed;
            Damage = DefaultDamage;
            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        public void SetToTracerRound()
        {
            IsTracer = true;
            Width += 40;
            Height++;
            Speed--;
        }
    }
}