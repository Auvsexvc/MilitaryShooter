using MilitaryShooter.Classes;
using MilitaryShooter.Models;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter
{
    internal class ModelFactory
    {
        private readonly List<GameModel> _gameObjectModels;

        public ModelFactory()
        {
            _gameObjectModels = new();
        }

        public void Decommission(GameModel gameModel)
        {
            _gameObjectModels.Remove(gameModel);
        }

        public void DecommissionAll()
        {
            _gameObjectModels.Clear();
        }

        public void DecommissionExpired()
        {
            _gameObjectModels.RemoveAll(o => o.GetGameObject().IsExpired);
        }

        public GameModel? FindGameModelBy(GameObject gameObject)
        {
            return _gameObjectModels.Find(o => o.GetGameObject() == gameObject);
        }

        public List<GameModel> GetGameModels()
        {
            return _gameObjectModels;
        }

        public GameModel MakeModel(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Character character:
                    _gameObjectModels.Add(new CharacterModel(character));
                    break;

                case Firearm firearm:
                    _gameObjectModels.Add(new FirearmModel(firearm));
                    break;

                case Upgrade upgrade:
                    _gameObjectModels.Add(new UpgradeModel(upgrade));
                    break;

                default:
                    break;
            }
            return _gameObjectModels.Last();
        }

        public GameModel MakeModel(Projectile projectile, Character gameObject)
        {
            switch (projectile)
            {
                case Bullet bullet:
                    _gameObjectModels.Add(new BulletModel(bullet, gameObject));
                    break;

                case Grenade grenade:
                    _gameObjectModels.Add(new GrenadeModel(grenade, gameObject));
                    break;

                default:
                    break;
            }
            return _gameObjectModels.Last();
        }
    }
}