using BreastVMATAutomation.Model;
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
    /// Interaction logic for CalculationModelUI.xaml
    /// </summary>
    public partial class CalculationModelUI : UserControl
    {
        private LINACDefaultCalculationContext _dbContext;
        private LINACDefaultCalculation NewLINACDefaultCalculation = new LINACDefaultCalculation();
        private LINACDefaultCalculation SelectedLINACDefaultCalculation = new LINACDefaultCalculation();

        public CalculationModelUI(LINACDefaultCalculationContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            GetLINACDefaultCalculations();
            AddLINACDefaultCalculationModel.DataContext = NewLINACDefaultCalculation;
        }

        private void AddLINACDefaultCalculation_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.InsertLINACDefaultCalculation(NewLINACDefaultCalculation);
            GetLINACDefaultCalculations();
            NewLINACDefaultCalculation = new LINACDefaultCalculation();
            AddLINACDefaultCalculationModel.DataContext = NewLINACDefaultCalculation;
        }

        private void EditLINACDefaultCalculation_Click(object sender, RoutedEventArgs e)
        {
            SelectedLINACDefaultCalculation = (sender as FrameworkElement).DataContext as LINACDefaultCalculation;
            UpdateLINACDefaultCalculationModel.DataContext = SelectedLINACDefaultCalculation;
        }

        private void UpdateLINACDefaultCalculation_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.UpdateLINACDefaultCalculation(SelectedLINACDefaultCalculation);
            GetLINACDefaultCalculations();
            UpdateLINACDefaultCalculationModel.DataContext = null;
        }

        private void DeleteLINACDefaultCalculation_Click(object sender, RoutedEventArgs e)
        {
            var calculationToBeDeleted = (sender as FrameworkElement).DataContext as LINACDefaultCalculation;
            _dbContext.DeleteLINACDefaultCalculation(calculationToBeDeleted.LINAC);
            GetLINACDefaultCalculations();
        }

        private void GetLINACDefaultCalculations()
        {
            LINACDefaultCalculationDBDG.ItemsSource = _dbContext.GetAllLINACDefaultCalculations();
        }

        private void CancelUpdateLINACDefaultCalculation_Click(object sender, RoutedEventArgs e)
        {
            UpdateLINACDefaultCalculationModel.DataContext = null;
        }

        private void UpdateArcModel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
