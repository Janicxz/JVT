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

using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace JVTCap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Sup");
            int adapterNum = 0, outputNum = 0;
            Adapter1 adapter = new Factory1().GetAdapter1(adapterNum);
            SharpDX.Direct3D11.Device device = new SharpDX.Direct3D11.Device(adapter);
            Output output = adapter.GetOutput(outputNum);


        }
    }
}
