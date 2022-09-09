using System;
using System.Collections.Generic;
using System.Linq;


namespace Classes
{
    public class CharacterInstance: Instance, IComparable<Instance>
    {
        public string Name { get; set; }

        public string CasinoId { get; set; }

        public string Email { get; set; }

        public int LastXPos { get; set; }

        public int LastYPos { get; set; }

        public int CurrentXPos { get; set; }

        public int CurrentYPos { get; set; }

        public int Direction { get; set; }

        public int Skin { get; set; }

        public string LastMessage { get; set; }

        public DateTime LastMessageTime { get; set; } = DateTime.Now;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        private readonly object MovementLock = new object();


        public CharacterInstance()
        {
            
        }

        public CharacterInstance(string i_Name, string i_CasinoId,  string i_Email, int i_CurrentXPos, int i_CurrentYPos, int i_Direction, int i_Skin)
        {
            Name = i_Name;
            CasinoId = i_CasinoId;
            Email = i_Email;
            LastXPos = i_CurrentXPos;
            LastYPos = i_CurrentYPos;
            CurrentXPos = i_CurrentXPos;
            CurrentYPos = i_CurrentYPos;
            Direction = i_Direction;
            Skin = i_Skin;
        }

        public bool UpdatePosition(int i_CurrentXPos, int i_CurrentYPos, int i_Direction, int i_Skin)
        {
           if (DateTime.Now.Subtract(LastMessageTime).TotalSeconds >= 3)
           {
                LastMessage = null;
           }
            lock(MovementLock)
            {
                if (ValidateNewLocation(i_CurrentXPos, i_CurrentYPos, this.CurrentXPos, this.CurrentYPos))
            {
                this.LastXPos = this.CurrentXPos;
                this.LastYPos = this.CurrentYPos;
                this.CurrentXPos = i_CurrentXPos;
                this.CurrentYPos = i_CurrentYPos;
                Direction = i_Direction;
                Skin = i_Skin;

                LastUpdate = DateTime.Now;
                return true;
            }
             else
            {
                return false;
            }
            }
        }

        public bool ValidateNewLocation(int i_CurrentXPos, int i_CurrentYPos, int i_LastXPos, int i_LastYPos)
        {
            return !(i_LastYPos < i_CurrentYPos - 100 || i_LastYPos > i_CurrentYPos + 100 || i_LastXPos < i_CurrentXPos - 100  || i_LastXPos > i_CurrentXPos + 100);
        }

        public void AddMessage(string body)
        {
            LastMessage = body;
            LastMessageTime = DateTime.Now;
        }

        public int CompareTo(Instance other)
        {
            if (CurrentYPos == other.CurrentYPos)
            {
                return CurrentXPos - other.CurrentXPos;
            }
            return CurrentYPos - other.CurrentYPos;
        }

        public override string ToString()
        {
            return $"({LastXPos}.{LastYPos}),({CurrentXPos}.{CurrentYPos}),{Direction},{Skin}\n";
        }
    }
}