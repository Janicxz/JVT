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

namespace JVTWpf
{
    /// <summary>
    /// Interaction logic for UserControlSlider.xaml
    /// </summary>
    public partial class UserControlSlider : UserControl
    {
        public UserControlSlider()
        {
            InitializeComponent();
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = 
            DependencyProperty.Register("Minimum", typeof(double), typeof(UserControlSlider), new UIPropertyMetadata(0d));

        public double StartValue
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(UserControlSlider), new UIPropertyMetadata(0d));

        public double CurrentValue
        {
            get { return (double)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(UserControlSlider), new UIPropertyMetadata(0d));


        public double EndValue
        {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(UserControlSlider), new UIPropertyMetadata(0d));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(UserControlSlider), new UIPropertyMetadata(0d));

    }
}
