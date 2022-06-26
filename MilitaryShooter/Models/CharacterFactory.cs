using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter.Models
{
    internal class CharacterFactory : ModelFactoria
    {
        private readonly Character _character;
        public CharacterFactory(Character character)
        {
            _character = character;
        }
        public override GameObjectModel FactoryMethod()
        {
            return new CharacterModel(_character);
        }
    }
}
