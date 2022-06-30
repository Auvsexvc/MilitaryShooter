using MilitaryShooter.Classes;
using MilitaryShooter.Factories;
using MilitaryShooter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilitaryShooter
{
    internal class GameEngine
    {
        private const int MaxDelay = 16;
        private const int MinDelay = 16;

        private readonly ModelFactory _modelFactory;
        private readonly ObjectFactory _objectFactory;

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

        public event Action? CloseGameMenu;

        public event Action<GameModel>? MakeCharacterModel;

        public event Action<GameModel>? MakeProjectileModel;

        public event Action? OpenGameMenu;

        public event Action? PauseGame;

        public event Action? PlayerDeath;

        public event Action<GameModel>? RemoveModel;

        public event Action? UnpauseGame;

        public event Action? UpdateLabels;

        public event Action? UpdateLinesOfFire;

        public event Action? UpdateModels;

        public static double ResX { get; private set; }
        public static double ResY { get; private set; }
        public GameControl? Controls { get; }
        public Enemy? CurrentEnemy { get; }
        public EnemyQueue? EnemyQueue { get; }

        public bool GameOver { get; private set; }

        public bool IsGameStarted { get; private set; }
        public bool Paused { get; private set; }
        public Player? Player { get; set; }
        public async Task GameLoop()
        {
            while (!Paused && IsGameStarted)
            {
                int delay = Math.Max(MinDelay, MaxDelay);
                await Task.Delay(delay);

                Update();
            }
        }

        public List<GameModel> GetGameModels()
        {
            return _modelFactory.GetGameModels();
        }

        public List<GameObject> GetGameObjects()
        {
            return _objectFactory.GetGameObjects();
        }

        public void OnGameRestartedByPlayer()
        {
            _objectFactory.DecommissionAll();
            _modelFactory.DecommissionAll();
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
            _objectFactory.DecommissionExpired();
            _modelFactory.DecommissionExpired();
        }

        private List<Character> GetCharacters()
        {
            return _objectFactory.GetCharacters();
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
            _objectFactory.Decommission(gameObject);
            GameModel? modelToRemove = _modelFactory.FindGameModelBy(gameObject);
            if (modelToRemove != null)
            {
                RemoveModel?.Invoke(modelToRemove);
                _modelFactory.Decommission(modelToRemove);
            }
        }

        private void Spawn(Character character)
        {
            MakeCharacterModel?.Invoke(_modelFactory.MakeModel(character));
            character.TriggerRemoveObject += RemoveGameObject;
            character.FireBullet += SpawnProjectileFiredBy;
            character.UseGrenade += SpawnProjectileFiredBy;
        }

        private void SpawnProjectileFiredBy(Character character, Projectile projectile)
        {
            projectile.TriggerRemoveObject += RemoveGameObject;
            MakeProjectileModel?.Invoke(_modelFactory.MakeModel(projectile, character));
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
            for (int i = 0; i < _objectFactory.GetGameObjects().Count; i++)
            {
                GameObject obj = _objectFactory.GetGameObjects()[i];
                obj.Update();
            }
        }
    }
}