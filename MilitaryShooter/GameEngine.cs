using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        private const int MaxDelay = 16;
        private const int MinDelay = 16;

        public GameControl? Controls { get; }
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }
        public Player? Player { get; set; }
        public EnemyQueue? EnemyQueue { get; }
        public Enemy? CurrentEnemy { get; }
        public List<GameObject> GameObjects { get; }

        public bool IsGameStarted { get; private set; }
        public bool GameOver { get; private set; }
        public bool Paused { get; private set; }

        public event Action<Projectile, Character>? TriggerSpawnBulletModel;

        public event Action<Character>? TriggerSpawnModel;

        public event Action<GameObject>? TriggerRemoveModel;

        public event Action<GameObject>? TriggerSpawn;

        public event Action? TriggerPlayerDeath;

        public event Action? GameRestarted;

        public event Action? DrawObjects;

        public event Action? DrawLinesOfFire;

        public event Action? UpdateLabels;

        public event Action? TriggerGamePause;

        public event Action? TriggerGameUnpause;

        public event Action? TriggerGameMenuOpen;
        public event Action? TriggerGameMenuClose;

        public GameEngine()
        {
            IsGameStarted = false;
            GameObjects = new List<GameObject>();
        }

        public GameEngine(double resX, double resY)
        {
            ResX = resX;
            ResY = resY;
            GameObjects = new List<GameObject>();
            GameObject.OnCreate += OnGameObjectCreate;
            Player = new Player();
            Controls = new GameControl(Player);
            Player.Death += OnPlayerDeath;
            Player.SwitchedGamePause += OnGamePauseSwitchedByPlayer;
            Player.SwitchedGameMenu += OnGameMenuSwitchByPlayer;

            EnemyQueue = new EnemyQueue();
            CurrentEnemy = EnemyQueue.Clones(0);
            IsGameStarted = true;
        }

        private async void OnGameMenuSwitchByPlayer()
        {
            if (IsGameStarted)
            {
                if (Paused)
                {
                    TriggerGameMenuClose?.Invoke();
                    TriggerGameUnpause?.Invoke();
                    await UnPause();
                }
                else
                {
                    Pause();
                    TriggerGamePause?.Invoke();
                    TriggerGameMenuOpen?.Invoke();
                }
            }
        }

        private async void OnGamePauseSwitchedByPlayer()
        {
            if (IsGameStarted)
            {
                if (Paused)
                {
                    TriggerGameUnpause?.Invoke();
                    await UnPause();
                }
                else
                {
                    Pause();
                    TriggerGamePause?.Invoke();
                }
            }
        }

        public async Task GameLoop()
        {
            while (!Paused)
            {
                int delay = Math.Max(MinDelay, MaxDelay);
                await Task.Delay(delay);

                DrawObjects!();
                DrawLinesOfFire!();
                UpdateObjects();
                UpdateLabels!();
                CleanGameObjects();
            }
        }

        public void UpdateObjects()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObject obj = GameObjects[i];
                obj.TakeAction();
            }
        }

        public void CleanGameObjects()
        {
            GameObjects.RemoveAll(o => o.IsExpired);
        }

        public void SpawnCharacters()
        {
            foreach (var character in GetCharacters())
            {
                Spawn(character);
            }
        }

        private void Spawn(Character character)
        {
            TriggerSpawnModel?.Invoke(character);
            character.FireBullet += SpawnProjectileFiredBy;
            character.UseGrenade += SpawnProjectileFiredBy;
        }

        private void SpawnProjectileFiredBy(Character character, Projectile projectile)
        {
            if (projectile is Bullet newBullet && character.BulletsFired > 0 && character.BulletsFired % GameStatic.rand.Next(3, 6) == 0)
            {
                newBullet.SetToTracerRound();
            }

            TriggerSpawnBulletModel?.Invoke(projectile, character);
        }

        private void OnGameObjectCreate(GameObject gameObject)
        {
            gameObject.TriggerRemoveObject += RemoveGameObject;
            GameObjects.Add(gameObject);
            TriggerSpawn?.Invoke(gameObject);
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            TriggerRemoveModel?.Invoke(gameObject);
        }

        public void Pause()
        {
            Paused = true;
        }

        public async Task UnPause()
        {
            Paused = false;
            await GameLoop();
        }

        public void Reset()
        {
            GameObjects.Clear();
            GameRestarted?.Invoke();
        }

        private void OnPlayerDeath()
        {
            GameOver = true;
            Pause();
            IsGameStarted = false;
            TriggerPlayerDeath?.Invoke();
        }

        public List<Character> GetCharacters()
        {
            return GameObjects.OfType<Character>().ToList();
        }

        public List<Projectile> GetProjectiles()
        {
            return GameObjects.OfType<Projectile>().ToList();
        }
    }
}