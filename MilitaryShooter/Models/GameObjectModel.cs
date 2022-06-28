using System;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal abstract class GameObjectModel
    {
        public static List<GameObjectModel> Models { get; set; } = new List<GameObjectModel>();
        public Guid Guid { get; }
        public GameObject GameObject { get; }

        public List<Shape> Shapes { get; protected set; }

        protected GameObjectModel(GameObject gameObject)
        {
            Shapes = new List<Shape>();
            GameObject = gameObject;
            Guid = gameObject.Guid;
            Models.Add(this);
        }

        public abstract void Transform();
    }
}