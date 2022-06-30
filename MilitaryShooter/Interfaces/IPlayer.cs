using System;

namespace MilitaryShooter.Interfaces
{
    internal interface IPlayer
    {
        event Action? SwitchedGameMenu;

        event Action? SwitchedGamePause;

        bool MoveDown { get; set; }
        bool MoveLeft { get; set; }
        bool MoveRight { get; set; }
        bool MoveUp { get; set; }
        void AimAt((double, double) p);

        void ClearWaypoints();

        void ContinueGame();

        void RestartGame();

        void SetWaypoint((double, double) p);

        void ShootROF();

        void SwitchGameMenu();

        void SwitchGamePause();

        void SwitchLaserTargeting();

        void ThrowGrenade();
    }
}