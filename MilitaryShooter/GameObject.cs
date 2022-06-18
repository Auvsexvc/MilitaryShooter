using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter
{
    internal abstract class GameObject
    {
        public Guid Guid { get; set; }

        protected GameObject()
        {
            Guid = Guid.NewGuid();
        }
    }
}
