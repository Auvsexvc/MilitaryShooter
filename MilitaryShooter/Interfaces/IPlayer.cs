using System;

namespace MilitaryShooter.Interfaces
{
    internal interface IPlayer
    {
        bool MoveDown { get; set; }

        bool MoveLeft { get; set; }

        bool MoveRight { get; set; }

        bool MoveUp { get; set; }

        event Action? SwitchedGameMenu;

        event Action? SwitchedGamePause;

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