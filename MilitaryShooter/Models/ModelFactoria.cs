using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter.Models
{
    abstract class ModelFactoria
    {
        public abstract GameObjectModel FactoryMethod();
    }
}
