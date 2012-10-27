using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace miRobotEditor.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class PropertyService
    {
        static string propertyFileName;
        static string propertyXmlRootNodeName;

        static string configDirectory;
        static string dataDirectory;

        static Properties properties;

        /// <summary>
        /// Returns true if initialized
        /// </summary>
        public static bool Initialized
        {
            get { return properties != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void InitializeServiceForUnitTests()
        {
            properties = null;
            InitializeService(null, null, null);
        }
        /// <summary>
        /// Initialize Service
        /// </summary>
        /// <param name="configDirectory"></param>
        /// <param name="dataDirectory"></param>
        /// <param name="propertiesName"></param>
        public static void InitializeService(string configDirectory, string dataDirectory, string propertiesName)
        {
            if (properties != null)
                throw new InvalidOperationException("Service is already initialized.");
            properties = new Properties();
            PropertyService.configDirectory = configDirectory;
            PropertyService.dataDirectory = dataDirectory;
            propertyXmlRootNodeName = propertiesName;
            propertyFileName = propertiesName + ".xml";
            properties.PropertyChanged += new PropertyChangedEventHandler(PropertiesPropertyChanged);
        }

        /// <summary>
        /// Returns configuration directory
        /// </summary>
        public static string ConfigDirectory
        {
            get
            {
                return configDirectory;
            }
        }

        /// <summary>
        /// Returns data directory
        /// </summary>
        public static string DataDirectory
        {
            get
            {
                return dataDirectory;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string Get(string property)
        {
            return properties[property];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(string property, T defaultValue)
        {
            return properties.Get(property, defaultValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void Set<T>(string property, T value)
        {
            properties.Set(property, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Load()
        {
            if (properties == null)
                throw new InvalidOperationException("Service is not initialized.");
            if (string.IsNullOrEmpty(configDirectory) || string.IsNullOrEmpty(propertyXmlRootNodeName))
                throw new InvalidOperationException("No file name was specified on service creation");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            if (!LoadPropertiesFromStream(Path.Combine(configDirectory, propertyFileName)))
            {
                LoadPropertiesFromStream(Path.Combine(DataDirectory, "options", propertyFileName));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool LoadPropertiesFromStream(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            try
            {
                using (LockPropertyFile())
                {
                    using (var reader = new XmlTextReader(fileName))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if (reader.LocalName == propertyXmlRootNodeName)
                                {
                                    properties.ReadProperties(reader, propertyXmlRootNodeName);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void Save()
        {
            if (string.IsNullOrEmpty(configDirectory) || string.IsNullOrEmpty(propertyXmlRootNodeName))
                throw new InvalidOperationException("No file name was specified on service creation");
            using (var ms = new MemoryStream())
            {
                var writer = new XmlTextWriter(ms, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement(propertyXmlRootNodeName);
                properties.WriteProperties(writer);
                writer.WriteEndElement();
                writer.Flush();

                ms.Position = 0;
                string fileName = Path.Combine(configDirectory, propertyFileName);
                using (LockPropertyFile())
                {
                    using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }

        /// <summary>
        /// Acquires an exclusive lock on the properties file so that it can be opened safely.
        /// </summary>
        public static IDisposable LockPropertyFile()
        {
            var mutex = new Mutex(false, "PropertyServiceSave-30F32619-F92D-4BC0-BF49-AA18BF4AC313");
            mutex.WaitOne();
            return new CallbackOnDispose(
                delegate
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                });
        }

        static void PropertiesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// Description of PropertyGroup.
    /// </summary>
    [Localizable(false)]
    public class Properties
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Version CurrentVersion = new Version(1, 0, 0, 0);

        /// <summary> Needed for support of late deserialization </summary>
        class SerializedValue
        {
            string content;

            public string Content
            {
                get { return content; }
            }

            public T Deserialize<T>()
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(content));
            }

            public SerializedValue(string content)
            {
                this.content = content;
            }
        }

        Dictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public string this[string property]
        {
            get
            {
                return Convert.ToString(Get(property), CultureInfo.InvariantCulture);
            }
            set
            {
                Set(property, value);
            }
        }

        public string[] Elements
        {
            get
            {
                lock (properties)
                {
                    List<string> ret = new List<string>();
                    foreach (KeyValuePair<string, object> property in properties)
                        ret.Add(property.Key);
                    return ret.ToArray();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object Get(string property)
        {
            lock (properties)
            {
                object val;
                properties.TryGetValue(property, out val);
                return val;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void Set<T>([Localizable(false)] string property, T value)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (value == null)
                throw new ArgumentNullException("value");
            T oldValue = default(T);
            lock (properties)
            {
                if (!properties.ContainsKey(property))
                {
                    properties.Add(property, value);
                }
                else
                {
                    oldValue = Get<T>(property, value);
                    properties[property] = value;
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
        }

        public bool Contains(string property)
        {
            lock (properties)
            {
                return properties.ContainsKey(property);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                lock (properties)
                {
                    return properties.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool Remove(string property)
        {
            lock (properties)
            {
                return properties.Remove(property);
            }
        }

        public override string ToString()
        {
            lock (properties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[Properties:{");
                foreach (KeyValuePair<string, object> entry in properties)
                {
                    sb.Append(entry.Key);
                    sb.Append("=");
                    sb.Append(entry.Value);
                    sb.Append(",");
                }
                sb.Append("}]");
                return sb.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Properties ReadFromAttributes(XmlReader reader)
        {
            Properties properties = new Properties();
            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    // some values are frequently repeated (e.g. type="MenuItem"),
                    // so we also use the NameTable for attribute values
                    // (XmlReader itself only uses it for attribute names)
                    string val = reader.NameTable.Add(reader.Value);
                    properties[reader.Name] = val;
                }
                reader.MoveToElement(); //Moves the reader back to the element node.
            }
            return properties;
        }

        internal void ReadProperties(XmlReader reader, string endElement)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                        {
                            return;
                        }
                        break;
                    case XmlNodeType.Element:
                        string propertyName = reader.LocalName;
                        if (propertyName == "Properties")
                        {
                            propertyName = reader.GetAttribute(0);
                            Properties p = new Properties();
                            p.ReadProperties(reader, "Properties");
                            properties[propertyName] = p;
                        }
                        else if (propertyName == "Array")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = ReadArray(reader);
                        }
                        else if (propertyName == "SerializedValue")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = new SerializedValue(reader.ReadInnerXml());
                        }
                        else
                        {
                            properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
                        }
                        break;
                }
            }
        }

        ArrayList ReadArray(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return new ArrayList(0);
            ArrayList l = new ArrayList();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Array")
                        {
                            return l;
                        }
                        break;
                    case XmlNodeType.Element:
                        l.Add(reader.HasAttributes ? reader.GetAttribute(0) : null);
                        break;
                }
            }
            return l;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteProperties(XmlWriter writer)
        {
            lock (properties)
            {
                var sortedProperties = new List<KeyValuePair<string, object>>(properties);
                sortedProperties.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Key, b.Key));
                foreach (KeyValuePair<string, object> entry in sortedProperties)
                {
                    object val = entry.Value;
                    if (val is Properties)
                    {
                        writer.WriteStartElement("Properties");
                        writer.WriteAttributeString("name", entry.Key);
                        ((Properties)val).WriteProperties(writer);
                        writer.WriteEndElement();
                    }
                    else if (val is Array || val is ArrayList)
                    {
                        writer.WriteStartElement("Array");
                        writer.WriteAttributeString("name", entry.Key);
                        foreach (object o in (IEnumerable)val)
                        {
                            writer.WriteStartElement("Element");
                            WriteValue(writer, o);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    else if (TypeDescriptor.GetConverter(val).CanConvertFrom(typeof(string)))
                    {
                        writer.WriteStartElement(entry.Key);
                        WriteValue(writer, val);
                        writer.WriteEndElement();
                    }
                    else if (val is SerializedValue)
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteRaw(((SerializedValue)val).Content);
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        var serializer = new XmlSerializer(val.GetType());
                        serializer.Serialize(writer, val, null);
                        writer.WriteEndElement();
                    }
                }
            }
        }

        void WriteValue(XmlWriter writer, object val)
        {
            if (val != null)
            {
                if (val is string)
                {
                    writer.WriteAttributeString("value", val.ToString());
                }
                else
                {
                    TypeConverter c = TypeDescriptor.GetConverter(val.GetType());
                    writer.WriteAttributeString("value", c.ConvertToInvariantString(val));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Save(XmlWriter writer)
        {
            using (writer)
            {
                writer.WriteStartElement("Properties");
                WriteProperties(writer);
                writer.WriteEndElement();
            }
        }

        //		public void BinarySerialize(BinaryWriter writer)
        //		{
        //			writer.Write((byte)properties.Count);
        //			foreach (KeyValuePair<string, object> entry in properties) {
        //				writer.Write(AddInTree.GetNameOffset(entry.Key));
        //				writer.Write(AddInTree.GetNameOffset(entry.Value.ToString()));
        //			}
        //		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Properties Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            var reader = new XmlTextReader(fileName);
            return Load(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Properties Load(XmlReader reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.LocalName)
                        {
                            case "Properties":
                                Properties properties = new Properties();
                                properties.ReadProperties(reader, "Properties");
                                return properties;
                        }
                    }
                }
            }
            return null;
        }

        public T Get<T>(string property, T defaultValue)
        {
            lock (properties)
            {
                object o;
                if (!properties.TryGetValue(property, out o))
                {
                    properties.Add(property, defaultValue);
                    return defaultValue;
                }

                if (o is string && typeof(T) != typeof(string))
                {
                    TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                    try
                    {
                        o = c.ConvertFromInvariantString(o.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                else if (o is ArrayList && typeof(T).IsArray)
                {
                    var list = (ArrayList)o;
                    var elementType = typeof(T).GetElementType();
                    var arr = System.Array.CreateInstance(elementType, list.Count);
                    var c = TypeDescriptor.GetConverter(elementType);
                    try
                    {
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            if (list[i] != null)
                            {
                                arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
                            }
                        }
                        o = arr;
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                else if (!(o is string) && typeof(T) == typeof(string))
                {
                    var c = TypeDescriptor.GetConverter(typeof(T));
                    if (c.CanConvertTo(typeof(string)))
                    {
                        o = c.ConvertToInvariantString(o);
                    }
                    else
                    {
                        o = o.ToString();
                    }
                }
                else if (o is SerializedValue)
                {
                    try
                    {
                        o = ((SerializedValue)o).Deserialize<T>();
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        o = defaultValue;
                    }
                    properties[property] = o; // store for future look up
                }
                try
                {
                    return (T)o;
                }
                catch (NullReferenceException)
                {
                    // can happen when configuration is invalid -> o is null and a value type is expected
                    return defaultValue;
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }



    /// <summary>
    /// This interface flags an object beeing "mementocapable". This means that the
    /// state of the object could be saved to an <see cref="Properties"/> object
    /// and set from a object from the same class.
    /// This is used to save and restore the state of GUI objects.
    /// </summary>
    public interface IMementoCapable
    {
        /// <summary>
        /// Creates a new memento from the state.
        /// </summary>
        Properties CreateMemento();

        /// <summary>
        /// Sets the state to the given memento.
        /// </summary>
        void SetMemento(Properties memento);
    }
}
