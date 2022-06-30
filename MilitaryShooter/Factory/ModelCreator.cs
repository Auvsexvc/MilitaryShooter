using MilitaryShooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter.Factory
{
    internal abstract class ModelCreator
    {
        public abstract GameObjectModel FactoryMethod(GameObject gameObject);
    }
}
