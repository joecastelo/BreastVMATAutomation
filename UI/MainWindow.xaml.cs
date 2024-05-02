using BreastVMATAutomation.Model;
using BreastVMATAutomation.Model.ESAPI;
using BreastVMATAutomation.Model.Templating;
using BreastVMATAutomation.UI.ConfigContexts;
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
using VMS.TPS.Common.Model.API;

namespace BreastVMATAutomation.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private ExternalPlanSetup _plan;
        public bool RDS { get => _plan.Beams.First().TreatmentUnit.MachineModelName.ToUpper().Contains("RDS"); }

        public MainWindow(ExternalPlanSetup planSetup)
        {
            _plan = planSetup;
            InitializeComponent();
            GetValidTargetStructures();
            GetValidArcSetups();
            GetValidBreastCLStructures();
            IsItOkToRunScript();

        }



        public void IsItOkToRunScript()
        {
            if (RDS)
            {
                NonRDS.Visibility = Visibility.Hidden;
            }
            if (_plan.IsDoseValid || _plan.ApprovalStatus != VMS.TPS.Common.Model.Types.PlanSetupApprovalStatus.UnApproved) 
            {
                MessageBox.Show("Cannot run Breast Geometry Tool on Approved/Rejected or Calculated Plans\n" +
                    "Reset Calculation or Unnaprove plan");
                Apply.IsEnabled = false;
            }
        }
        private void GetValidArcSetups()
        {
            var ctx = new ArcModelContext("v1");
            var models = ctx.GetAllArcs();
            Setups.ItemsSource = models;
            Setups.SelectedItem = models.First();
        }


        private void GetValidTargetStructures()
        {
            var ss = _plan.StructureSet.Structures.Where(e => e.Volume > 0);

            ss = ss.OrderByDescending(s => s.Id.Contains("ptv") && s.Id.Contains("Soma"))
                                    .ThenByDescending(s => s.Id.Contains("PTV"));
            Structures.ItemsSource = ss;
            if (ss.Any(e => e.Id == "z_ptvSoma"))
            {
                Structures.SelectedItem = ss.FirstOrDefault(e=>e.Id == "z_ptvSoma");
            }
            else
            {
                Structures.SelectedItem = ss.First();
            }
        }
        private void GetValidBreastCLStructures()
        {
            var ss = _plan.StructureSet.Structures.Where(e => e.Volume > 0);
            try
            {
                var ctx = new StructureNamingAliasContext("v1");
                var aliasBreastCL = ctx.GetAllStructureNamingAliases().First(e => e.IdOnApplication == "Breast Contralateral").Alias.Split(';');
                var orderedList = ss.OrderBy(item => aliasBreastCL.Contains(item.Id) ? 0 : 1);
                BreastCLSelection.ItemsSource = orderedList;
                BreastCLSelection.SelectedItem = orderedList.First();


            }
            catch (Exception)
            {

                BreastCLSelection.ItemsSource = ss;
                BreastCLSelection.SelectedItem = ss.First(x=>x.DicomType == "ORGAN");

            }


        }
        public static bool IsLeftBreast(Structure sumPTV, Structure body)
        {
            return sumPTV.CenterPoint.x > body.CenterPoint.x;

        }
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var linacDefaultCalculation = new LINACDefaultCalculation();
            var esapiWorker = new BreastGeometryTool(_plan, Structures.SelectedItems.Cast<Structure>(),
                (ArcModel) Setups.SelectedItem, 
                new NonRDSOptions(PREVENT15.IsChecked.Value, ADDNODES.IsChecked.Value, CLBlock.IsChecked.Value, 
                (Structure) BreastCLSelection.SelectedItem));
            esapiWorker.ChangeIsocenter();
            esapiWorker.CreateArcsBasedOnModel();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            var configScreen = new ConfigOptions();
            var window = new Window();
            window.Height = 80;
            window.Width = 350;
            window.Content = configScreen;
            window.ShowDialog();
        }

        private void RefreshSetups_Click(object sender, RoutedEventArgs e)
        {
            GetValidArcSetups();
        }

        private void CLBlock_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
            {
                GetValidBreastCLStructures();

                BreastCLSelection.IsEnabled = true;
            }
            else
            {
                BreastCLSelection.IsEnabled = false;
            }

        }
    }
}
