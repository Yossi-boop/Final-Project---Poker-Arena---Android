using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Pot
    {
        public int AmountOfMoney { get; set; }

        public List<int> ActivePlayerIndex { get; set; }

        public Pot(int amountOfMoney, List<int> activePlayer)
        {
            this.AmountOfMoney = amountOfMoney;
            this.ActivePlayerIndex = activePlayer;
        }
    }
}
