using System;
using System.Windows.Controls;

namespace MilitaryShooter
{
    internal abstract class Character : GameObject
    {
        public int Speed { get; set; } = 3;
        public (double X, double Y) Position { get; set; }
        public (double X, double Y) Aim { get; set; }
        public double Direction { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public event Action<Character>? TriggerSpawnBullet;
        public event Action<Character>? TriggerMove;

        public void Fire()
        {
            TriggerSpawnBullet?.Invoke(this);
        }

        public void Move()
        {
            if (MoveLeft)
            {
                Position = (Position.X - Speed, Position.Y);
            }
            if (MoveRight)
            {
                Position = (Position.X + Speed, Position.Y); ;
            }
            if (MoveUp)
            {
                Position = (Position.X, Position.Y - Speed);
            }
            if (MoveDown)
            {
                Position = (Position.X, Position.Y + Speed);
            }
            TriggerMove?.Invoke(this);
        }

    }
}