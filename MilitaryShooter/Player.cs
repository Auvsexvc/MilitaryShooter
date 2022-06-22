namespace MilitaryShooter
{
    internal class Player : Character
    {
        public override double Speed { get; protected set; }

        public Player()
        {
            Name = "PlayerOne";
            Speed = 4;
            PositionLT = (GameEngine.ResX / 2.0, GameEngine.ResY / 2.0);
            Health = 100;
        }
    }
}