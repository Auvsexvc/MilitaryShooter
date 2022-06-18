using System;
using System.Collections.Generic;

namespace MilitaryShooter
{
    internal class GameEngine
    {

        public Player Player { get; }
        public List<GameObject> GameObjects { get; } = new List<GameObject>();

        public event Action<Bullet>? TriggerSpawnBulletModel;
        internal event Action<Character> TriggerMoveCharacterModel;


        public GameEngine()
        {
            Player = new Player();
            Player.TriggerSpawnBullet += SpawnBullet;
            Player.TriggerMove += MoveCharacter;
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
        public void MoveCharacter(Character character)
        {
            TriggerMoveCharacterModel?.Invoke(character);
        }

        public void RemoveGameObject(GameObject obj)
        {
            GameObjects.Remove(obj);
        }

    }
}