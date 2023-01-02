using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static AssaultCube__.EntityList;

namespace AssaultCube__
{
    public class EntityList
    {
        private static int baseAddress = 0x50F4F8;
        private static int basePlayerAmmount = 0x50F500;
        public int playerAmmount;
        private static int adress;
        public List<Entity> entities;
        public LocalEntityBase lcPlayer;
        public EntityList(LocalEntityBase localEntity)
        {
            adress = Program.ReadInt(baseAddress);
            entities = new List<Entity>();
            lcPlayer = localEntity;
            UpdateEnteties();
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
                var e = new Entity(addr, lcPlayer);
                if (e.Health > 0 && e.Health < 101)
                {
                    if (entities.Count != 0)
                        if (e.magnitude < entities[0].magnitude)
                            entities.Insert(0, e);
                        else
                            entities.Add(e);
                    else
                        entities.Add(e);
                }
            }

        }
        public class Entity
        {
            public int Health;
            public float x;
            public float y;
            public float z;
            public float aimx;
            public float aimy;
            public float magnitude;
            public string Name;
            public int teamid;
            public float headx;
            public float headz;
            public float heady;

            int adressEntity;
            private LocalEntityBase localEntity;
            /// <summary>
            /// Creates a new entity
            /// </summary>
            /// <param name="adress">The derefrenced pointer to the address</param>
            public Entity(int adress, LocalEntityBase lclEntity)
            {
                adressEntity = adress;
                localEntity = lclEntity;
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
                Name = Program.ReadString(adressEntity + LocalEntityBase.memory.Name);
                teamid = Program.ReadInt(adressEntity + LocalEntityBase.memory.teamid);
                headx = Program.ReadFloat(adressEntity + LocalEntityBase.memory.headx);
                headz = Program.ReadFloat(adressEntity + LocalEntityBase.memory.headz);
                heady = Program.ReadFloat(adressEntity + LocalEntityBase.memory.heady);

                magnitude = getMagnitude(this, localEntity);
                //Console.WriteLine(GetInfo());
            }

            public string GetInfo()
            {
                return "Name:" + Name + " Adress: " + adressEntity.ToString() + "Health: " + Health + 
                    " Pos: " + " x: " + x + " y: " + y + " z: " + z
                    + " MAG: " + magnitude.ToString() + " TeamId: " + teamid.ToString();
            }

            public float getMagnitude(Entity entity, LocalEntityBase lcPLayer)
            {
                float dx = lcPLayer.x - entity.x;
                float dy = lcPLayer.y - entity.y;
                float dz = lcPLayer.z - entity.z;

                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
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

        public int Name { get; private set; }
        public int teamid { get; set; }

        public int headx { get; private set; }
        public int headz { get; private set; }
        public int heady { get; private set; }

        public int viewMatrix { get; private set; }

        public Memory()
        {
            Health = 0xF8;
            x = 0x34;            
            y = 0x38;
            z = 0x3c;
            aimx = 0x40;
            aimy = 0x44;
            Name = 0x225;
            teamid = 0x32c;
            headx = 0x4;
            heady = 0x8;
            headz = 0xc;
            viewMatrix = 0x00501AE8;
        }
    }
}
