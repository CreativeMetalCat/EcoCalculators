using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalCalc
{
    public class Machine
    {
        public string Name;

        [Obsolete("This was merged into name due to not having any impact on calculations")]
        //Определяющая хар-ка обородувания(например: Диаметр шлифовального круга,мм) 
        public int DefiningCharacteristic;

        //в какой катеогрии операций инструмент находится
        public string ActionType;

        //выделяющиеся в атмосферу вредные в-ва

        public Dictionary<string, float> DustTypes = new Dictionary<string, float>();

        public Machine(string name, string actionType)
        {
            Name = name;
            ActionType = actionType;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }

    //Класс который загружает информацию из таблицы
    public class MachineDataStorage
    {
        public List<Machine> Machines = new List<Machine>();

        public Machine this[string name, int characteristic]
        {
            get
            {
                return Machines.Where(p => p.DefiningCharacteristic == characteristic && p.Name == name)?.First() ?? throw new NullReferenceException("Оборудование подходящее под критерии не было найдено");
            }
        }

        public void Load()
        {
            //should be loading from table
            //for now filled manually

            //механическая обработка металлов
            if (System.IO.File.Exists("./tables/mechnical_processing_of_metals.csv"))
            {
                //reading as bytes because strings don't like russian symbols
                byte[] file = System.IO.File.ReadAllBytes("./tables/mechnical_processing_of_metals.csv");
                List<byte> bytes = new List<byte>(file);
                //separate data by lines where 0d0a is end line
                //lenght of first line with columns names is 260(inclding line return)
                int i = 260;
                while (i < bytes.Count)
                {
                    try
                    {
                        /*
                         * name is the first entry in the line
                         * special info is the second
                         * rest are dust types
                         */
                        List<byte> name = new List<byte>();
                        List<byte> dat = new List<byte>();
                        while (bytes[i] != 0x3B)
                        {
                            name.Add(bytes[i]);
                            i++;
                        }
                        //move past ";"
                        i++;
                        while (bytes[i] != 0x3B)
                        {
                            dat.Add(bytes[i]);
                            i++;
                        }
                        //move past ";"
                        i++;
                        //yeah i could have used ternary 
                        if (dat.Count > 0)
                        {
                            Machines.Add(new Machine($"{System.Text.Encoding.UTF8.GetString(name.ToArray()) } (D = {System.Text.Encoding.UTF8.GetString(dat.ToArray())})мм", "Механическая обработка металлов"));
                        }
                        else
                        {
                            Machines.Add(new Machine($"{System.Text.Encoding.UTF8.GetString(name.ToArray()) }", "Механическая обработка металлов"));
                        }
                        //not the pretties way but this way we can convert row id to element code via simle switch
                        int row = 0;

                        //Machines.Last().DustTypes.Add(2930, 0.006f);
                        while (bytes[i] != 0x0D && bytes[i + 1] != 0x0A)
                        {
                            List<byte> value = new List<byte>();
                            //read line bytes
                            while (bytes[i] != 0x3B/*0x3B is ;*/)
                            {
                                value.Add(bytes[i]);
                                i++;
                            }
                            if (value.Count > 0)
                            {
                                string code = "2930";
                                switch (row)
                                {
                                    case 1:
                                        code = "0123";
                                        break;
                                    case 2:
                                        code = "2917";
                                        break;
                                    case 3:
                                        code = "0228";
                                        break;
                                }
                                Machines.Last().DustTypes.Add(code, (float)Convert.ToDouble(System.Text.Encoding.UTF8.GetString(value.ToArray()), System.Globalization.CultureInfo.InvariantCulture));
                            }
                            row++;
                            i++;
                        }
                        i++;
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        break;
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
            }
        }
    }

}
