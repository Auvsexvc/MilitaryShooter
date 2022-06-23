using System;
using System.Diagnostics;

namespace MilitaryShooter
{
    internal class Enemy : Character, ICloneable
    {
        private bool shotFired;
        public override double Speed { get; protected set; }

        public Enemy()
        {
            Name = "Enemy";
            Speed = GameStatic.rand.Next(1, 4) + GameStatic.rand.NextDouble();
            PositionLT = (GameStatic.rand.Next((int)Width, (int)GameEngine.ResX) - (int)Width, GameStatic.rand.Next((int)Height, (int)GameEngine.ResY) - (int)Height);
            Aim = (GameStatic.rand.Next(0, (int)GameEngine.ResX), GameStatic.rand.Next(0, (int)GameEngine.ResY));
            Health = GameStatic.rand.Next(25, 201);
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
            clone.PositionLT = (GameStatic.rand.Next(0, (int)GameEngine.ResX) - (Width / 2), GameStatic.rand.Next(0, (int)GameEngine.ResY) - (Height / 2));

            return clone;
        }

        public void LocksTarget(Character target)
        {
            Aim = target.CenterPosition;
        }

        public void ShootAtTarget(Character target)
        {
            Aim = target.CenterPosition;
            if (!Stopwatch.IsRunning)
            {
                Stopwatch.Start();
            }

            if (Stopwatch.ElapsedMilliseconds % DefaultRateOfFire == 0 && !shotFired)
            {
                Shoot();
                shotFired = true;
            }
            if (Stopwatch.ElapsedMilliseconds > DefaultRateOfFire)
            {
                Stopwatch.Stop();
                Stopwatch.Reset();
                shotFired = false;
            }

        }
    }
}