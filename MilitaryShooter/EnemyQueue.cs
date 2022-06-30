namespace MilitaryShooter
{
    internal class EnemyQueue
    {
        private readonly ObjectFactory _objectFactory;
        public Enemy NextEnemy { get; private set; }

        public EnemyQueue(ObjectFactory factory)
        {
            _objectFactory = factory;
            NextEnemy = _objectFactory.Make(new Enemy());
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

        public Enemy Clones(int clones)
        {
            while (clones > 0)
            {
                NextEnemy = _objectFactory.Make(new Enemy());
                clones--;
            }

            return NextEnemy;
        }
    }
}