using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Betting
    {
        public List<PokerPlayer> ActivePlayers { get; set; }

        public eAction ActionType { get; set; }

        public Betting(List<PokerPlayer> i_ActivePlayers, int i_BigBlind, int i_CurrentPlayerIndex)
        {
            ActivePlayers = i_ActivePlayers;
            MinimumBet = i_BigBlind;
            RaiseJump = i_BigBlind;
            CurrentPlayerIndex = i_CurrentPlayerIndex;
        }

        public PokerPlayer CurrentPlayer {
            get
            {
                try {
                    return ActivePlayers[CurrentPlayerIndex];
                } catch(Exception e)
                {
                    Logger.WriteToLogger("Betting.CurrentPlayer/" + e.Message);
                    throw e;
                }
            }
        }

        public int BiggestMoneyInPot
        {
            get
            {
                int maxAmount = 0;

                foreach (var player in ActivePlayers)
                {
                    if (player != null)
                    {
                        if (player.CurrentRoundBet > maxAmount)
                        {
                            maxAmount = player.CurrentRoundBet;
                        }
                    }
                }

                return maxAmount;
            }
        }

        public int MinimumBet { get; set; }

        public int RaiseJump { get; set; }

        public int CurrentPlayerIndex { get; set; }

        internal bool MakeAnAction(string i_Signature, eAction i_Action, int i_RaiseAmount)
        {
            try
            {
                if (!(CurrentPlayer.Signature.Equals(i_Signature)))
                {
                    return false;
                }

                ActionType = i_Action;
                CurrentPlayer.LastAction = ActionType;

                switch (ActionType)
                {
                    case eAction.Fold:
                        {
                            CurrentPlayer.InHand = false;
                            CurrentPlayer.ShouldPlayInRound = false;
                            break;
                        }
                    case eAction.Call:
                        {
                            CurrentPlayer.PlaceMoney(BiggestMoneyInPot - CurrentPlayer.CurrentRoundBet);
                            break;
                        }
                    case eAction.Raise:
                        {
                            if (CurrentPlayer.Money < i_RaiseAmount)
                            {
                                i_RaiseAmount = CurrentPlayer.Money + CurrentPlayer.CurrentRoundBet;
                            }
                            if (MinimumBet > i_RaiseAmount)
                            {
                                i_RaiseAmount = MinimumBet;
                            }
                            updateMinimumBet(i_RaiseAmount);
                            CurrentPlayer.PlaceMoney(i_RaiseAmount - CurrentPlayer.CurrentRoundBet);
                            requestActionFromAllPlayers();
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }

                CurrentPlayer.ShouldPlayInRound = false;
                return true;
            }
            catch (Exception e)
            {
               
                    Logger.WriteToLogger("Betting.MakeAnAction/" + e.Message);
               
                throw e;
            }
        }

        private void updateMinimumBet(int i_RaiseAmount)
        {
            RaiseJump = i_RaiseAmount - BiggestMoneyInPot;
            MinimumBet = BiggestMoneyInPot + RaiseJump;
        }

        private void requestActionFromAllPlayers()
        {
            try
            {
                foreach (var player in ActivePlayers)
                {
                    if (player != null)
                    {
                        if (player.InHand && player.Money > 0)
                        {
                            player.ShouldPlayInRound = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Betting.requestActionFromAllPlayers/" + e.Message);
                
                throw e;
            }
        }

        private bool checkIfThereIsNeedForAction()
        {
            try
            {
                foreach (var player in ActivePlayers)
                {
                    if (player != null)
                    {
                        if (player.ShouldPlayInRound)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("Betting.checkIfThereIsNeedForAction/" + e.Message);
                
                throw e;
            }
        }

    }
}
