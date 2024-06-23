using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monoGameBlackjack
{
    internal struct Card
    {
        public int number;
        public string suite;
        public int position;

        public Card(int position,string suite,int number)
        {
            this.number = number;
            this.suite = suite;
            this.position = position;
        }
    }
}
