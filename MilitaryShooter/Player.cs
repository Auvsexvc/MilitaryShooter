namespace MilitaryShooter
{
    internal class Player : Character
    {
        public Player()
        {
            Name = "PlayerOne";
            Speed = 4;
            Width = 32;
            Height = 32;
            Position = (GameEngine.ResX / 2.0, GameEngine.ResY / 2.0);
        }
    }
}