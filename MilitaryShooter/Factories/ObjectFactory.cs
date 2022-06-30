using System;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter
{
    internal class ObjectFactory
    {
        private readonly List<GameObject> _gameObjects;

        public ObjectFactory()
        {
            _gameObjects = (new());
        }

        public T Make<T>(T obj)
        {
            if (obj is GameObject gameObject)
            {
                _gameObjects.Add(gameObject);
                gameObject.Factory = this;
            }
            return (T)Convert.ChangeType(obj, obj!.GetType())!;
        }

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects;
        }

        public void Decommission(GameObject gameObject)
        {
            _gameObjects.Remove(gameObject);
        }

        public void DecommissionAll()
        {
            _gameObjects.Clear();
        }

        public void DecommissionExpired()
        {
            _gameObjects.RemoveAll(o => o.IsExpired);
        }

        public List<Character> GetCharacters()
        {
            return _gameObjects.OfType<Character>().ToList();
        }
    }
}