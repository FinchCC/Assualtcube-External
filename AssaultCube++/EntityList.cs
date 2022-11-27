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

    public static class Memory
    {
        static int Health { get; }
        static int x { get; }
        static int y { get; }
        static int z { get; }
        public Memory()
        {
            Heal
        }
    }
}
