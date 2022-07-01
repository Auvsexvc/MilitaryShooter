using MilitaryShooter.Classes;

namespace MilitaryShooter
{
    internal class EnemyQueue
    {
        private readonly GameObjectCreator _objectCreator;
        public Enemy NextEnemy { get; private set; }

        public EnemyQueue(GameObjectCreator factory)
        {
            _objectCreator = factory;
            NextEnemy = _objectCreator.Make(new Enemy());
        }

        public Enemy Clones(int clones)
        {
            while (clones > 0)
            {
                NextEnemy = _objectCreator.Make(new Enemy());
                clones--;
            }

            return NextEnemy;
        }

        public Enemy GetAndUpdate()
        {
            Enemy enemy = NextEnemy;
            do
            {
                NextEnemy = _objectCreator.Make(new Enemy());
            }
            while (enemy.Guid == NextEnemy.Guid);

            return enemy;
        }
    }
}