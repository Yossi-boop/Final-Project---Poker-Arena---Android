using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
    public class Round
    {
        public string Id { get; set; }

        public List<PokerPlayer> ActivePlayersIndex { get; set; }

        public int NumberOfPlayers { get; set; }

        public int SmallBlind { get; set; }

        public int Dealer { get; set; }

        public int TotalBets { get; set; }

        public RoundPart Part { get; set; }

        public PotsManager Pot { get; set; }

        public Pot MainPot { get; set; }

        public List<Pot> SidePots { get; set; }

        public List<Card> SharedCards  { get; set; }

        public List<List<Card>> UsersCards { get; set; }

        public List<PokerHand> UsersHands { get; set; }

        public List<int> WinnersIndex { get; set; }

        public Betting currentBettingRound { get; set; }

        public DateTime LastActionTime { get; set; }

        private readonly object makeAnActionLock = new object();

        public void CheckIfTimeout()
        {
            try
            {
                if (DateTime.Now.Subtract(LastActionTime).TotalSeconds >= 17)
                {
                    skipTurn();
                }
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("Round.CheckIfTimeout/" + e.Message);
                
                throw e;
            }
        }

        private void skipTurn()
        {
            try
            {
                if (currentBettingRound.BiggestMoneyInPot > currentBettingRound.CurrentPlayer.CurrentRoundBet)
                {
                    MakeAnAction(currentBettingRound.CurrentPlayer.Signature, eAction.Fold, 0);
                }
                else
                {
                    MakeAnAction(currentBettingRound.CurrentPlayer.Signature, eAction.Check, 0);
                }
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("Round.skipTurn/" + e.Message);
                
                throw e;
            }
        }
        public Round()
        {
            
        }

        public Round(List<PokerPlayer> i_ActivePlayersIndex, int i_NumberOfPlayers, int i_Dealer, int i_SmallBlind)
        {
            try
            {
                Id = "round_" + SystemTools.RandomString(10);
                ActivePlayersIndex = i_ActivePlayersIndex;
                checkIfReadyToPlay();
                NumberOfPlayers = i_NumberOfPlayers;
                transferMoneyFromStatsToTable();
                Dealer = i_Dealer;
                SmallBlind = i_SmallBlind;
                SharedCards = new List<Card>(5);
                UsersCards = new List<List<Card>>();
                UsersHands = null;
                randomCards(NumberOfPlayers * 2 + 5);
                Part = RoundPart.PreFlop;
                LastActionTime = DateTime.Now;
                startRound();
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.Round/" + e.Message);
                
                throw e;
            }
        }

        private void transferMoneyFromStatsToTable()
        {
            try
            {
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null)
                    {
                        player.Stat.Money -= player.Money;
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.transferMoneyFromStatsToTable/" + e.Message);
                
                throw e;
            }
        }

        private void checkIfReadyToPlay()
        {
            try
            {
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null)
                    {
                        player.NewHand();
                        if (player.ReadyToPlay)
                        {
                            if (player.Money <= 0)
                            {
                                player.ReadyToPlay = false;
                                player.ShouldPlayInRound = false;
                                player.InHand = false;
                            }
                            else
                            {
                                player.ShouldPlayInRound = true;
                                player.InHand = true;
                            }
                        }
                        else
                        {
                            player.InHand = false;
                            player.UpdateResult = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.checkIfReadyToPlay/" + e.Message);
                
                throw e;
            }
        }

        private void startRound()
        {
            try
            {
                playRoundPart();
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.startRound/" + e.Message);
                
                throw e;
            }
        }

        private void endRound()
        {
            try
            {
                UsersHands = new List<PokerHand>();
                getBestHandsForAllRemainingUsers();

                Pot = new PotsManager(ActivePlayersIndex);
                MainPot = Pot.MainPot;
                SidePots = Pot.SidePots;
                collectMoney(MainPot, SidePots);
                updateStatsForAllPlayers();

                Pot = null;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.endRound/" + e.Message);
                
                throw e;
            }
        }

        private void updateStatsForAllPlayers()
        {
            try
            {
                bool winner = false;
                for (int i = 0; i < NumberOfPlayers; i++)
                {
                    if (ActivePlayersIndex[i] != null)
                    {
                        winner = WinnersIndex.Contains(i);
                        ActivePlayersIndex[i].UpdateStats(winner, UsersHands[i]);
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.updateStatsForAllPlayers/" + e.Message);
                
                throw e;
            }
        }

        private void collectMoney(Pot i_MainPot, List<Pot> i_SidePots)
        {
            try
            {
                WinnersIndex = winnerIndexes(i_MainPot.ActivePlayerIndex);
                pushMoneyToPlayers(WinnersIndex.Count, WinnersIndex, i_MainPot.AmountOfMoney);
                foreach (var pot in i_SidePots)
                {
                    WinnersIndex = winnerIndexes(pot.ActivePlayerIndex);
                    pushMoneyToPlayers(WinnersIndex.Count, WinnersIndex, pot.AmountOfMoney);
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.collectMoney/" + e.Message);
                
                throw e;
            }
        }

        private void pushMoneyToPlayers(int i_WinnersCount, List<int> i_WinnersIndex, int i_MainPotAmountOfMoney)
        {
            try
            {
                foreach (var index in i_WinnersIndex)
                {
                    ActivePlayersIndex[index].Money += i_MainPotAmountOfMoney / i_WinnersCount;
                    ActivePlayersIndex[index].WinningAmount = i_MainPotAmountOfMoney / i_WinnersCount;
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.pushMoneyToPlayers/" + e.Message);
                
                throw e;
            }
        }

        private List<int> winnerIndexes(List<int> i_IndexOfPlayerInPot)
        {
            try
            {
                int currentBestHandIndex = i_IndexOfPlayerInPot[0];
                List<int> winnersIndex = new List<int>();

                foreach (var index in i_IndexOfPlayerInPot)
                {
                    if (UsersHands[currentBestHandIndex].CompareTo(UsersHands[index]) == 0)
                    {
                        winnersIndex.Add(index);
                    }
                    else if (UsersHands[currentBestHandIndex].CompareTo(UsersHands[index]) < 0)
                    {
                        currentBestHandIndex = index;
                        winnersIndex.Clear();
                        winnersIndex.Add(index);
                    }
                }

                return winnersIndex;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.winnerIndexes/" + e.Message);
                
                throw e;
            }
        }

        private void getBestHandsForAllRemainingUsers()
        {
            try
            {
                foreach (var userCards in UsersCards)
                {
                    UsersHands.Add(findBestHand(userCards));
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.getBestHandsForAllRemainingUsers/" + e.Message);
                
                throw e;
            }
        }
        
        private PokerHand findBestHand(List<Card> i_UserCards)
        {
            try
            {
                if (i_UserCards == null)
                {
                    return null;
                }

                PokerHand bestHand = new PokerHand(SharedCards);

                for (int i = 0; i < 5; i++)
                {
                    List<Card> cardsToCompare = copyHand(SharedCards);
                    cardsToCompare.Add(i_UserCards[0]);
                    cardsToCompare.Remove(SharedCards[i]);
                    PokerHand handToCompare = new PokerHand(copyHand(cardsToCompare));
                    if (handToCompare.CompareTo(bestHand) > 0)
                    {
                        bestHand = new PokerHand(copyHand(handToCompare.Hand));
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    List<Card> cardsToCompare = copyHand(SharedCards);
                    cardsToCompare.Add(i_UserCards[1]);
                    cardsToCompare.Remove(SharedCards[i]);
                    PokerHand handToCompare = new PokerHand(copyHand(cardsToCompare));
                    if (handToCompare.CompareTo(bestHand) > 0)
                    {
                        bestHand = new PokerHand(copyHand(handToCompare.Hand));
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    for (int j = i + 1; j < 5; j++)
                    {
                        List<Card> cardsToCompare = copyHand(SharedCards);
                        cardsToCompare.Add(i_UserCards[1]);
                        cardsToCompare.Add(i_UserCards[0]);
                        cardsToCompare.Remove(SharedCards[i]);
                        cardsToCompare.Remove(SharedCards[j]);
                        PokerHand handToCompare = new PokerHand(copyHand(cardsToCompare));
                        if (handToCompare.CompareTo(bestHand) > 0)
                        {
                            bestHand = new PokerHand(copyHand(handToCompare.Hand));
                        }
                    }
                }

                return bestHand;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.findBestHand/" + e.Message);
                
                throw e;
            }
        }

        private List<Card> copyHand(List<Card> i_SharedCards)
        {
            try
            {
                List<Card> cards = new List<Card>();
                foreach (var card in i_SharedCards)
                {
                    cards.Add(card);
                }

                return cards;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.copyHand/" + e.Message);
                
                throw e;
            }
        }

        private void randomCards(int i_Size)
        {
            try
            {
                List<int> randomResult = SystemTools.GetRandomCards(i_Size);
                int j = 0;
                for (int i = 0; i < 9; i++)
                {
                    List<Card> userCards = null;
                    if (ActivePlayersIndex[i] != null)
                    {
                        userCards = new List<Card>(2);
                        Card card = Deck.deck[randomResult[j++]];
                        userCards.Add(card);
                        card = Deck.deck[randomResult[j++]];
                        userCards.Add(card);
                    }
                    UsersCards.Add(userCards);
                }

                for (int i = i_Size - 5; i < i_Size; i++)
                {
                    Card card = Deck.deck[randomResult[i]];
                    SharedCards.Add(card);
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.randomCards/" + e.Message);
                
                throw e;
            }
        }

        public bool MakeAnAction(string i_Signature, eAction i_Action, int i_Raise)
        {
            try
            {
                lock(makeAnActionLock){
                if (currentBettingRound.MakeAnAction(i_Signature, i_Action, i_Raise))
                {
                    LastActionTime = DateTime.Now;
                    Console.WriteLine(LastActionTime);
                    currentBettingRound.CurrentPlayerIndex = nextPlayer(currentBettingRound.CurrentPlayerIndex);
                    if (isOnlyOne(i_Signature))
                    {
                        Part = RoundPart.Result;
                        endRound();
                    }
                    else if (!checkIfThereIsNeedForAction())
                    {
                        Part++;
                        playRoundPart();
                    }
                }
                else
                {
                    return false;
                }
                }


                return true;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.MakeAnAction/" + e.Message);
                
                throw e;
            }
        }

        private bool checkIfThereIsNeedForAction()
        {
            try
            {
                foreach (var player in ActivePlayersIndex)
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

                Logger.WriteToLogger("Round.checkIfThereIsNeedForAction/" + e.Message);
                
                throw e;
            }
        }

        private void playRoundPart()
        {
            try
            {
                resetAllPlayersRoundBet();
                int startIndex = Dealer;


                switch (Part)
                {
                    case RoundPart.PreFlop:
                        {
                            placeBlinds();
                            startIndex = nextPlayer(startIndex);
                            startIndex = nextPlayer(startIndex);
                            if (!isHeadsUp())
                            {
                                startIndex = nextPlayer(startIndex);
                            }

                            currentBettingRound = new Betting(ActivePlayersIndex, SmallBlind * 2, startIndex);
                            currentBettingRound.MinimumBet = 4 * SmallBlind;
                            break;
                        }
                    case RoundPart.Result:
                        {
                            endRound();
                            break;
                        }
                    default:
                        {
                            startIndex = nextPlayer(startIndex);
                            currentBettingRound = new Betting(ActivePlayersIndex, SmallBlind * 2, startIndex);
                            break;
                        }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.playRoundPart/" + e.Message);
                
                throw e;
            }
        }

        private void resetAllPlayersRoundBet()
        {
            try
            {
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null)
                    {
                        player.NewRound();
                    }
                }
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.resetAllPlayersRoundBet/" + e.Message);
                
                throw e;
            }
        }

        private bool isOnlyOne(string i_Signature)
        {
            try
            {
                int count = 0;
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null && player.InHand &&
                        (player.Money > 0 || player.Signature.Equals(i_Signature)))
                    {
                        count++;
                    }
                }

                if (count <= 1)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.isOnlyOne/" + e.Message);
                
                throw e;
            }
        }

        private bool isHeadsUp()
        {
            try
            {
                int count = 0;
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null && player.InHand)
                    {
                        count++;
                    }
                }

                if (count == 2)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.isHeadsUp/" + e.Message);
                
                throw e;
            }
        }

        private void placeBlinds()
        {
            try
            {
                int smallBlindIndex = nextPlayer(Dealer);
                if (isHeadsUp())
                {
                    smallBlindIndex = Dealer;
                }
                int bigBlindIndex = nextPlayer(smallBlindIndex);
                ActivePlayersIndex[smallBlindIndex].PlaceMoney(SmallBlind);
                ActivePlayersIndex[smallBlindIndex].LastAction = eAction.Small;
                ActivePlayersIndex[bigBlindIndex].PlaceMoney(SmallBlind * 2);
                ActivePlayersIndex[bigBlindIndex].LastAction = eAction.Big;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.placeBlinds/" + e.Message);
                
                throw e;
            }
        }

        private int nextPlayer(int i_LastIndex)
        {
            try
            {
                i_LastIndex++;
                for (int i = 0; i < NumberOfPlayers; i++)
                {
                    if (ActivePlayersIndex[(i_LastIndex + i) % NumberOfPlayers] != null &&
                        ActivePlayersIndex[(i_LastIndex + i) % NumberOfPlayers].InHand)
                    {
                        return (i_LastIndex + i) % NumberOfPlayers;
                    }
                }

                return -1;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.nextPlayer/" + e.Message);
                
                throw e;
            }
        }

        public bool FinishResult()
        {
            try
            {
                if (Part != RoundPart.Result)
                {
                    return false;
                }
                foreach (var player in ActivePlayersIndex)
                {

                    if (player != null && !player.UpdateResult && DateTime.Now.Subtract(player.LastActionTime).TotalSeconds <= 15)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.FinishResult/" + e.Message);
                
                throw e;
            }
        }


        public override string ToString()
        {
            try
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine("Round:" + Part);
                foreach (var card in SharedCards)
                {
                    result.Append(card + ",");
                }

                for (int i = 0; i < 9; i++)
                {
                    result.AppendLine("");
                    if (ActivePlayersIndex[i] != null)
                    {
                        result.AppendLine(ActivePlayersIndex[i].ToString());
                        result.AppendLine("Cards:" + UsersCards[i][0] + "," + UsersCards[i][1]);
                        if (UsersHands != null)
                        {
                            result.AppendLine("Cards:" + UsersHands[i].Hand[0] + "," + UsersHands[i].Hand[1] + ","
                                              + UsersHands[i].Hand[2] + "," + UsersHands[i].Hand[3] + ","
                                              + UsersHands[i].Hand[4] + ",");
                        }
                    }
                }


                return result.ToString();
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.ToString/" + e.Message);
                
                throw e;
            }
        }

        internal void GetOut(int i_PlayerPosition)
        {
            try
            {
                PokerPlayer player = ActivePlayersIndex[i_PlayerPosition];
                if (currentBettingRound != null && currentBettingRound.CurrentPlayerIndex == i_PlayerPosition)
                {
                    MakeAnAction(player.Signature, eAction.Fold, 0);
                }
                player.UpdateResult = true;
                player.InHand = false;
                player.ShouldPlayInRound = false;
                player.ReadyToPlay = false;

            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.GetOut/" + e.Message);
                
                throw e;
            }
        }   
        public void CalculateTotalBets()
        {
            try
            {
                int total = 0;
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null)
                    {
                        total += player.CurrentlyInPot;
                    }
                }

                TotalBets = total;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.CalculateTotalBets/" + e.Message);
                
                throw e;
            }
        }
        public PokerPlayer GetPlayer(string i_Email)
        {
            try
            {
                foreach (var player in ActivePlayersIndex)
                {
                    if (player != null)
                    {
                        if (player.Email.Equals(i_Email))
                        {
                            return player;
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("Round.GetPlayer/" + e.Message);
                
                throw e;
            }
        }
    }
}
