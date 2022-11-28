using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCube__
{
    public class EntityList
    {
        public static int baseAddress = 0x50F4F8;
        private static int adress;
        public EntityList()
        {
            adress = Program.ReadInt(baseAddress);
        }
        public class Entity
        {
            int adressEntity;
            public Entity(int offset)
            {

            }
        }


    }

    public class Memory
    {
        public int Health { get; private set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }
        public int aimx { get; private set; }
        public int aimy { get; private set; }
        public Memory()
        {
            Health = 0xF8;
            x = 0x38;
            y = 0x3C;
            z = 0x34;

        }
    }
}
