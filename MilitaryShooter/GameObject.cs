using System;

namespace MilitaryShooter
{
    internal abstract class GameObject
    {
        private const int margin = 64;
        public Guid Guid { get; protected set; }
        public string? Name { get; set; }
        public abstract double Speed { get; protected set; }
        public abstract double Width { get; protected set; }
        public abstract double Height { get; protected set; }
        public int Health { get; set; }
        public (double X, double Y) PositionLT { get; set; }
        public (double X, double Y) CenterPosition => (PositionLT.X + (Width / 2), PositionLT.Y + (Height / 2));

        public static event Action<GameObject>? OnCreate;

        public abstract void Move();

        public abstract void MoveToPoint();

        protected abstract (double X, double Y) Displacement((double X, double Y) source, (double X, double Y) target);

        protected GameObject()
        {
            Guid = Guid.NewGuid();
            OnCreate?.Invoke(this);
        }

        public bool IsOutOfBounds() => PositionLT.X < -margin || PositionLT.X > GameEngine.ResX + margin || PositionLT.Y < -margin || PositionLT.Y > GameEngine.ResY + margin;
    }
}