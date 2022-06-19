using System;

namespace MilitaryShooter
{
    internal abstract class Character : GameObject
    {
        public string Name { get; set; }
        public double Speed { get; set; } = 3;
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


        public void Fire()
        {
            TriggerSpawnBullet?.Invoke(this);
        }

        public (double X, double Y) Move()
        {
            if (MoveLeft && Position.X - Width > 0 )
            {
                Position = (Position.X - Speed, Position.Y);
            }
            if (MoveRight && Position.X < GameEngine.ResX -  Width)
            {
                Position = (Position.X + Speed, Position.Y);
            }
            if (MoveUp && Position.Y - Height > 0)
            {
                Position = (Position.X, Position.Y - Speed * (GameEngine.ResY / GameEngine.ResX));
            }
            if (MoveDown && Position.Y < GameEngine.ResY - Height)
            {
                Position = (Position.X, Position.Y + Speed * (GameEngine.ResY / GameEngine.ResX));
            }
            return Position;
        }
    }
}