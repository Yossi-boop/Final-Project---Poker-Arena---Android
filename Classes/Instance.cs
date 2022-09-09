using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public interface Instance: IComparable<Instance>
    {
        int CurrentXPos { get; set; }
        int CurrentYPos { get; set; }
    }   
}
