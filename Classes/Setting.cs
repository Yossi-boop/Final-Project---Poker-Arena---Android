namespace Classes
{
    public class Setting
    {
        public int MinBalance { get; set; }

        public int MaxBalance { get; set; }

        public int BigBlind { get; set; }

        public int SmallBlind { get; set; }

        public int TimeForPlay { get; set; }

        public Setting(int i_MinBalance, int i_MaxBalance, int i_BigBlind, int i_SmallBlind, int i_TimeForPlay)
        {
            MinBalance = i_MinBalance;
            MaxBalance = i_MaxBalance;
            BigBlind = i_BigBlind;
            SmallBlind = i_SmallBlind;
            TimeForPlay = i_TimeForPlay;
        }
    }
}