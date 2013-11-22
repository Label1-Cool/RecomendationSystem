using Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Logic.Models;
using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI.Windows;
using Logic.Analysis;

namespace ModernUI.ViewModels.LSA
{
    public class LSAUsersClustersViewModel : INotifyPropertyChanged
    {   
        #region Fields
        private Visibility _isVisibleProgressBar = Visibility.Hidden;
        private bool _isInitialized = false;
        private bool _isSelectAll;
        private List<ItemPosition> _allUsers = new List<ItemPosition>();
        private List<ItemPosition> _allCluster = new List<ItemPosition>();
        private ItemPosition _selectedUser;
        private Dictionary<string, double> _resultDictionary = new Dictionary<string, double>();

        
        #endregion

        #region Properties
        
        public Visibility IsVisibleProgressBar
        {
            get { return _isVisibleProgressBar; }
            set
            {
                if ( _isVisibleProgressBar!=value)
                {
                     _isVisibleProgressBar = value;

                    UpdateUI(new PropertyChangedEventArgs("IsVisibleProgressBar"));
                }
            }
        }
        

        
        public bool IsSelectAll
        {
            get { return _isSelectAll; }
            set
            {
                if (_isSelectAll != value)
                {
                    _isSelectAll = value;

                    _resultDictionary.Clear();
                    UpdateUI(new PropertyChangedEventArgs("ResultDictionary"));
                }
            }
        }

        
        public List<ItemPosition> AllUsers
        {
            get { return _allUsers; }
        }

        public ItemPosition SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;

                    //Обновляем информацию в табличной форме
                    if (_allCluster!=null)
                    {
                        _resultDictionary = value.CalculateOptimalDirections(_allCluster);
                        UpdateUI(new PropertyChangedEventArgs("ResultDictionary"));

                        UpdateUI(new PropertyChangedEventArgs("SelectedUser"));
                    }
                    
                }
            }
        }


        public List<ItemPosition> AllClusters
        {
            get { return _allCluster; }
        }

        /// <summary>
        /// Для отображения в DataGrid
        /// </summary>
        public Dictionary<string,double> ResultDictionary
        {
            get { return _resultDictionary; }
            set { _resultDictionary = value; }
        }
         
        #endregion

        public async Task Init()
        {
            if (!_isInitialized)
            {
                IsVisibleProgressBar = Visibility.Visible;

                var analysis = UserToClusterAnalysis.Instance;

                await analysis.CalculateUserToClusterForLSA(); 

                _allUsers = analysis.UsersToClusterPosition;
                _allCluster = analysis.ClustersToUserPosition;
                UpdateUI(new PropertyChangedEventArgs("AllUsers"));
                UpdateUI(new PropertyChangedEventArgs("AllClusters"));

                SelectedUser = _allUsers.FirstOrDefault();

                IsVisibleProgressBar = Visibility.Hidden;
                _isInitialized = true;
            }
        }
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void UpdateUI(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler!=null)
            {
                handler(this, args);
            }
        }
        #endregion
    }
}
