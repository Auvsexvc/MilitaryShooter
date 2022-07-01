using System;
using System.Collections.Generic;
using System.Windows;

namespace MilitaryShooter.Classes
{
    internal abstract class GameObject
    {
        public (double X, double Y) CenterPosition => GetCenter();

        public GameObjectCreator Creator { get; set; }

        public Guid Guid { get; protected set; }

        public int Health { get; protected set; }

        public double Height { get; set; }

        public bool IsExpired { get; set; }

        public string? Name { get; protected set; }

        public (double X, double Y) PositionLT { get; set; }

        public double Speed { get; protected set; }

        public double Width { get; set; }

        public event Action<GameObject>? TriggerRemoveObject;

        protected GameObject()
        {
            Guid = Guid.NewGuid();
            IsExpired = false;
            Creator = new GameObjectCreator();
        }

        public virtual void TakeDamage(double damage)
        {
            Health -= (int)damage;
        }

        public abstract void Update();

        protected static double DistanceMeter((double X, double Y) source, (double X, double Y) target)
        {
            return Math.Sqrt(Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2));
        }

        protected virtual GameObject? CheckCollisions()
        {
            return GetGameObjects().Find(obj => IntersectsWith(obj));
        }

        protected List<GameObject> GetGameObjects()
        {
            return Creator.GetGameObjects();
        }

        protected virtual bool IntersectsWith(GameObject gameObject)
        {
            return new Rect(PositionLT.X, PositionLT.Y, Width, Height).IntersectsWith(new Rect(gameObject.PositionLT.X, gameObject.PositionLT.Y, gameObject.Width, gameObject.Height));
        }

        protected bool IsOutOfBounds()
        {
            return CenterPosition.X + Width < 0 || PositionLT.X - Width > GameEngine.ResX || PositionLT.Y + Width < 0 || PositionLT.Y - Width > GameEngine.ResY;
        }

        protected void RemoveGameObject()
        {
            IsExpired = true;
            TriggerRemoveObject?.Invoke(this);
        }

        private (double X, double Y) GetCenter()
        {
            return (PositionLT.X + (Width / 2), PositionLT.Y + (Height / 2));
        }
    }
}