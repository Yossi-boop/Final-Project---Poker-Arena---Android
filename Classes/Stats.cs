using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Classes
{
    public class Stats
    {
        [Required]
        [Key]
        public string UserEmail { get; set; }

        [Required]
        public int Money { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public int Xp { get; set; }

        [Required]
        public int NumberOfHandsPlay { get; set; }

        [Required]
        public int NumberOfHandsWon { get; set; }

        [Required]
        public float VictoryPercentage { get; set; }

        [Required]
        public float BiggestPot { get; set; }

        public int? Card1 { get; set; }

        public int? Card2 { get; set; }

        public int? Card3 { get; set; }

        public int? Card4 { get; set; }

        public int? Card5 { get; set; }

        public Stats()
        {
            
        }

        public Stats(string i_UserEmail)
        {
            UserEmail = i_UserEmail;
            Money = 5000;
            Level = 1;
            Xp = 0;
            NumberOfHandsPlay = 0;
            NumberOfHandsWon = 0;
            VictoryPercentage = 0;
            BiggestPot = 0;
        }

        public void ConvertHandToInts(PokerHand i_Hand)
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    int count = i_Hand.Hand[i].Value - 2;
                    string suit = i_Hand.Hand[i].Suit;
                    switch (suit)
                    {
                        case "Diamonds":
                            count += 13;
                            break;
                        case "Clubs":
                            count += 26;
                            break;
                        case "Spades":
                            count += 39;
                            break;
                    }
                    switch (i)
                    {
                        case 0:
                            Card1 = count;
                            break;
                        case 1:
                            Card2 = count;
                            break;
                        case 2:
                            Card3 = count;
                            break;
                        case 3:
                            Card4 = count;
                            break;
                        case 4:
                            Card5 = count;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("Stats.ConvertHandToInts/" + e.Message);
                
                throw e;
            }
        }
    }
}
