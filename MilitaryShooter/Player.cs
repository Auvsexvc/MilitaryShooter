namespace MilitaryShooter
{
    internal class Player : Character
    {
        public Player()
        {
            Name = "PlayerOne";
            Speed = 2;
            Width = 64;
            Height = 64;
            PositionLT = (GameEngine.ResX / 2.0, GameEngine.ResY / 2.0);
        }
    }
}