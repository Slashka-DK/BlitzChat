using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bliUtils
{
    public class XMLSerializer
    {

        public static void serialize(object ob, string file) {
            lock (ob)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                    Directory.CreateDirectory(fileInfo.Directory.FullName);

                if (!File.Exists(file))
                {
                    FileStream fs = File.Create(file);
                    fs.Close();
                }
                XmlSerializer serializer = new XmlSerializer(ob.GetType());
                TextWriter textWriter = new StreamWriter(@file);
                serializer.Serialize(textWriter, ob);
                textWriter.Close();
            }
        }

        public static object deserialize(object ob, string file) {
            if (File.Exists(file))
            {
                try
                {
                    XmlSerializer deserializer = new XmlSerializer(ob.GetType());
                    TextReader reader = new StreamReader(@file);
                    object obj = deserializer.Deserialize(reader);
                    //Address XmlData = (Address)obj;
                    reader.Close();
                    return obj;
                }
                catch (Exception e) { 
                    Debug.Print("Deserializing Exception: " +e.Message);
                    return null;
                }
            }else
                return null;
        }
    }
}
