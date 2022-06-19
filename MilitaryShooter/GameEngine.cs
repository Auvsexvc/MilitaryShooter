using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        public static double ResX { get; set; }
        public static double ResY { get; set; }
        public Player Player { get; private set; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();

        public event Action<Bullet>? TriggerSpawnBulletModel;

        public event Action<GameObject>? TriggerSpawnModel;
        public event Action<GameObject>? TriggerRemoveModel;

        public GameEngine(double resX, double resY)
        {
            ResX = resX;
            ResY = resY;
            CreatePlayer();
            GameObject.OnCreate += Spawn;
            Player!.TriggerSpawnBullet += SpawnBulletFiredBy;
        }

        private void CreatePlayer()
        {
            Player = new Player();
            GameObjects.Add(Player);
        }

        public void SpawnBulletFiredBy(Character character)
        {
            Bullet newBullet = new()
            {
                Target = character.Aim,
                Source = character.CenterPosition,
                PositionLT = character.CenterPosition
            };
            //GameObjects.Add(newBullet);
            TriggerSpawnBulletModel?.Invoke(newBullet);
        }

        public void UpdateBulletPos(Bullet bullet)
        {
            bullet.Travel();
            if (bullet.IsOutOfBounds())
            {
                RemoveGameObject(bullet);
            }
        }

        public void Spawn(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            if (gameObject.GetType() != typeof(Bullet))
            {
                TriggerSpawnModel?.Invoke(gameObject);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
            TriggerRemoveModel?.Invoke(gameObject);
        }
    }
}