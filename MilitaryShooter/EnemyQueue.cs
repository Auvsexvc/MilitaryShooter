using MilitaryShooter.Classes;
using MilitaryShooter.Factories;

namespace MilitaryShooter
{
    internal class EnemyQueue
    {
        private readonly ObjectFactory _objectFactory;
        public EnemyQueue(ObjectFactory factory)
        {
            _objectFactory = factory;
            NextEnemy = _objectFactory.Make(new Enemy());
        }

        public Enemy NextEnemy { get; private set; }
        public Enemy Clones(int clones)
        {
            while (clones > 0)
            {
                NextEnemy = _objectFactory.Make(new Enemy());
                clones--;
            }

            return NextEnemy;
        }

        public Enemy GetAndUpdate()
        {
            Enemy enemy = NextEnemy;
            do
            {
                NextEnemy = _objectFactory.Make(new Enemy());
            }
            while (enemy.Guid == NextEnemy.Guid);

            return enemy;
        }
    }
}