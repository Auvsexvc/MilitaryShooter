using System;
using System.Collections.Generic;

namespace MilitaryShooter
{
    internal class ObjectFactory
    {
        public List<GameObject> GameObjects { get; set; }

        public ObjectFactory()
        {
            GameObjects = new();
        }

        public T Make<T>(T obj)
        {
            if(obj is GameObject gameObject)
            {
                GameObjects.Add(gameObject);
                gameObject.Factory = this;
            }
            return (T)Convert.ChangeType(obj, obj!.GetType())!;
        }
    }
}