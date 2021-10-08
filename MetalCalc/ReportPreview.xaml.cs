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
using System.Windows.Shapes;
using MetalCalc.Output;

namespace MetalCalc
{
    namespace Output
    { 
        public class Result
        {
            public string _code;
            public string Code => _code;

            public string _name;
            public string Name => _name;
            
            public string _res1;
            public string R1 => _res1;
            public string _res2;
            public string R2 => _res2;
            public string _j;
            public string J => _j;
            public string _res4;
            public string R4 => _res4;
            public string _res5;
            public string R5 => _res5;

            public Result(string code, string name, string res1, string res2, string j, string res4, string res5)
            {
                this._code = code;
                this._name = name;
                this._res1 = res1;
                this._res2 = res2;
                this._j = j;
                this._res4 = res4;
                this._res5 = res5;
            }
        }
    }

    /// <summary>
    /// Interaction logic for ReportPreview.xaml
    /// </summary>
    public partial class ReportPreview : Window
    {
        protected StringBuilder csv;

        protected List<Result> table = new List<Result>();
        public ReportPreview(StringBuilder _csv)
        {
            InitializeComponent();
            csv = _csv;
            var strings = csv.ToString().Split('\n');
            int i = 0;
            foreach (string st in strings)
            {
                if (i > 1)
                {
                    try
                    {
                        var gg = st.Split(";");
                        DataTable.Items.Add((new Result(gg[0], gg[1], gg[2], gg[3], gg[4], gg[5], gg[6])));
                    }
                    catch(System.IndexOutOfRangeException e)
                    {
                        break;
                    }
                }
                i++;
            }
            
        }
    }
}
