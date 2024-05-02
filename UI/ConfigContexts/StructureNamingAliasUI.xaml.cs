using BreastVMATAutomation.Model.Templating;
using System.Windows;
using System.Windows.Controls;

namespace BreastVMATAutomation.UI.ConfigContexts
{
    /// <summary>
    /// Interaction logic for StructureNamingAliasUI.xaml
    /// </summary>
    public partial class StructureNamingAliasUI : UserControl
    {
        private StructureNamingAliasContext _dbContext;
        private StructureNamingAlias NewStructureNamingAlias = new StructureNamingAlias();
        private StructureNamingAlias SelectedStructureNamingAlias = new StructureNamingAlias();

        public StructureNamingAliasUI(StructureNamingAliasContext dbContext)
        {
            _dbContext = dbContext;
            InitializeComponent();
            GetStructureNamingAliases();
            AddStructureNamingAliasModel.DataContext = NewStructureNamingAlias;
        }

        private void AddStructureNamingAlias_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.InsertStructureNamingAlias(NewStructureNamingAlias);
            GetStructureNamingAliases();
            NewStructureNamingAlias = new StructureNamingAlias();
            AddStructureNamingAliasModel.DataContext = NewStructureNamingAlias;
        }

        private void EditStructureNamingAlias_Click(object sender, RoutedEventArgs e)
        {
            SelectedStructureNamingAlias = (sender as FrameworkElement).DataContext as StructureNamingAlias;
            UpdateStructureNamingAliasModel.DataContext = SelectedStructureNamingAlias;
        }

        private void UpdateStructureNamingAlias_Click(object sender, RoutedEventArgs e)
        {
            _dbContext.UpdateStructureNamingAlias(SelectedStructureNamingAlias);
            GetStructureNamingAliases();
            UpdateStructureNamingAliasModel.DataContext = null;
        }

        private void DeleteStructureNamingAlias_Click(object sender, RoutedEventArgs e)
        {
            var aliasToBeDeleted = (sender as FrameworkElement).DataContext as StructureNamingAlias;
            _dbContext.DeleteStructureNamingAlias(aliasToBeDeleted.IdOnApplication);
            GetStructureNamingAliases();
        }

        private void GetStructureNamingAliases()
        {
            StructureNamingAliasDBDG.ItemsSource = _dbContext.GetAllStructureNamingAliases();
        }

        private void CancelUpdateStructureNamingAlias_Click(object sender, RoutedEventArgs e)
        {
            UpdateStructureNamingAliasModel.DataContext = null;
        }
    }
}
