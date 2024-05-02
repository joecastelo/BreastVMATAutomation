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
    /// Interaction logic for ArcModelUI.xaml
    /// </summary>
    public partial class ArcModelUI : UserControl
    {


        private ArcModelContext _dbContext;
        private ArcModel NewArcModel = ArcModel.GetDefaultValues();
        private ArcModel SelectedArcModel = new ArcModel();
        public ArcModelUI(ArcModelContext dBContext)
        {
            _dbContext = dBContext;
            InitializeComponent();
            GetArcModels();
            AddNewArcModelGrid.DataContext = NewArcModel;


        }
        private void AddArcModel_Click(object s, RoutedEventArgs e)
        {
            _dbContext.InsertArc(NewArcModel);
            GetArcModels();
            NewArcModel = ArcModel.GetDefaultValues();
            AddNewArcModelGrid.DataContext = NewArcModel;
        }
        private void UpdateArcModelForEdit(object s, RoutedEventArgs e)
        {
            SelectedArcModel = (s as FrameworkElement).DataContext as ArcModel;
            UpdateArcModelGrid.DataContext = SelectedArcModel;
        }
        private void UpdateArcModel_Click(object s, RoutedEventArgs e)
        {
            _dbContext.UpdateArc(SelectedArcModel);
            GetArcModels();
            UpdateArcModelGrid.DataContext = null;

        }

        private void GetArcModels()
        {
            ArcModelDBDG.ItemsSource = _dbContext.GetAllArcs();
        }

        private void Delete_Click(object s, RoutedEventArgs e)
        {
            var productToBeDeleted = (s as FrameworkElement).DataContext as ArcModel;
            _dbContext.DeleteArc(productToBeDeleted.Id);
            GetArcModels();
        }


    }
}
