using MilitaryShooter.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter
{
    internal class GameObjectCreator
    {
        private readonly List<GameObject> _gameObjects;

        public GameObjectCreator()
        {
            _gameObjects = new();
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

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects;
        }

        public T Make<T>(T obj)
        {
            if (obj is GameObject gameObject)
            {
                _gameObjects.Add(gameObject);
                gameObject.Creator = this;
            }
            return (T)Convert.ChangeType(obj, obj!.GetType())!;
        }
    }
}