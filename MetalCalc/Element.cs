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
        public int Code;

        public string Name;

        //асбтрактное, более компактное имя(для тех случаев когда при счете выводится элемент который не совсем точен)
        public string SimpleName;

        //Норматив качества атмосферного воздузха в мг/м^3
        public float AirQualityStandart;

        //Технологические процессы при которых выделяется
        string ReasonToEmit;

        public Element(int code,string name, float airQualityStandart)
        {
            Name = name;
            AirQualityStandart = airQualityStandart;
            Code = code;
        }
    }

    //Handbles loading and storing of data from element table
    public class ElementDataStorage
    {
        public Dictionary<int, Element> Elements = new Dictionary<int, Element>();

        public void LoadData()
        {
            //load from table
            //currently just being filled manually

            Elements.Add(2930, new Element(0123,"Пыль абразивная (Корунд белый, Монокорунд)", 0.04f));
            Elements.Add(0123, new Element(2930,"диЖелезо триоксид (Железа оксид) (в пересчете на железо)", 0.04f));
        }
    }

}
