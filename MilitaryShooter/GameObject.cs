using System;

namespace MilitaryShooter
{
    internal abstract class GameObject
    {
        public Guid Guid { get; set; }
        public string? Name { get; set; }
        public double Speed { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public (double X, double Y) PositionLT { get; set; }
        public (double X, double Y) CenterPosition => (PositionLT.X + (Width / 2), PositionLT.Y + (Height / 2));

        public static event Action<GameObject>? OnCreate;

        protected GameObject()
        {
            Guid = Guid.NewGuid();
            OnCreate?.Invoke(this);
        }

        public bool IsOutOfBounds() => PositionLT.X < 0 || PositionLT.X > GameEngine.ResX || PositionLT.Y < 0 || PositionLT.Y > GameEngine.ResY;
    }
}