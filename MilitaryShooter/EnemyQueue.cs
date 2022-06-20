namespace MilitaryShooter
{
    internal class EnemyQueue
    {
        public Enemy NextEnemy { get; private set; }

        public EnemyQueue()
        {
            NextEnemy = new Enemy();
        }

        public Enemy GetAndUpdate()
        {
            Enemy enemy = NextEnemy;
            do
            {
                NextEnemy = new Enemy();
            }
            while (enemy.Guid == NextEnemy.Guid);

            return enemy;
        }

        public Enemy Clones(int clones)
        {
            do
            {
                NextEnemy = new Enemy();
                clones--;
            }
            while (clones>0);

            return NextEnemy;
        }
    }
}