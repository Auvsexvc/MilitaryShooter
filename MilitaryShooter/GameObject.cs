using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MilitaryShooter
{
    internal abstract class GameObject : INotifyPropertyChanged
    {
        private const int Margin = 64;

        public Guid Guid { get; protected set; }
        public string? Name { get; protected set; }
        public double Speed { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public virtual double Angle { get; protected set; }
        public int Health { get; protected set; }
        public (double X, double Y) PositionLT { get; set; }
        public (double X, double Y) CenterPosition => (PositionLT.X + (Width / 2), PositionLT.Y + (Height / 2));

        public static event Action<GameObject>? OnCreate;

        public event PropertyChangedEventHandler? PropertyChanged;

        public abstract void MoveToPoint();

        private bool IntersectsWith(GameObject gameObject)
        {
            return new Rect(this.PositionLT.X, this.PositionLT.Y, this.Width, this.Height).IntersectsWith(new Rect(gameObject.PositionLT.X, gameObject.PositionLT.Y, gameObject.Width, gameObject.Height));
        }

        public GameObject? CheckCollisions<T>(List<GameObject> listOfObjectsToTestFor, List<T>? exception = null)
        {
            List<GameObject> list = listOfObjectsToTestFor.Except(new List<GameObject>() { this }).ToList();
            if (exception != null)
            {
                list = list.Except(exception.Cast<GameObject>()).ToList();
            }
            foreach (GameObject obj in list)
            {
                if (this.IntersectsWith(obj))
                {
                    return obj;
                }
            }
            return null;
        }

        public bool IsOutOfBounds() => PositionLT.X < -Margin || PositionLT.X > GameEngine.ResX + Margin || PositionLT.Y < -Margin || PositionLT.Y > GameEngine.ResY + Margin;

        protected GameObject()
        {
            Guid = Guid.NewGuid();
            OnCreate?.Invoke(this);
        }

        protected abstract (double X, double Y) Displacement((double X, double Y) source, (double X, double Y) target);

        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
}