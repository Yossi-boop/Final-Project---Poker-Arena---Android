using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
    public class Card: IComparable
    {
        public int Value;
        public static string[] SuitsArray = new string[] { "Hearts", "Diamonds", "Clubs", "Spades" };
        public string Suit;

        public Card()
        {
            
        }

        public Card(int value, string suit)
        {
            Value = value;
            Suit = suit;
        }

        public override string ToString()
        {
            try
            {
                string tempValue = "";
                string suitSentence = "";
                switch (Value)
                {
                    case 11:
                        tempValue = "Jack";
                        break;
                    case 12:
                        tempValue = "Queen";
                        break;
                    case 13:
                        tempValue = "King";
                        break;
                    case 14:
                        tempValue = "Ace";
                        break;
                    default:
                        tempValue = Value.ToString();
                        break;
                }
                switch (Suit)
                {
                    case "Hearts":
                        suitSentence = " of Hearts";
                        break;
                    case "Diamonds":
                        suitSentence = " of Diamonds";
                        break;
                    case "Clubs":
                        suitSentence = " of Clubs";
                        break;
                    case "Spades":
                        suitSentence = " of Spades";
                        break;
                }
                return tempValue + suitSentence;
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("Card.ToString/" + e.Message);
                
                throw e;
            }
        }

        public int CompareTo(object obj)
        {
            try { if (obj == null) return 1;

                if (obj is Card otherCard)
                    return otherCard.Value - Value;
                else
                    throw new ArgumentException("Object is not a Card");
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Card.CompareTo/" + e.Message);
                
                throw e;
            }
        }
    }



}
