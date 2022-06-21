﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilitaryShooter
{
    internal static class GameControl
    {
        public static void KeyDown(MainWindow window, Player player, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    player.MoveLeft = true;
                    player.PointsToMoveTo.Clear();
                    break;

                case Key.D:
                    player.MoveRight = true;
                    player.PointsToMoveTo.Clear();
                    break;

                case Key.W:
                    player.MoveUp = true;
                    player.PointsToMoveTo.Clear();
                    break;

                case Key.S:
                    player.MoveDown = true;
                    player.PointsToMoveTo.Clear();
                    break;
                case Key.Escape:
                    _ = window.OpenGamePanel();
                    break;
            }
        }

        public static void KeyUp(Player player, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    player.MoveLeft = false;
                    break;

                case Key.D:
                    player.MoveRight = false;
                    break;

                case Key.W:
                    player.MoveUp = false;
                    break;

                case Key.S:
                    player.MoveDown = false;
                    break;
            }
        }
    }
}
