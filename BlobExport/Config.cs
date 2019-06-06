using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Config
{
    public string IPAddress { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    private static String path = System.AppDomain.CurrentDomain.BaseDirectory + "config.xml";

    private static Config mInstance;

    private Config()
    {

    }

    public static Config GetInstance()
    {
        if (mInstance == null)
        {
            if (!File.Exists(path))
            {
                mInstance = new Config();
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    new XmlSerializer(typeof(Config)).Serialize(fs, mInstance);
                    fs.Close();
                }
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    mInstance = new XmlSerializer(typeof(Config)).Deserialize(fs) as Config;
                }
            }
        }
        return mInstance;
    }

    public void Save()
    {
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            new XmlSerializer(this.GetType()).Serialize(fs, mInstance);
            fs.Close();
        }
    }
}
