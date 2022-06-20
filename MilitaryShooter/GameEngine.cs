using System;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }
        public Player Player { get; }
        public EnemyQueue EnemyQueue { get; }
        public Enemy CurrentEnemy { get; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();
        public List<Character> Characters => GameObjects.OfType<Character>().ToList();

        public event Action<Bullet, Character>? TriggerSpawnBulletModel;

        public event Action<Character>? TriggerSpawnModel;

        public event Action<GameObject>? TriggerRemoveModel;
        public event Action<GameObject>? TriggerSpawn;

        public GameEngine(double resX, double resY)
        {
            GameObject.OnCreate += OnGameObjectCreate;
            ResX = resX;
            ResY = resY;
            EnemyQueue = new EnemyQueue();
            Player = new Player();
            CurrentEnemy = EnemyQueue.Clones(10);

            SpawnBullets();
            SpawnCharacters();
        }

        public void SpawnCharacters()
        {
            foreach (var character in Characters)
            {
                Spawn(character);
            }
        }

        private void Spawn(Character character)
        {
            TriggerSpawnModel?.Invoke(character);
        }

        private void SpawnBullets()
        {
            foreach (var item in Characters)
            {
                item.TriggerSpawnBullet += SpawnBulletFiredBy;
            }
        }

        private void SpawnBulletFiredBy(Character character)
        {
            Bullet newBullet = new()
            {
                Target = character.Aim,
                Source = character.CenterPosition,
                PositionLT = character.CenterPosition
            };
            TriggerSpawnBulletModel?.Invoke(newBullet, character);
        }

        public void UpdateBulletPos(Bullet bullet)
        {
            bullet.Travel();
            if (bullet.IsOutOfBounds())
            {
                RemoveGameObject(bullet);
            }
        }

        private void OnGameObjectCreate(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            TriggerSpawn?.Invoke(gameObject);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
            TriggerRemoveModel?.Invoke(gameObject);
            GC.Collect();
        }
    }
}