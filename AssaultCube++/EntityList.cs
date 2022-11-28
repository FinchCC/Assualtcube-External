using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCube__
{
    public class EntityList
    {
        private static int baseAddress = 0x50F4F8;
        private static int basePlayerAmmount = 0x50F500;
        public int playerAmmount;
        private static int adress;
        public List<Entity> entities;
        public EntityList()
        {
            adress = Program.ReadInt(baseAddress);
            entities = new List<Entity>();
        }

        public void PrintAllEnteties()
        {
            if (entities is null)
                Console.WriteLine("Enteties is empty");
            else
            {
                foreach (var entety in entities)
                {
                    Console.WriteLine(entety.GetInfo() + "\n");
                }
            }
        }

        public void UpdateEnteties()
        {
            entities.Clear();
            playerAmmount = Program.ReadInt(basePlayerAmmount);
            int failedAmmount = 0;
            for (int i = 1; i < playerAmmount; i++)
            {
                int addr = Program.ReadInt(adress + (0x4 * i));
                if (addr == 0)
                    break;
                entities.Add(new Entity(addr));
            }
            PrintAllEnteties();
        }
        public class Entity
        {
            public int Health;
            public float x;
            public float y;
            public float z;
            public float aimx;
            public float aimy;

            int adressEntity;
            /// <summary>
            /// Creates a new entity
            /// </summary>
            /// <param name="adress">The derefrenced pointer to the address</param>
            public Entity(int adress)
            {
                adressEntity = adress;
                updateEnt();
            }

            private void updateEnt()
            {
                Health = Program.ReadInt(adressEntity + LocalEntityBase.memory.Health);
                x = Program.ReadFloat(adressEntity + LocalEntityBase.memory.x);
                y = Program.ReadFloat(adressEntity + LocalEntityBase.memory.y);
                z = Program.ReadFloat(adressEntity + LocalEntityBase.memory.z);
                aimx = Program.ReadFloat(adressEntity + LocalEntityBase.memory.aimx);
                aimy = Program.ReadFloat(adressEntity + LocalEntityBase.memory.aimy);

                //Console.WriteLine(GetInfo());
            }

            public string GetInfo()
            {
                return "Adress: " + adressEntity.ToString() + "Health: " + Health + " Pos: " + " x: " + x + " y: " + y + " z: " + z;
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
            aimx = 0x40;
            aimy = 0x44;
        }
    }
}
