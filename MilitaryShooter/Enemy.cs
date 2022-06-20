using System;

namespace MilitaryShooter
{
    internal class Enemy : Character
    {
        private readonly Random rand = new();

        public Enemy()
        {
            Name = "Enemy";
            Speed = 3;
            PositionLT = (rand.Next(0, (int)GameEngine.ResX) - (Width / 2), rand.Next(0, (int)GameEngine.ResY) - (Height / 2));
            Aim = (rand.Next(0, (int)GameEngine.ResX), rand.Next(0, (int)GameEngine.ResY));
        }

        public Enemy(double x, double y)
        {
            Name = "Enemy";
            Speed = 3;
            PositionLT = (x, y);
        }
    }
}