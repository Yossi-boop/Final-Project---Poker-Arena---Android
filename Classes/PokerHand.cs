using System;
using System.Collections.Generic;
using System.Globalization;

namespace Classes
{
    public class PokerHand: IComparable<PokerHand>
    {
        public List<Card> Hand { get; set; }

        public Rank HandRank { get; set; }

        public PokerHand(List<Card> i_Hand)
        {
            try
            {
                if (i_Hand != null)
                {
                    Hand = i_Hand;
                    Hand.Sort();
                    HandRank = CalculateHandRank(Hand);
                }
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("PokerHand.PokerHand/" + e.Message);
                
                throw e;
            }
        }

        private Rank CalculateHandRank(List<Card> i_Hand)
        {
            try
            {
                int maxNumberOfSameCards = MaxNumberOfSameCards(i_Hand);

                switch (maxNumberOfSameCards)
                {
                    case 1:
                        if (CheckIfFlush(Hand))
                        {
                            if (CheckIfStraight(Hand))
                            {
                                if (i_Hand[0].Value == 14)
                                {
                                    return Rank.RoyalFlush;
                                }

                                return Rank.StraightFlush;

                            }

                            return Rank.Flush;

                        }
                        else
                        {
                            if (CheckIfStraight(Hand))
                            {
                                return Rank.Straight;
                            }

                            return Rank.HighCard;

                        }

                    case 2:
                        if (CheckIfTwoPairs(Hand))
                        {
                            return Rank.TwoPairs;
                        }
                        else
                        {
                            return Rank.Pair;
                        }

                    case 3:
                        if (CheckIfFullHouse(Hand))
                        {
                            return Rank.FullHouse;
                        }
                        else
                        {
                            return Rank.ThreeOfAKind;
                        }

                    case 4:
                        return Rank.FourOfAKind;

                }

                return Rank.HighCard;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CalculateHandRank/" + e.Message);
                
                throw e;
            }
        }

        private int MaxNumberOfSameCards(List<Card> i_Hand)
        {
            int sameValue = 1;
            int max = 1;
            int startIndex= 0;
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_Hand[i].CompareTo(i_Hand[i + 1]) == 0)
                    {
                        sameValue++;
                    }
                    else
                    {
                        if (max < sameValue)
                        {
                            max = sameValue;
                            startIndex = i - max + 1;
                        }

                        sameValue = 1;
                    }


                }

                if (max < sameValue)
                {
                    max = sameValue;
                    startIndex = 4 - max + 1;
                }

                if (max > 1)
                {
                    Hand = ChangeHandOrder(max, startIndex);
                }

                return max;
            }
            catch (Exception e)
            {
                
                    Logger.WriteToLogger("PokerHand.MaxNumberOfSameCards/" + e.Message);
                
                throw e;
            }
        }

        private List<Card> ChangeHandOrder(int i_Max, int i_StartIndex)
        {
            try
            {
                List<Card> newOrder = new List<Card>(5);
                for (int i = i_StartIndex; i < i_StartIndex + i_Max; i++)
                {
                    newOrder.Add(Hand[i]);
                }

                for (int i = 0; i < i_StartIndex; i++)
                {
                    newOrder.Add(Hand[i]);
                }

                for (int i = i_StartIndex + i_Max; i < 5; i++)
                {
                    newOrder.Add(Hand[i]);
                }

                return newOrder;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.ChangeHandOrder/" + e.Message);
                
                throw e;
            }
        }

        private bool CheckIfTwoPairs(List<Card> i_Hand)
        {
            try
            {
                bool res = false;
                for (int i = 2; i < 4; i++)
                {
                    if (i_Hand[i].CompareTo(i_Hand[i + 1]) == 0)
                    {
                        int startIndex = i;
                        res = true;
                        Hand = ChangeHandOrderForTwoPairs(startIndex);
                        break;
                    }
                }
                return res;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CheckIfTwoPairs/" + e.Message);
                
                throw e;
            }
        }

        private List<Card> ChangeHandOrderForTwoPairs(int i_StartIndex)
        {
            try
            {
                List<Card> newOrder = new List<Card>(5);
                if (Hand[0].CompareTo(Hand[i_StartIndex]) < 0)
                {
                    newOrder.Add(Hand[0]);
                    newOrder.Add(Hand[1]);
                    if (i_StartIndex == 2)
                    {
                        newOrder.Add(Hand[2]);
                        newOrder.Add(Hand[3]);
                        newOrder.Add(Hand[4]);
                    }
                    else
                    {
                        newOrder.Add(Hand[3]);
                        newOrder.Add(Hand[4]);
                        newOrder.Add(Hand[2]);
                    }
                }
                else
                {
                    if (i_StartIndex == 2)
                    {
                        newOrder.Add(Hand[2]);
                        newOrder.Add(Hand[3]);
                        newOrder.Add(Hand[0]);
                        newOrder.Add(Hand[1]);
                        newOrder.Add(Hand[4]);
                    }
                    else
                    {
                        newOrder.Add(Hand[3]);
                        newOrder.Add(Hand[4]);
                        newOrder.Add(Hand[0]);
                        newOrder.Add(Hand[1]);
                        newOrder.Add(Hand[2]);
                    }
                }

                return newOrder;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.ChangeHandOrderForTwoPairs/" + e.Message);
                
                throw e;
            }
        }

        private bool CheckIfFullHouse(List<Card> i_Hand)
        {
            try
            {
                return i_Hand[3].CompareTo(i_Hand[4]) == 0;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CheckIfFullHouse/" + e.Message);
                
                throw e;
            }
        }

        private bool CheckIfStraight(List<Card> i_Hand)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i_Hand[i].CompareTo(i_Hand[i + 1]) != -1)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CheckIfStraight/" + e.Message);
                
                throw e;
            }
        }

        private bool CheckIfFlush(List<Card> i_Hand)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (String.Compare(i_Hand[i].Suit, i_Hand[i + 1].Suit, StringComparison.Ordinal) != 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CheckIfFlush/" + e.Message);
                
                throw e;
            }
        }

        public int CompareTo(PokerHand other)
        {
            try
            {
                if (this.HandRank == other.HandRank)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (this.Hand[i].CompareTo(other.Hand[i]) != 0)
                        {
                            return Hand[i].Value - other.Hand[i].Value;
                        }
                    }

                    return 0;
                }
                return this.HandRank - other.HandRank;
            }
            catch (Exception e)
            {

                Logger.WriteToLogger("PokerHand.CompareTo/" + e.Message);
                
                throw e;
            }
        }
    }
}