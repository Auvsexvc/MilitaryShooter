using MilitaryShooter.Classes;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter.Models
{
    internal class ModelFactory
    {
        private readonly List<GameObjectModel> _gameObjectModels;

        public ModelFactory()
        {
            _gameObjectModels = (new());
        }

        public GameObjectModel ProduceModel(GameObject gameObject)
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

        public GameObjectModel ProduceModel(Projectile projectile, Character gameObject)
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

        public List<GameObjectModel> GetGameModels()
        {
            return _gameObjectModels;
        }

        public void DecommissionAll()
        {
            _gameObjectModels.Clear();
        }

        public void Decommission(GameObjectModel gameModel)
        {
            _gameObjectModels.Remove(gameModel);
        }
    }
}