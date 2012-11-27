using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SecretSantaHelper
{
    public static class SantaSackSerializer
    {
        public static SantaSack Deserialize()
        {
            SantaSack santaSack;
            IFormatter formatter = new BinaryFormatter();
            var path = Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).FullName + @"\SantaSack.bin";

            if (File.Exists(path))
            {
                Stream stream = new FileStream(path,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read);
                santaSack = (SantaSack)formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                santaSack = new SantaSack();
            }

            return santaSack;
        }

        public static void Serialize(SantaSack santaSack)
        {
            IFormatter formatter = new BinaryFormatter();
            var path = Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).FullName + @"\SantaSack.bin";
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, santaSack);
            stream.Close();
        }
    }
}
