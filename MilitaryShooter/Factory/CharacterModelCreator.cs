using MilitaryShooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter.Factory
{
    internal class CharacterModelCreator : ModelCreator
    {
        public override GameObjectModel FactoryMethod(GameObject gameObject)
        {
            return new CharacterModel(gameObject);
        }
    }
}
