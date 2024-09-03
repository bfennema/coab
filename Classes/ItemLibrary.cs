using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Classes
{
    public class ItemLibrary
    {

        static string libraryPath = Logging.Config.AppDataPath;
        static string libraryFile = Path.Combine(libraryPath, "ItemLibrary.xml");

        static List<Item> library = new List<Item>();
        public static void Add(Item item)
        {
            Item i = item.ShallowClone();
            i.readied = false;
            i.hidden_names_flag = 0;
            i.name = i.GenerateName(0);
            if (library.Contains(i) == false)
            {
                library.Add(i);
                Write();
            }
        }

        public static void Read()
        {
            if (System.IO.File.Exists(libraryFile))
            {
                FileStream fs = new FileStream(libraryFile, FileMode.Open);

                if (fs.Length == 0)
                {
                    library = new List<Item>();
                    return;
                }

                // Construct a XmlSerializer and use it to serialize the data to the stream.
                XmlSerializer formatter = new XmlSerializer(library.GetType());
                try
                {
                    library = (List<Item>)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    //Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        public static void Write()
        {
            Directory.CreateDirectory(libraryPath);
            FileStream fs = new FileStream(libraryFile, FileMode.Create);

            // Construct a XmlSerializer and use it to serialize the data to the stream.
            XmlSerializer formatter = new XmlSerializer(library.GetType());
            try
            {
                formatter.Serialize(fs, library);
            }
            catch (SerializationException e)
            {
                //Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
