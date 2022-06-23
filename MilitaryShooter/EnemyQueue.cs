namespace MilitaryShooter
{
    internal class EnemyQueue
    {
        public Enemy NextEnemy { get; private set; }

        public EnemyQueue()
        {
            NextEnemy = new Enemy();
        }

        public EnemyQueue(Character character)
        {
            NextEnemy = new Enemy(character);
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
            while (clones > 0)
            {
                NextEnemy = new Enemy();
                clones--;
            }

            return NextEnemy;
        }
    }
}