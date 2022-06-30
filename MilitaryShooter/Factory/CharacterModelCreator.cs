using MilitaryShooter.Classes;
using MilitaryShooter.Models;

namespace MilitaryShooter.Factory
{
    internal class CharacterModelCreator : ModelCreator
    {
        public override GameModel FactoryMethod(GameObject gameObject)
        {
            return new CharacterModel(gameObject);
        }
    }
}