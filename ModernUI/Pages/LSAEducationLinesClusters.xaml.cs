﻿using FirstFloor.ModernUI.Windows;
using ModernUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ModernUI.Pages
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