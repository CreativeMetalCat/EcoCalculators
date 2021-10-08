using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalCalc
{
    //содержит информацию о самом в-ве
    public class Element
    {
        public string Code;//maybe make this a string

        public byte[] NameBytes;

        public string Name => Encoding.UTF8.GetString(NameBytes);

        public string name;

        //асбтрактное, более компактное имя(для тех случаев когда при счете выводится элемент который не совсем точен)
        public string SimpleName;

        //Норматив качества атмосферного воздузха в мг/м^3
        public float AirQualityStandart;

        public bool Solid = true;

        //Технологические процессы при которых выделяется
        string ReasonToEmit;

        public Element(string code, string name,byte[] nameBytes, float airQualityStandart, bool solid = true)
        {
            //Name = name;
            AirQualityStandart = airQualityStandart;
            Code = code;
            Solid = solid;
            NameBytes = nameBytes;
            this.name = name;
        }

        public Element(string code, string name, float airQualityStandart, bool solid = true)
        {
            //Name = name;
            AirQualityStandart = airQualityStandart;
            Code = code;
            Solid = solid;
            this.name = name;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    //Handbles loading and storing of data from element table
    public class ElementDataStorage
    {
        public Dictionary<string, Element> Elements = new Dictionary<string, Element>();

        public void LoadData()
        {
            //load from table
            //currently just being filled manually

           /*Elements.Add(2930, new Element(0123, "Пыль абразивная (Корунд белый, Монокорунд)", 0.04f, true));
            Elements.Add(0123, new Element(2930, "диЖелезо триоксид (Железа оксид) (в пересчете на железо)", 0.04f, true));*/

            if (System.IO.File.Exists("./tables/elements.csv"))
            {
                //maybe replcae with line by line
                byte[] file = System.IO.File.ReadAllBytes("./tables/elements.csv");
                var lines = System.IO.File.ReadAllText("./tables/elements.csv",System.Text.Encoding.UTF8);
                List<byte> bytes = new List<byte>(file);
                //starting position
                int i = 0;
                void fillArray(out List<byte> array,bool removeSpaces = true)
                {
                    array = new List<byte>();
                   
                }
                while (i < bytes.Count)
                {
                    List<Byte> name = new List<Byte>();
                    List<Byte> code = new List<Byte>();
                    List<Byte> solid = new List<Byte>();
                    List<Byte> value = new List<Byte>();
                    //id
                    while (bytes[i] != 0x3b)
                    {
                        i++;
                    }
                    i++;
                    while (bytes[i] != 0x3b)
                    {
                        
                        if (bytes[i] != 0x20)
                            code.Add(bytes[i]);
                        i++;
                    }
                    i++;
                    while (bytes[i] != 0x3b)
                    {
                        name.Add(bytes[i]);
                        i++;
                    }
                    i++;

                    while (bytes[i] != 0x3b)
                    {
                        if (bytes[i] != 0x20)
                            value.Add(bytes[i]);
                        i++;
                    }
                    i++;
                    //solid
                    while (bytes[i] != 0x0a)
                    {
                        solid.Add(bytes[i]);
                        i++;
                    }
                    i++;
                    Elements.Add(Encoding.UTF8.GetString(code.ToArray()),
                        new Element
                        (
                             Encoding.UTF8.GetString(code.ToArray()),//gives correct value
                            $"{Encoding.UTF8.GetString(name.ToArray())}",//gives broken text due to encoding errors
                            name.ToArray(),
                            (float)BitConverter.ToDouble(value.ToArray(), 0),
                             BitConverter.ToBoolean(solid.ToArray(), 0)
                        ));
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Failed to find elements.csv");
            }
        }
    }

}
