using System;
using System.Collections.Generic;

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

        void SwitchGameMenu();
        void SwitchGamePause();
        void SwitchLaserTargeting();
        void ThrowGrenade();
        void ShootROF();
        void SetWaypoint((double, double) p);
        void AimAt((double, double) p);
        void ClearWaypoints();
    }
}