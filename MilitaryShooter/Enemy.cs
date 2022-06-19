using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace MilitaryShooter
{
    internal class Enemy : Character
{
        public Enemy()
        {
            Name = "Enemy";
            Speed = 3;
            Width = 32;
            Height = 32;
            PositionLT = (GameEngine.ResX / 3.0, GameEngine.ResY / 3.0);
        }
    }
}
