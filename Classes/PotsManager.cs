using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class PotsManager
    {
        private IList<PokerPlayer> players;

        public PotsManager()
        {
            
        }

        public PotsManager(IList<PokerPlayer> players)
        {
            try
            {
                this.players = players;
                this.MainPot = createMainPot();
                this.SidePots = createSidePots();
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PotsManager.PotsManager/" + e.Message);
                
                throw e;
            }
        }

        public Pot MainPot { get; set; }

        private Pot createMainPot()
        {
            try
            {
                var levels = this.Levels();
                var upperLimit = levels.First();
                return this.Create(0, upperLimit);
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PotsManager.createMainPot/" + e.Message);
                
                throw e;
            }
        }

        private List<Pot> createSidePots()
        {
            try
            {
                var pots = new List<Pot>();
                var levels = this.Levels();

                if (levels.Count > 1)
                {
                    var list = levels.ToList();

                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        var pot = this.Create(list[i], list[i + 1]);

                        if (pot.AmountOfMoney != 0)
                        {
                            pots.Add(pot);
                        }
                    }
                }

                return pots;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PotsManager.createSidePots/" + e.Message);
                
                throw e;
            }
        }

        public List<Pot> SidePots { get; set; }

        private SortedSet<int> Levels()
        {
            try
            {
                if (this.players == null)
                {
                    return null;
                }
                var levels = new SortedSet<int> { int.MaxValue };

                foreach (var item in this.players)
                {
                    if (item != null && item.Money <= 0)
                    {
                        levels.Add(item.CurrentlyInPot);
                    }
                }

                return levels;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PotsManager.Levels/" + e.Message);
                
                throw e;
            }
        }

        private Pot Create(int lowerLimit, int upperLimit)
        {
            try
            {
                var amountOfMoney = 0;
                var activePlayerIndex = new List<int>();

                foreach (var item in this.players)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item.CurrentlyInPot > lowerLimit && item.CurrentlyInPot <= upperLimit)
                    {
                        amountOfMoney += item.CurrentlyInPot - lowerLimit;

                        if (item.InHand)
                        {
                            activePlayerIndex.Add(item.Position);
                        }
                    }
                    else if (item.CurrentlyInPot > upperLimit)
                    {
                        amountOfMoney += upperLimit - lowerLimit;

                        if (item.InHand)
                        {
                            activePlayerIndex.Add(item.Position);
                        }
                    }
                }

                return new Pot(amountOfMoney, activePlayerIndex);
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PotsManager.Create/" + e.Message);
                
                throw e;
            }
        }
    }
}
