namespace MilitaryShooter.Models
{
    internal class ModelFactory
    {
        public GameObjectModel GameObjectModel { get; }

        public ModelFactory(GameObject gameObject)
        {
            if (gameObject is Character character)
            {
                GameObjectModel = new CharacterModel(character);
            }
            else
            {
                GameObjectModel = new CharacterModel((Character)gameObject);
            }
        }

        public ModelFactory(Projectile projectile, GameObject gameObject)
        {
            if (projectile is Bullet bullet)
            {
                GameObjectModel = new BulletModel(bullet, gameObject);
            }
            else
            {
                GameObjectModel = new BulletModel((Bullet)projectile, gameObject);
            }
        }
    }
}