using MilitaryShooter.Models;
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
        public Enemy? CurrentEnemy { get; }
        public EnemyQueue? EnemyQueue { get; }

        public bool GameOver { get; private set; }

        public bool IsGameStarted { get; private set; }
        public bool Paused { get; private set; }
        public Player? Player { get; set; }
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }

        public event Action? DrawLinesOfFire;

        public event Action? DrawObjects;

        public event Action? TriggerGameMenuClose;

        public event Action? TriggerGameMenuOpen;

        public event Action? TriggerGamePause;

        public event Action? TriggerGameUnpause;

        public event Action? TriggerPlayerDeath;

        public event Action<GameObject>? TriggerRemoveModel;

        public event Action<GameObject>? TriggerSpawn;

        public event Action<Projectile, Character>? TriggerSpawnBulletModel;

        public event Action<Character>? TriggerSpawnModel;

        public event Action? UpdateLabels;

        public GameEngine()
        {
            IsGameStarted = false;
        }

        public GameEngine(double resX, double resY)
        {
            ResX = resX;
            ResY = resY;
            GameObject.OnCreate += OnGameObjectCreate;

            Player = new Player();
            Controls = new GameControl(Player);
            Player.Death += OnPlayerDeath;
            Player.SwitchedGamePause += OnGamePauseSwitchedByPlayer;
            Player.SwitchedGameMenu += OnGameMenuSwitchByPlayer;
            Player.RestartedGame += Reset;

            EnemyQueue = new EnemyQueue();
            CurrentEnemy = EnemyQueue.Clones(0);
            IsGameStarted = true;
            Paused = false;
        }

        private void CleanGameObjects()
        {
            GetGameObjects().RemoveAll(o => o.IsExpired);
        }

        public async Task GameLoop()
        {
            while (!Paused && IsGameStarted)
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

        public List<Character> GetCharacters()
        {
            return GetGameObjects().OfType<Character>().ToList();
        }

        public List<GameObject> GetGameObjects()
        {
            return GameObject.GameObjects;
        }

        public void Reset()
        {
            GameObject.GameObjects.Clear();
            GameObjectModel.Models.Clear();
        }

        public void SpawnCharacters()
        {
            foreach (var character in GetCharacters())
            {
                Spawn(character);
            }
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

        private void OnGameObjectCreate(GameObject gameObject)
        {
            gameObject.TriggerRemoveObject += RemoveGameObject;
            TriggerSpawn?.Invoke(gameObject);
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

        private void OnPlayerDeath()
        {
            GameOver = true;
            Pause();
            IsGameStarted = false;
            TriggerPlayerDeath?.Invoke();
        }

        private void Pause()
        {
            Paused = true;
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            TriggerRemoveModel?.Invoke(gameObject);
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

        private async Task UnPause()
        {
            Paused = false;
            await GameLoop();
        }

        private void UpdateObjects()
        {
            for (int i = 0; i < GetGameObjects().Count; i++)
            {
                GameObject obj = GetGameObjects()[i];
                obj.Update();
            }
        }
    }
}