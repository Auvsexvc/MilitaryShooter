using MilitaryShooter.Classes;
using System.Collections.Generic;
using System.Linq;

namespace MilitaryShooter.Models
{
    internal class ModelFactory
    {
        public List<GameObjectModel> GameObjectModels { get; set; }

        public ModelFactory()
        {
            GameObjectModels = new();
        }

        public GameObjectModel ProduceModel(GameObject gameObject)
        {
            switch (gameObject)
            {
                case Character character:
                    GameObjectModels.Add(new CharacterModel(character));
                    break;
                case Firearm firearm:
                    GameObjectModels.Add(new FirearmModel(firearm));
                    break;
                case Upgrade upgrade:
                    GameObjectModels.Add(new UpgradeModel(upgrade));
                    break;
                default:
                    GameObjectModels.Add(new CharacterModel((Character)gameObject));
                    break;
            }
            return GameObjectModels.Last();
        }

        public GameObjectModel ProduceModel(Projectile projectile, Character gameObject)
        {
            switch (projectile)
            {
                case Bullet bullet:
                    GameObjectModels.Add(new BulletModel(bullet, gameObject));
                    break;
                default:
                    GameObjectModels.Add(new GrenadeModel((Grenade)projectile, gameObject));
                    break;
            }
            return GameObjectModels.Last();
        }
    }
}