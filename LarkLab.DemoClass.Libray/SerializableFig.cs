using LarkLab.DemoClass.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LarkLab.DemoClass.Libray.DemoFig
{
    public class SerializableFig : BaseFig, IDiscoverable<string>
    {
        public void ExecuteTest()
        {
            var animal = new Animal() { Name = "dog", Type = "tugou", Age = 13 };
            var contentBuilder = new StringBuilder();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(Animal));
            using (var writer = new StringWriter(contentBuilder))
            {
                serializer.Serialize(writer, animal, ns);
            }
            var xmlContent = contentBuilder.ToString();
            using (var reader = new StringReader(xmlContent))
            {
                var animalDe = serializer.Deserialize(reader) as Animal;
            }

            IFormatter formatter = new BinaryFormatter();
            byte[] objArray = new byte[1024];
            var stream = new MemoryStream(objArray);
            formatter.Serialize(stream, animal);
            byte[] objDeserializeArray = new byte[1024];
            objArray.CopyTo(objDeserializeArray, 0);
            stream = new MemoryStream(objDeserializeArray);
            var obj = formatter.Deserialize(stream);
            stream.Close();

        }

        public string Description
        {
            get
            {
                return "Demo for ISerializable";
            }
        }

        public string Code
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Execut()
        {
            return "public string Execut()";
        }
    }
    [Serializable]
    public class Animal : ISerializable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Age { get; set; }
        public Animal() { }
        protected Animal(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString($"{nameof(Animal)}{nameof(Name)}");
            Type = info.GetString($"{nameof(Animal)}{nameof(Type)}");
            Age = info.GetInt32($"{nameof(Animal)}{nameof(Age)}");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(Animal)}{nameof(Name)}", Name);
            info.AddValue($"{nameof(Animal)}{nameof(Type)}", Type);
            info.AddValue($"{nameof(Animal)}{nameof(Age)}", Age);
        }
    }
}
