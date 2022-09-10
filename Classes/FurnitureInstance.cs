using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class FurnitureInstance :Instance, IComparable<Instance>
    {
        public int Type { get; set; }

        public int CurrentXPos { get; set; }

        public int CurrentYPos { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public string Id { get; set; }

        public FurnitureInstance()
        {
            
        }

        public FurnitureInstance(int i_Type, int i_CurrentXPos, int i_CurrentYPos, int i_Length, int i_Width, string i_Id = null)
        {
            Type = i_Type;
            CurrentXPos = i_CurrentXPos;
            CurrentYPos = i_CurrentYPos;
            Length = i_Length;
            Width = i_Width;
            Id = i_Id;
        }

        public int CompareTo(Instance other)
        {
            if (CurrentYPos == other.CurrentYPos)
            {
                return CurrentXPos - other.CurrentXPos;
            }
            return CurrentYPos - other.CurrentYPos;
        }
    }
}
