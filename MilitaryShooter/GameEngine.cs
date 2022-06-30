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

        private readonly ModelFactory _modelFactory;
        private readonly ObjectFactory _objectFactory;

        public GameControl? Controls { get; }
        public Enemy? CurrentEnemy { get; }
        public EnemyQueue? EnemyQueue { get; }

        public bool GameOver { get; private set; }

        public bool IsGameStarted { get; private set; }
        public bool Paused { get; private set; }
        public Player? Player { get; set; }
        public static double ResX { get; private set; }
        public static double ResY { get; private set; }

        public event Action? UpdateLinesOfFire;

        public event Action? UpdateModels;

        public event Action? CloseGameMenu;

        public event Action? OpenGameMenu;

        public event Action? PauseGame;

        public event Action? UnpauseGame;

        public event Action? PlayerDeath;

        public event Action<GameObjectModel>? RemoveModel;

        public event Action<GameObjectModel>? MakeProjectileModel;

        public event Action<GameObjectModel>? MakeCharacterModel;

        public event Action? UpdateLabels;

        public GameEngine()
        {
            IsGameStarted = false;
            _modelFactory = new();
            _objectFactory = new();
        }

        public GameEngine(double resX, double resY)
        {
            ResX = resX;
            ResY = resY;

            _modelFactory = new();
            _objectFactory = new();

            Player = _objectFactory.Make(new Player());
            Player.Death += OnPlayerDeath;
            Player.SwitchedGamePause += OnGamePauseSwitchedByPlayer;
            Player.SwitchedGameMenu += OnGameMenuSwitchByPlayer;
            Player.RestartedGame += OnGameRestartedByPlayer;

            Controls = new GameControl(Player);

            EnemyQueue = new EnemyQueue(_objectFactory);
            CurrentEnemy = EnemyQueue.Clones(0);
            IsGameStarted = true;
            Paused = false;
        }

        public async Task GameLoop()
        {
            while (!Paused && IsGameStarted)
            {
                int delay = Math.Max(MinDelay, MaxDelay);
                await Task.Delay(delay);

                Update();
            }
        }

        public List<GameObjectModel> GetModels()
        {
            return _modelFactory.GameObjectModels;
        }

        public void OnGameRestartedByPlayer()
        {
            _objectFactory.GameObjects.Clear();
            _modelFactory.GameObjectModels.Clear();
        }

        public void SpawnCharacters()
        {
            foreach (var character in GetCharacters())
            {
                Spawn(character);
            }
        }

        private void CleanGameObjects()
        {
            GetGameObjects().RemoveAll(o => o.IsExpired);
        }

        private List<Character> GetCharacters()
        {
            return GetGameObjects().OfType<Character>().ToList();
        }

        private List<GameObject> GetGameObjects()
        {
            return _objectFactory.GameObjects;
        }

        private async void OnGameMenuSwitchByPlayer()
        {
            if (IsGameStarted)
            {
                if (Paused)
                {
                    CloseGameMenu?.Invoke();
                    UnpauseGame?.Invoke();
                    await UnPause();
                }
                else
                {
                    Pause();
                    PauseGame?.Invoke();
                    OpenGameMenu?.Invoke();
                }
            }
        }

        private async void OnGamePauseSwitchedByPlayer()
        {
            if (IsGameStarted)
            {
                if (Paused)
                {
                    UnpauseGame?.Invoke();
                    await UnPause();
                }
                else
                {
                    Pause();
                    PauseGame?.Invoke();
                }
            }
        }

        private void OnPlayerDeath()
        {
            GameOver = true;
            Pause();
            IsGameStarted = false;
            PlayerDeath?.Invoke();
        }

        private void Pause()
        {
            Paused = true;
        }

        private void RemoveGameObject(GameObject gameObject)
        {
            _objectFactory.GameObjects.Remove(gameObject);
            var modelToRemove = _modelFactory.GameObjectModels.Find(o => o.GetGameObject() == gameObject);
            if (modelToRemove != null)
            {
                RemoveModel?.Invoke(modelToRemove);
                _modelFactory.GameObjectModels.Remove(modelToRemove);
            }
        }

        private void Spawn(Character character)
        {
            MakeCharacterModel?.Invoke(_modelFactory.ProduceModel(character));
            character.TriggerRemoveObject += RemoveGameObject;
            character.FireBullet += SpawnProjectileFiredBy;
            character.UseGrenade += SpawnProjectileFiredBy;
        }

        private void SpawnProjectileFiredBy(Character character, Projectile projectile)
        {
            if (projectile is Bullet newBullet && character.BulletsFired > 0 && character.BulletsFired % GameStatic.rand.Next(3, 6) == 0)
            {
                newBullet.SetToTracerRound();
            }
            projectile.TriggerRemoveObject += RemoveGameObject;
            MakeProjectileModel?.Invoke(_modelFactory.ProduceModel(projectile, character));
        }

        private async Task UnPause()
        {
            Paused = false;
            await GameLoop();
        }

        private void Update()
        {
            UpdateGameObjects();
            UpdateModels?.Invoke();
            UpdateLinesOfFire?.Invoke();
            UpdateLabels?.Invoke();
            CleanGameObjects();
        }

        private void UpdateGameObjects()
        {
            for (int i = 0; i < GetGameObjects().Count; i++)
            {
                GameObject obj = GetGameObjects()[i];
                obj.Update();
            }
        }
    }
}