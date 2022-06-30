using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MilitaryShooter.Classes
{
    internal class Grenade : Projectile
    {
        private const double DefaultBlastRadius = 300;
        private const double DefaultBlastSpeed = 50;
        private const double DefaultDamage = 15;
        private const int DefaultFuserTime = 5000;
        private const double DefaultHeight = 16;
        private const double DefaultRange = 300;
        private const double DefaultSpeed = 8;
        private const double DefaultWidth = 16;
        public Grenade()
        {
            Width = DefaultWidth;
            Height = DefaultHeight;
            Damage = DefaultDamage;
            Speed = DefaultSpeed;
            Range = DefaultRange;
            BlastRadius = DefaultBlastRadius;
            BlastSpeed = DefaultBlastSpeed;
            CurrentBlastRadius = Width;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public event Action<Grenade>? TriggerModelResize;

        public double BlastRadius { get; set; }
        public double BlastSpeed { get; set; }
        public double CurrentBlastRadius { get; set; }
        public bool Exploded { get; set; }
        public Stopwatch Stopwatch { get; }
        public override void Update()
        {
            MoveToPoint();
            if (Stopwatch.ElapsedMilliseconds >= DefaultFuserTime)
            {
                foreach (var collider in CheckBlastCollisions())
                {
                    if (collider != null && collider is not Projectile)
                    {
                        collider.TakeDamage(Damage);
                    }
                }
            }
        }

        protected override void MoveToPoint()
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

        private List<GameObject> CheckBlastCollisions()
        {
            List<GameObject> retList = new();
            retList.AddRange(GetGameObjects().Where(obj => IsInBlastRadius(obj)));

            return retList;
        }

        private void Explode()
        {
            if (CurrentBlastRadius < BlastRadius && !Exploded)
            {
                CurrentBlastRadius += BlastSpeed;
                TriggerModelResize?.Invoke(this);
            }
            else if (CurrentBlastRadius >= BlastRadius && !Exploded)
            {
                Exploded = true;
            }
            else
            {
                FallDown();
            }
        }

        private void FallDown()
        {
            if (CurrentBlastRadius >= 0)
            {
                CurrentBlastRadius -= BlastSpeed;
                TriggerModelResize?.Invoke(this);
            }
            else
            {
                RemoveGameObject();
            }
        }

        private bool IsInBlastRadius(GameObject gameObject)
        {
            return DistanceMeter(CenterPosition, gameObject.CenterPosition) <= CurrentBlastRadius;
        }

        private void TimeToExplode()
        {
            if (Stopwatch.ElapsedMilliseconds >= DefaultFuserTime)
            {
                Explode();
            }
        }
    }
}