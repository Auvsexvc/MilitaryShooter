using System;
using System.Diagnostics;

namespace MilitaryShooter
{
    internal class Grenade : Projectile
    {
        private const double DefaultSpeed = 5;
        private const double DefaultDamage = 100;
        private const double DefaultWidth = 16;
        private const double DefaultHeight = 16;
        private const double DefaultRange = 200;
        private const double DefaultBlastRadius = 150;
        private const double DefaultBlastSpeed = 10;

        public double BlastRadius { get; set; }
        public double BlastSpeed { get; set; }
        public bool Exploded { get; set; }
        public Stopwatch Stopwatch { get; }
        public double CurrentBlastRadius { get; set; }

        public event Action<Grenade>? TriggerModelResize;

        public Grenade()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            Damage = DefaultDamage;
            Speed = DefaultSpeed;
            Range = DefaultRange;
            BlastRadius = DefaultBlastRadius;
            BlastSpeed = DefaultBlastSpeed;
            CurrentBlastRadius = 1;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public override void MoveToPoint()
        {
            if (DistanceCovered <= Range && DistanceCovered <= DistanceMeter(Source, Target))
            {
                if (DistanceMeter(Source, Target) > DistanceMeter(Source, MaxRangePointTowardTarget()))
                {
                    Displacement(Source, MaxRangePointTowardTarget());
                }
                else if (DistanceMeter(Source, Target) < DistanceMeter(Source, MaxRangePointTowardTarget()))
                {
                    Displacement(Source, Target);
                }
            }
            else
            {
                TimeToExplode();
            }
        }

        private void TimeToExplode()
        {
            if (Stopwatch.ElapsedMilliseconds >= 5000)
            {
                Explode();
            }
        }

        private void Explode()
        {
            if (CurrentBlastRadius <= BlastRadius && !Exploded)
            {
                CurrentBlastRadius += BlastSpeed;
                TriggerModelResize?.Invoke(this);
            }
            else
            {
                Exploded = true;
                FallDown();
            }
        }

        private void FallDown()
        {
            if (CurrentBlastRadius >= 0)
            {
                CurrentBlastRadius -= BlastSpeed * 2;
                TriggerModelResize?.Invoke(this);
            }
            else
            {
                IsExpired = true;
            }
        }
    }
}