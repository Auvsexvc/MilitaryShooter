using System;

namespace MilitaryShooter
{
    internal class Enemy : Character, ICloneable
    {
        private readonly Random rand = new();

        public Enemy()
        {
            Name = "Enemy";
            Speed = rand.Next(1, 4) + rand.NextDouble();
            PositionLT = (rand.Next(0, (int)GameEngine.ResX) - (Width / 2), rand.Next(0, (int)GameEngine.ResY) - (Height / 2));
            Aim = (rand.Next(0, (int)GameEngine.ResX), rand.Next(0, (int)GameEngine.ResY));
        }

        public Enemy(Character character) : this()
        {
            Aim = character.CenterPosition;
        }

        public Enemy(double x, double y) : this()
        {
            PositionLT = (x, y);
        }

        public Enemy(double x, double y, Character character) : this(x, y)
        {
            Aim = character.CenterPosition;
        }

        public object Clone()
        {
            var clone = (Enemy)MemberwiseClone();
            clone.Guid = Guid.NewGuid();
            clone.PositionLT = (rand.Next(0, (int)GameEngine.ResX) - (Width / 2), rand.Next(0, (int)GameEngine.ResY) - (Height / 2));

            return clone;
        }

        public void LocksTarget(Character target)
        {
            Aim = target.CenterPosition;
        }

        public void ShootAtTarget(Character target)
        {
            Aim = target.CenterPosition;
            this.Shoot();
        }
    }
}