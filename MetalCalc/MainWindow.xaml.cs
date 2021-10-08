using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetalCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected enum ReportType
        {
            Full,
            NoSuck,
            FromRoom,
            FromSource
        }

        protected enum CoolingType
        {
            NoCooling,
            Oil_Grinding,
            Emuls_Grinding_Less_3,
            Emuls_Grinding_Less_3_10,
            Oil_NotGrinding,
            Emuls_NotGrinding_Less_3,
            Emuls_NotGrinding_Less_3_10,
            Water
        }
        //СОЖ
        protected System.Collections.Generic.List<Element> SOJ = new List<Element>();

        ReportPreview preview;

        protected ReportType reportType;

        protected CoolingType coolingType;

        protected ElementDataStorage elementData;

        StringBuilder csv = new StringBuilder();

        protected MachineDataStorage machineData;

        protected System.Collections.ObjectModel.ObservableCollection<Machine> machines = new System.Collections.ObjectModel.ObservableCollection<Machine>();

        protected void UpdateMachineList()
        {
            machines.Clear();
            //заполнить массив с обородуванием
            foreach (Machine machine in machineData.Machines)
            {
                if (machine.ActionType == (Operation.SelectedItem as ComboBoxItem).Content.ToString())
                {
                    machines.Add(machine);
                }
            }
            if(machines.Count > 0)
            {
                Machines.SelectedIndex = 0;
            }
        }

        protected void LoadSojData(string file = "./soj.csv")
        {
            //this is more of a test function
            var lines = System.IO.File.ReadAllLines(file);
            foreach(var line in lines)
            {
                var data = line.Split(';');
                SOJ.Add(new Element(data[0], data[2], (float)Convert.ToDouble(data[3], System.Globalization.CultureInfo.InvariantCulture)));
                SOJ.Last<Element>().SimpleName = data[1];
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            elementData = new ElementDataStorage();
            elementData.LoadData();
            foreach(var elem in elementData.Elements)
            {
                elems.Items.Add(elem.Value);
            }

            machineData = new MachineDataStorage();
            machineData.Load();
            Operation.SelectedIndex = 0;
            UpdateMachineList();

            //заполнить список обородувания на основе данных из таблицы
            Machines.ItemsSource = machines;

            LoadSojData();
        }

        private void calculateEmit(float q,int n,int t,float N,int T,string elemCode,string name,StringBuilder resultCSV,bool soj = false)
        {
            //записать значения из тиблиц на основе введеных данных
            //удельное выделение i-го загрязняющего в-ва
            //берется из таблицы

            //поправочный коэф
            //берется на основе оборудования
            //Для металлической и абразивной пыли 0.2, для других твердых компонентов (и компонентов СОЖ) 0.4
            float kGrav = soj ? 0.4f : ((coolingType == CoolingType.Water || coolingType == CoolingType.NoCooling)? 0.2f : 0.4f);

            float K0 = 1;
            switch (reportType)
            {
                case ReportType.NoSuck:
                    K0 = 1;
                    break;
                case ReportType.Full:
                    K0 = (float)Convert.ToDouble(LocalSuckEfficiency.Text, System.Globalization.CultureInfo.InvariantCulture);
                    break;
                case ReportType.FromRoom:
                    K0 = 1 - (float)Convert.ToDouble(LocalSuckEfficiency.Text, System.Globalization.CultureInfo.InvariantCulture);
                    break;
                case ReportType.FromSource:
                    K0 = 1 - (float)Convert.ToDouble(LocalSuckEfficiency.Text, System.Globalization.CultureInfo.InvariantCulture);
                    kGrav = 1;
                    break;
            }
            //степень очистки воздуха
            float j = 0;

            //расчет выброса пыли
            float Mv = n * kGrav * q * K0 * t / 1200 * N;

            float Muog = Mv * (1 - j);

            //валовый выброс
            float Mgv = 3.6f * n * q * kGrav * K0 * T * 0.001f * N;
            float Moug = Mgv * (1 - j);

            resultCSV.AppendLine($"{elemCode};{name};{Math.Round(Mv, 7)};{Math.Round(Mgv, 7)};{j};{Math.Round(Muog, 7)};{Math.Round(Moug, 7)}");
        }

        private void DoCalculations()
        {
            if (Machines.SelectedItem is Machine machine)
            {
                List<string> res = new List<string>();


                csv.AppendLine("Код;Название вещества;Без учета очистки;Без учета очистки;Очистка(j);С учетом очистки;С учетом очистки");
                csv.AppendLine(" ; ;г/с;т/г;%;г/с;т/г");

                //введеные данные
                //кол-во станков
                int n = Convert.ToInt32(WorkplaceCount.Text);

                //время действия ИЗА в течении 20-ти минутного интервала времени в секундах
                int t = Convert.ToInt32(CycleLenght_Text.Text) * 60;

                int T = Convert.ToInt32(TimePerYear_Text.Text);
                float N = 1;  //мощность станка
                    N = (coolingType == CoolingType.NoCooling) ? 1 : (float)Convert.ToDouble(Power.Text.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                

                if (ReportTypeSelector.SelectedItem is ComboBoxItem item)
                {
                    if (!Enum.TryParse<ReportType>(item.Name, out reportType))
                    {
                        throw new Exception("Ошибка в выборе вида отчета.\n Unable to parse report name. Please check for naming errors");
                    }
                }

                if (CoolingTypeSelector.SelectedItem is ComboBoxItem itemc)
                {
                    if (!Enum.TryParse<CoolingType>(itemc.Name, out coolingType))
                    {
                        throw new Exception("Ошибка в выборе типа охлаждения.\n Unable to parse cooling type name. Please check for naming errors");
                    }
                }
                if (
                    coolingType != CoolingType.Emuls_NotGrinding_Less_3 &&
                    coolingType != CoolingType.Emuls_NotGrinding_Less_3_10 &&
                    coolingType != CoolingType.Oil_NotGrinding
                    )
                {
                    //высчитать для каждого типа пыли
                    foreach (var dust in machine.DustTypes)
                    {
                        calculateEmit(dust.Value * ((coolingType.ToString().Contains("Grinding") || coolingType == CoolingType.Water) ? 0.1f : 1f), n, t, N, T, dust.Key, elementData.Elements[dust.Key].Name, csv);
                    }
                }
                if (coolingType != CoolingType.Water && coolingType != CoolingType.NoCooling)
                {
                    string name = (CoolingTypeSelector.SelectedItem as ComboBoxItem).Content.ToString();
                    foreach (var soj in SOJ)
                    {
                        if (soj.Name == name)
                        {
                            calculateEmit(soj.AirQualityStandart*0.000001f, n, t, N, T, soj.Code, soj.SimpleName, csv, true);
                            break;
                        }
                    }
                    if (Trian.SelectedIndex == 1)
                    {
                        calculateEmit(0.0000003f, n, t, N, T, "2197", "Три(2 - гидросиэтил)амин(Триэтаноламин)", csv, true);
                    }
                }
                
                string operationName = (Operation.SelectedItem as ComboBoxItem).Content.ToString();
                csv.AppendLine();
                csv.AppendLine($"Технологическая операция: ;{operationName}");
                csv.AppendLine($"Вид оборудования: ;{machine.Name}");
                csv.AppendLine($"Кол-во станков: ;{n}");
                csv.AppendLine($"Время работы станка за год (T): ;{T}");
                csv.AppendLine($"Продолжительность производственного цикла (ti): ;{t}(сек) -> {t / 60f}(мин)");
                csv.AppendLine($"Мощность станка (N): ;{N}");
                float K0 = 1;
                switch (reportType)
                {
                    case ReportType.NoSuck:
                        K0 = 1;
                        break;
                    case ReportType.Full:
                        K0 = (float)Convert.ToDouble(LocalSuckEfficiency.Text, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case ReportType.FromRoom:
                    case ReportType.FromSource:
                        K0 = 1 - (float)Convert.ToDouble(LocalSuckEfficiency.Text, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                }
                csv.AppendLine($"Эффективность местных отсосов;{K0}");
                csv.AppendLine($"Тип отчета : ;{(ReportTypeSelector.SelectedItem as ComboBoxItem).Content.ToString()}");
                csv.AppendLine($"Тип охлаждения : ;{(CoolingTypeSelector.SelectedItem as ComboBoxItem).Content.ToString()}");

            }
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            DoCalculations();
            string operationName = (Operation.SelectedItem as ComboBoxItem).Content.ToString();
            System.IO.File.WriteAllText($"./calc_{operationName}_{DateTime.Now.ToFileTime()}.csv", csv.ToString(), Encoding.UTF8);

        }
        

        private void Operation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //change contents of the list
            UpdateMachineList();
        }

        private void TextboxNumCheck_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            DoCalculations();
            preview?.Close();
           preview = new ReportPreview(csv);
            preview.Show();
        }
    }
}
