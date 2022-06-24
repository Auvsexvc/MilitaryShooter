using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        private const int maxDelay = 16;
        private const int minDelay = 16;

        private readonly List<GameObject> gameObjectsToClean = new();
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }
        public Player Player { get; set; }
        public EnemyQueue EnemyQueue { get; }
        public Enemy CurrentEnemy { get; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();
        public List<Character> Characters => GameObjects.OfType<Character>().ToList();
        public List<Bullet> Bullets => GameObjects.OfType<Bullet>().ToList();
        public bool Paused { get; set; }

        public event Action<Bullet, Character>? TriggerSpawnBulletModel;

        public event Action<Character>? TriggerSpawnModel;

        public event Action<GameObject>? TriggerRemoveModel;

        public event Action<GameObject>? TriggerSpawn;

        public event Action? GameRestarted;

        public event Action? DrawObjects;

        public event Action? DrawLinesOfFire;

        public event Action? UpdateLabels;

        public GameEngine(double resX, double resY)
        {
            GameObject.OnCreate += OnGameObjectCreate;
            ResX = resX;
            ResY = resY;
            Player = new Player();
            EnemyQueue = new EnemyQueue();
            CurrentEnemy = EnemyQueue.Clones(0);

            //SpawnCharacters();
        }

        public async Task GameLoop()
        {
            while (!Paused)
            {
                int delay = Math.Max(minDelay, maxDelay);
                await Task.Delay(delay);

                DrawObjects!();

                UpdateObjects();
                DrawLinesOfFire!();
                UpdateLabels!();
                CleanGameObjects();
            }
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
                else if (obj is Enemy enemy)
                {
                    enemy.LocksTarget(Player);
                    enemy.ShorteningDistanceToTarget(Player);
                    enemy.ShootAtTarget(Player);
                }
                else if (obj is Bullet bullet)
                {
                    GameObject? collider = bullet.CheckCollisions(GameObjects, Bullets);
                    if (collider != null && collider != bullet.Shooter)
                    {
                        collider.Health -= 25;
                        RemoveGameObject(obj);
                    }
                    else
                    {
                        bullet.MoveToPoint();
                    }
                }

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
            character.FireBullet += SpawnBulletFiredBy;
        }

        private void SpawnBulletFiredBy(Character character)
        {
            Bullet newBullet = new()
            {
                Target = character.Aim,
                Source = character.CenterPosition,
                PositionLT = character.CenterPosition,
                Shooter = character
            };
            if (character.BulletsFired > 0 && character.BulletsFired % GameStatic.rand.Next(3, 6) == 0)
            {
                newBullet.SetToTracerRound();
            }

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

        public void Pause()
        {
            Paused = true;
        }

        public void UnPause()
        {
            Paused = false;
        }

        public void Reset()
        {
            GameObjects.Clear();
            GameRestarted?.Invoke();
        }
    }
}