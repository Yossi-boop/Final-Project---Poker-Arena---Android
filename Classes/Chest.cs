using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Chest : FurnitureInstance, IComparable<Instance>
    {
        public bool Collected = false;
        public int GoldAmount { get; set;}
        public Chest()
        {
        }

        public Chest(int i_GoldAmpunt, int XPos, int YPos) : base(8,XPos,YPos,64,83,null)
        {
            GoldAmount = i_GoldAmpunt;
        }

        
    }
}
