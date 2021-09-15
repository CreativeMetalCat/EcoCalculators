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

        protected ReportType reportType;

        protected ElementDataStorage elementData;

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

        public MainWindow()
        {
            InitializeComponent();

            elementData = new ElementDataStorage();
            elementData.LoadData();

            machineData = new MachineDataStorage();
            machineData.Load();
            Operation.SelectedIndex = 0;
            UpdateMachineList();

            //заполнить список обородувания на основе данных из таблицы
            Machines.ItemsSource = machines;
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (Machines.SelectedItem is Machine machine)
            {
                List<string> res = new List<string>();

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Код;Название вещества;Без учета очистки;Без учета очистки;Очистка(j);С учетом очистки;С учетом очистки");
                csv.AppendLine(" ; ;г/с;т/г;%;г/с;т/г");

                //введеные данные
                //кол-во станков
                int n = Convert.ToInt32(WorkplaceCount.Text);

                //время действия ИЗА в течении 20-ти минутного интервала времени в секундах
                int t = Convert.ToInt32(CycleLenght_Text.Text) * 60;

                int T = Convert.ToInt32(TimePerYear_Text.Text);

                if(ReportTypeSelector.SelectedItem is ComboBoxItem item)
                {
                    if(!Enum.TryParse<ReportType>(item.Name, out reportType))
                    {
                        throw new Exception("Ошибка в выборе вида отчета.\n Unable to parse report name");
                    }
                }

                //высчитать для каждого типа пыли
                foreach (var dust in machine.DustTypes)
                {
                    //записать значения из тиблиц на основе введеных данных
                    //удельное выделение i-го загрязняющего в-ва
                    //берется из таблицы
                    float q = dust.Value;

                    //поправочный коэф
                    //берется на основе оборудования
                    //Для металлической и абразивной пыли 0.2, для других твердых компонентов (и компонентов СОЖ) 0.4
                    float kGrav = 0.2f;

                    float K0 = 1;
                    switch (reportType)
                    {
                        case ReportType.NoSuck:
                            K0 = 1;
                            break;
                        case ReportType.Full:
                            K0 = (float)Convert.ToDouble(LocalSuckEfficiency.Text);
                            break;
                        case ReportType.FromRoom:
                            K0 = 1 - (float)Convert.ToDouble(LocalSuckEfficiency.Text);
                            break;
                    }

                    //степень очистки воздуха
                    float j = 0;

                    //расчет выброса пыли
                    float Mv = n * kGrav * q * K0 * t / 1200;

                    float Muog = Mv * (1 - j);

                    //валовый выброс
                    float Mgv = 3.6f * n * q * kGrav * K0 * T * 0.001f;
                    float Moug = Mgv * (1 - j);

                    res.Add($"Мв = {Math.Round(Mv, 7)}, МвУОГ = {Math.Round(Muog, 7)},МвГ = {Math.Round(Mgv, 7)}, МвГУОГ = {Math.Round(Moug, 7)}");
                    csv.AppendLine($"{dust.Key};{elementData.Elements[dust.Key].Name};{Math.Round(Mv, 7)};{Math.Round(Mgv, 7)};{j};{Math.Round(Muog, 7)};{Math.Round(Moug, 7)}");
                }
                string operationName = (Operation.SelectedItem as ComboBoxItem).Content.ToString();
                csv.AppendLine();
                csv.AppendLine($"Технологическая операция: ;{operationName}");
                csv.AppendLine($"Вид оборудования: ;{machine.Name}");
                csv.AppendLine($"Тип охлаждения: ;##$#%%&^&(^*&{machine.Name}");
                csv.AppendLine($"Кол-во станков: ;{n}");
                csv.AppendLine($"Время работы станка за год (T): ;{T}");          
                csv.AppendLine($"Продолжительность производственного цикла (ti): : ;{t}(сек)/{t / 60f}(мин)");
                csv.AppendLine($"Тип отчета : ;{(ReportTypeSelector.SelectedItem as ComboBoxItem).Content}");
                System.IO.File.WriteAllText($"./calc_{operationName}_{DateTime.Now.ToFileTime()}.csv", csv.ToString(), Encoding.UTF8);            
            }
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
    }
}
