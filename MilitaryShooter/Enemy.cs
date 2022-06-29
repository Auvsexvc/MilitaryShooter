using System;
using System.Linq;

namespace MilitaryShooter
{
    internal class Enemy : Character, ICloneable
    {
        public Enemy()
        {
            Name = "Enemy";
            Speed = GameStatic.rand.Next(1, 4) + GameStatic.rand.NextDouble();
            PositionLT = (GameStatic.rand.Next((int)Width, (int)GameEngine.ResX) - (int)Width, GameStatic.rand.Next((int)Height, (int)GameEngine.ResY) - (int)Height);
            SetAim((GameStatic.rand.Next(0, (int)GameEngine.ResX), GameStatic.rand.Next(0, (int)GameEngine.ResY)));
            Health = GameStatic.rand.Next(25, 201);
            RangeOfFire = DefaultRangeOfFire;
            RateOfFire += 500;
            LaserAssistance = false;
        }

        public Enemy(Character character) : this()
        {
            SetAim(character.CenterPosition);
        }

        public Enemy(double x, double y) : this()
        {
            PositionLT = (x, y);
        }

        public Enemy(double x, double y, Character character) : this(x, y)
        {
            SetAim(character.CenterPosition);
        }

        public override void Update()
        {
            GameObject target = SetNearestTarget();
            AimAt(target.CenterPosition);
            ShorteningDistanceToTarget(target);
            ShootAtTarget(target);
        }

        public object Clone()
        {
            var clone = (Enemy)MemberwiseClone();
            clone.Guid = Guid.NewGuid();
            clone.PositionLT = (GameStatic.rand.Next(0, (int)GameEngine.ResX) - (Width / 2), GameStatic.rand.Next(0, (int)GameEngine.ResY) - (Height / 2));

            return clone;
        }

        private void ShootAtTarget(GameObject target)
        {
            AimAt(target.CenterPosition);
            if (IsTargetInTheRangeOfFire(target))
            {
                ShootROF();
            }
        }

        private void ShorteningDistanceToTarget(GameObject target)
        {
            AimAt(target.CenterPosition);
            if (!IsTargetInTheRangeOfView(target))
            {
                (double X, double Y) maxRangePointTowardTarget = MaxRangePointTowardTarget(this.CenterPosition, target.CenterPosition, RangeOfView);
                MoveToPoint(maxRangePointTowardTarget);
            }
        }

        private GameObject SetNearestTarget()
        {
            return GetGameObjects().Where(o => o is Character && o != this).OrderBy(o => DistanceMeter(CenterPosition, o.CenterPosition)).ThenByDescending(o => o.GetType().Name).First();
        }
    }
}