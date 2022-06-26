using System;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace MilitaryShooter.Models
{
    internal abstract class GameObjectModel
    {
        public static List<GameObjectModel> models = new();
        public static List<GameObjectModel> Models { get => models; set => models = value; }
        public Guid Guid { get; set; }
        public Type Type { get; set; }

        public List<Shape> Shapes { get; set; } = new();

        protected GameObjectModel(GameObject gameObject)
        {
            Guid = gameObject.Guid;
            Type = gameObject.GetType();
            models.Add(this);
        }
    }
}