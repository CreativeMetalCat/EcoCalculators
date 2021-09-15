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

        //Определяющая хар-ка обородувания(например: Диаметр шлифовального круга,мм)
        public int DefiningCharacteristic;

        //в какой катеогрии операций инструмент находится
        public string ActionType;

        //выделяющиеся в атмосферу вредные в-ва

        public Dictionary<int, float> DustTypes = new Dictionary<int, float>();

        public Machine(string name, int definingCharacteristic, string actionType)
        {
            Name = name;
            DefiningCharacteristic = definingCharacteristic;
            ActionType = actionType;
        }

        public override string ToString()
        {
            return $"{Name} ({DefiningCharacteristic})";
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

            Machines.Add(new Machine("Заточные Станки", 100, "Механическая обработка металлов"));
            Machines.Last().DustTypes.Add(0123, 0.004f);
            Machines.Last().DustTypes.Add(2930, 0.006f);

            Machines.Add(new Machine("Заточные Станки", 150, "Механическая обработка металлов"));
            Machines.Last().DustTypes.Add(0123, 0.006f);
            Machines.Last().DustTypes.Add(2930, 0.008f);

            Machines.Add(new Machine("Заточные Станки", 200, "Механическая обработка металлов"));
            Machines.Last().DustTypes.Add(0123, 0.008f);
            Machines.Last().DustTypes.Add(2930, 0.012f);

            Machines.Add(new Machine("Заточные Станки", 250, "Механическая обработка металлов"));
            Machines.Last().DustTypes.Add(0123, 0.011f);
            Machines.Last().DustTypes.Add(2930, 0.016f);

            Machines.Add(new Machine("Заточные Станки", 300, "Механическая обработка металлов"));
            Machines.Last().DustTypes.Add(0123, 0.013f);
            Machines.Last().DustTypes.Add(2930, 0.021f);
        }
    }

}
