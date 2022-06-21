using System;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        private readonly List<GameObject> gameObjectsToClean = new();
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }
        public Player Player { get; }
        public EnemyQueue EnemyQueue { get; }
        public Enemy CurrentEnemy { get; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();
        public List<Character> Characters => GameObjects.OfType<Character>().ToList();
        public List<Bullet> Bullets => GameObjects.OfType<Bullet>().ToList();

        public event Action<Bullet, Character>? TriggerSpawnBulletModel;

        public event Action<Character>? TriggerSpawnModel;

        public event Action<GameObject>? TriggerRemoveModel;

        public event Action<GameObject>? TriggerSpawn;

        public GameEngine(double resX, double resY)
        {
            GameObject.OnCreate += OnGameObjectCreate;
            ResX = resX;
            ResY = resY;
            Player = new Player();
            EnemyQueue = new EnemyQueue(Player);
            CurrentEnemy = EnemyQueue.Clones(0);

            SpawnBullets();
            SpawnCharacters();
        }

        public void UpdateObjects()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObject obj = GameObjects[i];
                if (obj is Player player)
                {
                    if (player.PointsToMoveTo.Count > 0)
                    {
                        player.MoveToPoint();
                    }
                    else
                    {
                        player.Move();
                    }
                }
                else if (obj is Enemy enemy1)
                {
                    enemy1.MoveToPoint(Player.Aim);
                }
                else
                {
                    obj.MoveToPoint();
                }

                if (obj is Enemy enemy)
                {
                    enemy.LocksTarget(Player);
                    //enemy.ShootAtTarget(Player);
                }

                if (obj.IsOutOfBounds())
                {
                    RemoveGameObject(obj);
                }
            }
        }

        public void UpdateCharacters()
        {
            foreach (Character obj in Characters)
            {
                obj.Move();
                if (obj.IsOutOfBounds())
                {
                    RemoveGameObject(obj);
                }

                if (obj is Enemy enemy)
                {
                    enemy.LocksTarget(Player);
                    enemy.ShootAtTarget(Player);
                }
            }
        }

        public void UpdateBullets()
        {
            foreach (Bullet obj in Bullets)
            {
                obj.Move();
                if (obj.IsOutOfBounds())
                {
                    RemoveGameObject(obj);
                }
            }
        }

        public void CleanGameObjects()
        {
            if (gameObjectsToClean.Count > 0)
            {
                GameObjects.RemoveAll(o => gameObjectsToClean.Contains(o));
                gameObjectsToClean.Clear();
                GC.Collect();
            }
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
                PositionLT = character.CenterPosition,
            };
            if (character.BulletsFired > 0 && character.BulletsFired % new Random().Next(3, 6) == 0) newBullet.SetToTracerRound();
            TriggerSpawnBulletModel?.Invoke(newBullet, character);
        }

        private void OnGameObjectCreate(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            TriggerSpawn?.Invoke(gameObject);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            gameObjectsToClean.Add(gameObject);
            TriggerRemoveModel?.Invoke(gameObject);
        }
    }
}