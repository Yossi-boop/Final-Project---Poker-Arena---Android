using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public static class Deck
    {
        public static Card[] deck = new Card[52];

        static Deck()
        {
            FillDeck();
        }
        public static void FillDeck()
        {
            int index = 0;
            foreach (string suit in Card.SuitsArray)
            {
                for (int value = 2; value <= 14; value++)
                {
                    Card card = new Card(value, suit);
                    deck[index] = card;
                    index++;
                }
            }
        }
    }
}
