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
            if (gameObject is Character character)
            {
                GameObjectModels.Add(new CharacterModel(character));
            }
            else
            {
                GameObjectModels.Add(new CharacterModel((Character)gameObject));
            }
            return GameObjectModels.Last();
        }

        public GameObjectModel ProduceModel(Projectile projectile, Character gameObject)
        {
            if (projectile is Bullet bullet)
            {
                GameObjectModels.Add(new BulletModel(bullet, gameObject));
            }
            else
            {
                GameObjectModels.Add(new GrenadeModel((Grenade)projectile, gameObject));
            }
            return GameObjectModels.Last();
        }
    }
}