using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilitaryShooter.Models
{
    internal class BulletFactory : ModelFactoria
    {
        private readonly Bullet _bullet;
        private readonly GameObject _gameObject;
        public BulletFactory(Bullet bullet, GameObject gameObject)
        {
            _gameObject = gameObject;
            _bullet = bullet;
        }
        public override GameObjectModel FactoryMethod()
        {
            return new BulletModel(_bullet, _gameObject);
        }

    }
}
