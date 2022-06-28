using System;

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
            Laser = false;
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

        public object Clone()
        {
            var clone = (Enemy)MemberwiseClone();
            clone.Guid = Guid.NewGuid();
            clone.PositionLT = (GameStatic.rand.Next(0, (int)GameEngine.ResX) - (Width / 2), GameStatic.rand.Next(0, (int)GameEngine.ResY) - (Height / 2));

            return clone;
        }

        public void ShootAtTarget(Character target)
        {
            LocksTarget(target);
            if (IsTargetInTheRangeOfFire(target))
            {
                ShootROF();
            }
        }

        public void ShorteningDistanceToTarget(Character target)
        {
            LocksTarget(target);
            if (!IsTargetInTheRangeOfView(target))
            {
                (double X, double Y) maxRangePointTowardTarget = MaxRangePointTowardTarget(this.CenterPosition, target.CenterPosition, RangeOfView);
                MoveToPoint(maxRangePointTowardTarget);
            }
        }
    }
}