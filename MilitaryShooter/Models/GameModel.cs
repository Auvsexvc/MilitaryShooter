using System;
using System.Collections.Generic;
using System.Windows;

namespace MilitaryShooter.Models
{
    internal abstract class GameModel
    {
        private readonly GameObject _gameObject;
        public Guid Guid { get; }
        public List<UIElement> UIElements { get; protected set; }

        protected GameModel(GameObject gameObject)
        {
            Guid = gameObject.Guid;
            _gameObject = gameObject;
            UIElements = new List<UIElement>();
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public abstract void Transform();
    }
}