using BreastVMATAutomation.Model.Templating;
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

namespace BreastVMATAutomation.UI.ConfigContexts
{
    /// <summary>
    /// Interaction logic for ConfigOptions.xaml
    /// </summary>
    public partial class ConfigOptions : UserControl
    {
        public ConfigOptions()
        {
            InitializeComponent();
        }

        private void ArcSetupConfig_Click(object sender, RoutedEventArgs e)
        {
            var arcCTX = new ArcModelContext("v1");
            var window = new Window();
            window.Content = new ArcModelUI(arcCTX);
            window.ShowDialog();
        }

        private void LINACCalcDefault_Click(object sender, RoutedEventArgs e)
        {
            var arcCTX = new LINACDefaultCalculationContext("v1");
            var window = new Window();
            window.Content = new CalculationModelUI(arcCTX);
            window.ShowDialog();
        }

        private void BreastCLAlias_Click(object sender, RoutedEventArgs e)
        {
            var arcCTX = new StructureNamingAliasContext("v1");
            var window = new Window();
            window.Content = new StructureNamingAliasUI(arcCTX);
            window.ShowDialog();
        }
    }
}
