using MilitaryShooter.Classes;
using MilitaryShooter.Models;

namespace MilitaryShooter.Factory
{
    internal abstract class ModelCreator
    {
        public abstract GameModel FactoryMethod(GameObject gameObject);
    }
}