using System;
using System.Collections.Generic;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        public static double ResX { get; set; }
        public static double ResY { get; set; }
        public Player Player { get; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();

        public event Action<Bullet>? TriggerSpawnBulletModel;


        public GameEngine()
        {
            Player = new Player();
            Player.TriggerSpawnBullet += SpawnBullet;
        }

        public void SpawnBullet(Character character)
        {
            Bullet newBullet = new()
            {
                Target = character.Aim,
                Source = character.Position
            };
            GameObjects.Add(newBullet);
            TriggerSpawnBulletModel?.Invoke(newBullet);
        }

        public void RemoveGameObject(GameObject obj)
        {
            GameObjects.Remove(obj);
        }

    }
}