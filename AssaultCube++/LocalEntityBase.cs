using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AssaultCube__.EntityList;

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
        public int baseDll = 0x400000;

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

        public ScreenSize screensize { get { return getRect(); } }
        public vector3 localPos { get { return getPos(); } }
        public Matrix4x4 viewMatrix { get { return readMatrix(); } }
        public LocalEntityBase()
        {
            memory = new Memory();
            entityPointer = getEntity();        
        }
        
        public struct ScreenSize
        {
            public int width { get; set; }
            public int height { get; set; }
        }

        public ScreenSize getRect()
        {
            int width = Program.ReadInt(0x00510C94);
            int height = Program.ReadInt(0x00510C94 + 0x04);

            var s = new ScreenSize();
            s.width = width;
            s.height = height;

            return s;
        }

        public Matrix4x4 readMatrix()
        {
            return Program.readViewMatrix(LocalEntityBase.memory.viewMatrix);
        }

        public void updateEnt()
        {
            Health = Program.ReadInt(entityPointer + LocalEntityBase.memory.Health);
            x = Program.ReadFloat(entityPointer + LocalEntityBase.memory.x);
            y = Program.ReadFloat(entityPointer + LocalEntityBase.memory.y);
            z = Program.ReadFloat(entityPointer + LocalEntityBase.memory.z);
            aimx = Program.ReadFloat(entityPointer + LocalEntityBase.memory.aimx);
            aimy = Program.ReadFloat(entityPointer + LocalEntityBase.memory.aimy);
            Name = Program.ReadString(entityPointer + LocalEntityBase.memory.Name);
            teamid = Program.ReadInt(entityPointer + LocalEntityBase.memory.teamid);
            headx = Program.ReadFloat(entityPointer + LocalEntityBase.memory.headx);
            headz = Program.ReadFloat(entityPointer + LocalEntityBase.memory.headz);
            heady = Program.ReadFloat(entityPointer + LocalEntityBase.memory.heady);
            //Console.WriteLine(GetInfo());
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

        public static Vector2 CalcAngles(EntityList.Entity destEnt, LocalEntityBase lcEntity)
        {
            //yaw is the horizontal rotation, exemple the yaw of a plane is when it swings without tilting
            //calculating the yaw
            if (destEnt == null || lcEntity == null)
                return new Vector2(0, 0);
            float dx = destEnt.headx - lcEntity.headx;
            float dy = destEnt.heady - lcEntity.heady;
            double angleYaw = Math.Atan2(dy, dx) * 180 / Math.PI;

            ////calculating the pitch to the player:

            //calculate verticle angle between enemy and player (pitch)
            double distance = Math.Sqrt(dx * dx + dy * dy);
            float dz = destEnt.headz - lcEntity.headz;
            double anglePitch = Math.Atan2(dz, distance) * 180 / Math.PI;

            //
            float Yaw = (float)angleYaw + 90;
            float Pitch = (float)anglePitch;
            return new Vector2(Yaw, Pitch);
        }

        public void setAim(LocalEntityBase lcEntity, Vector2 point)
        {
            Console.WriteLine("Point: " + point.X.ToString() + " - " +  point.Y.ToString());
            Program.WriteFloat(entityPointer + memory.aimx, point.X);
            Program.WriteFloat(entityPointer + memory.aimy, point.Y);
        }

        public void setSilentAim(LocalEntityBase lcEntity, Vector2 point)
        {
            Vector2 oldPoints = new Vector2(lcEntity.aimx, lcEntity.aimy);
            setAim(lcEntity, point);
            Thread.Sleep(5);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(15);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            setAim(lcEntity, oldPoints);
        }

        public static float CalcDist(LocalEntityBase lcEntity, EntityList.Entity destEnt)
        {
            return (float)Math.Sqrt(Math.Pow(destEnt.x - lcEntity.localPos.x, 2) +
                Math.Pow(destEnt.y - lcEntity.localPos.y, 2));
        }


        public int getEntity()
        {
            int tempaddr = Program.ReadInt(baseAddress);
            return tempaddr;
        }

        public Vector2 WorldToScreen(Vector3 position, int width, int height)
        {



            //int Width = width;
            //int Height = height;

            //float screenW = (vMatrix.M14 * position.x) + (vMatrix.M24 * position.y) +
            //       (vMatrix.M34 * position.z) + vMatrix.M44;

            //if (screenW > 0.001f)
            //{
            //    float screenX = (vMatrix.M11 * position.x) + (vMatrix.M21 * position.x) +
            //        (vMatrix.M31 + position.z) + vMatrix.M41;

            //    float screenY = (vMatrix.M12 * position.x) + (vMatrix.M22 * position.x) +
            //        (vMatrix.M32 + position.z) + vMatrix.M42;

            //    float camX = width / 2f;
            //    float camY = height / 2f;

            //    float X = camX + (camX * screenX / screenW);
            //    float Y = camY - (camY * screenY / screenW);

            //    return new Point((int)X, (int)Y);
            //}
            //else
            //{
            //    return new Point((int)-99, (int)-99);
            //}


            var m = ReadMatrix(0x501AE8);
            Point screenPos;
            float screenX = (m.m11 * position.X) + (m.m21 * position.Y) + (m.m31 * position.Z) + m.m41;
            float screenY = (m.m12 * position.X) + (m.m22 * position.Y) + (m.m32 * position.Z) + m.m42;
            float screenZ = (m.m13 * position.X) + (m.m23 * position.Y) + (m.m33 * position.Z) + m.m43;
            float w = position.X * m.m14 + position.Y * m.m24 + position.Z * m.m34 + m.m44;


            //camera position (eye level/middle of screen)
            float camX = width / 2f;
            float camY = height / 2f;
            float camZ = screenZ / w;

            float x = camX + (camX * screenX / screenZ);
            float y = camY - (camY * screenY / screenZ);

            Vector2 v1 = new Vector2(camX, camY);

            Vector2 v2;
            bool s = m.WorldToScreen(position, width, height, out v1);

            return v1;

            //convert to homogeneous position
            //float x = camX + (camX * screenX / screenW);
            //float y = camY - (camY * screenY / screenW);

            //check if object is behind camera / off screen (not visible)
            //w = z where z is relative to the camera 

        }


        public static Matrix ReadMatrix(int baseAddress)
        {
            //float matrix[16]; 16-value array laid out contiguously in memory       
            byte[] buffer = new byte[16 * 4];

            //read memory into buffer
            int bytesRead;
            Program.ReadProcessMemory((int)Program.pHandleOverlay, baseAddress, buffer, (int)buffer.Length, out bytesRead);

            //convert bytes to floats
            Matrix mat = new Matrix();
            mat.m11 = BitConverter.ToSingle(buffer, (0 * 4));
            mat.m12 = BitConverter.ToSingle(buffer, (1 * 4));
            mat.m13 = BitConverter.ToSingle(buffer, (2 * 4));
            mat.m14 = BitConverter.ToSingle(buffer, (3 * 4));

            mat.m21 = BitConverter.ToSingle(buffer, (4 * 4));
            mat.m22 = BitConverter.ToSingle(buffer, (5 * 4));
            mat.m23 = BitConverter.ToSingle(buffer, (6 * 4));
            mat.m24 = BitConverter.ToSingle(buffer, (7 * 4));

            mat.m31 = BitConverter.ToSingle(buffer, (8 * 4));
            mat.m32 = BitConverter.ToSingle(buffer, (9 * 4));
            mat.m33 = BitConverter.ToSingle(buffer, (10 * 4));
            mat.m34 = BitConverter.ToSingle(buffer, (11 * 4));

            mat.m41 = BitConverter.ToSingle(buffer, (12 * 4));
            mat.m42 = BitConverter.ToSingle(buffer, (13 * 4));
            mat.m43 = BitConverter.ToSingle(buffer, (14 * 4));
            mat.m44 = BitConverter.ToSingle(buffer, (15 * 4));
            return mat;
        }

        public class Matrix
        {
            public float m11, m12, m13, m14; //00, 01, 02, 03
            public float m21, m22, m23, m24; //04, 05, 06, 07
            public float m31, m32, m33, m34; //08, 09, 10, 11
            public float m41, m42, m43, m44; //12, 13, 14, 15


            /// <summary>
            /// Project a 3D position in world to a 2D position on the screen.
            /// </summary>
            /// <param name="worldPos">object's 3D position in world</param>
            /// <param name="width">screen width</param>
            /// <param name="height">screen height</param>
            /// <param name="screenPos">object's 2D position on screen</param>
            /// <returns>true if object is visible, false otherwise</returns>
            public bool WorldToScreen(Vector3 worldPos, int width, int height, out Vector2 screenPos)
            {

                //multiply vector against matrix
                float screenX = (m11 * worldPos.X) + (m21 * worldPos.Y) + (m31 * worldPos.Z) + m41;
                float screenY = (m12 * worldPos.X) + (m22 * worldPos.Y) + (m32 * worldPos.Z) + m42;
                float screenW = (m14 * worldPos.X) + (m24 * worldPos.Y) + (m34 * worldPos.Z) + m44;

                //camera position (eye level/middle of screen)
                float camX = width / 2f;
                float camY = height / 2f;

                //convert to homogeneous position
                float x = camX + (camX * screenX / screenW);
                float y = camY - (camY * screenY / screenW);
                screenPos = new Vector2(x, y);

                //check if object is behind camera / off screen (not visible)
                //w = z where z is relative to the camera 
                return (screenW > 0.001f);
            }
        }

        public static bool IsValid(long address)
        {
            return (address >= 0x10000 && address < 0x000F000000000000);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
    }
}
