using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCube__
{    
    public struct vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public string getCords()
        {
            return "x: " + x.ToString() + ", y: " + y.ToString() + ", z: " + z.ToString();
        }
    }
    public class LocalEntityBase
    { 
        static int baseAddress = 0x50F4F4;
        public int entityPointer;
        public static Memory memory;

        public int localHealth { get { return getLocalheath(); } set { setLocalhealth(value); } }
        public vector3 localPos { get { return getPos(); } }
        public LocalEntityBase()
        {
            memory = new Memory();
            entityPointer = getEntity();        
        }

        public int getLocalheath()
        {
            int health = Program.ReadInt(entityPointer + 0xF8);
            return health;
        }

        public vector3 getPos()
        {
            vector3 pos = new vector3 { x = Program.ReadFloat(entityPointer + memory.x), 
                y = Program.ReadFloat(entityPointer + memory.y), z = Program.ReadFloat(entityPointer + memory.z) };
            return pos;
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
