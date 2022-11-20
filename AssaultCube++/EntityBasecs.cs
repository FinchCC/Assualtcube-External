using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCube__
{    
    public class EntityBasecs
    { 
        static int baseAddress = 0x50F4F4;
        public int entityPointer;

        public int localHealth { get { return getLocalheath(); } set { setLocalhealth(value); } }
        public EntityBasecs()
        {
            entityPointer = getEntity();        
        }

        public int getLocalheath()
        {
            int health = Program.ReadInt(entityPointer + 0xF8);
            return health;
        }

        public void setLocalhealth(int ammount)
        {
            Program.WriteInt(entityPointer + 0xF8, ammount);

        }


        public int getEntity()
        {
            int tempaddr = Program.ReadInt(baseAddress);
            return tempaddr;
        }
    }
}
