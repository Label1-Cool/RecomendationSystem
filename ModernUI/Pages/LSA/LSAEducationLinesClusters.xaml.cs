using ModernUI.ViewModels.LSA;
using System.Windows;
using System.Windows.Controls;

namespace ModernUI.Pages.LSA
{
    public partial class LSAEducationLinesClusters : UserControl
    {
        LSAEducationLinesClustersViewModel _vm = new LSAEducationLinesClustersViewModel();
        public LSAEducationLinesClusters()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _vm.Init();
        } 
    }
}
