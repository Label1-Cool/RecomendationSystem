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

namespace ModernUI.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {   
        #region Fields
        private Visibility _isVisibleProgressBar = Visibility.Hidden;
        private bool _isInitialized = false;
        private bool _isSelectAll;
        private List<UserAnalyzed> _allUsers = new List<UserAnalyzed>();
        private List<ClusterAnalyzed> _allCluster = new List<ClusterAnalyzed>();
        ObservableCollection<UserAnalyzed> _usersToDisplay = new ObservableCollection<UserAnalyzed>();
        private UserAnalyzed _selectedUser;
        private Dictionary<string, double> _resultDictionary = new Dictionary<string, double>();

        DataExtractor dataExtractor = new DataExtractor();
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

                    _usersToDisplay.Clear();
                    _resultDictionary.Clear();
                    //обновление отображаемых пользователей на графике и в таблице
                    if (_isSelectAll == true)
                    {
                        foreach (var analysedUser in _allUsers)
                        {
                            _usersToDisplay.Add(analysedUser);
                        }
                        //Так, или через observable collections(переписать)
                        //_resultDictionary = new Dictionary<string, double>();
                    }
                    UpdateUI(new PropertyChangedEventArgs("UsersToDisplay"));
                    UpdateUI(new PropertyChangedEventArgs("ResultDictionary"));
                }
            }
        }

        
        public List<UserAnalyzed> AllUsers
        {
            get { return _allUsers; }
            set
            {
                if (_allUsers != value)
                {
                    _allUsers = value;

                    UpdateUI(new PropertyChangedEventArgs("AllUsers"));
                }
            }
        }
        
        
        public ObservableCollection<UserAnalyzed> UsersToDisplay
        {
            get { return _usersToDisplay; }
            set
            {
                if (_usersToDisplay != value)
                {
                    _usersToDisplay = value;
                }
            }
        }

        
        public UserAnalyzed SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;

                    //обновление отображаемых пользователей
                    _usersToDisplay.Clear();
                    _usersToDisplay.Add(value);
                    UpdateUI(new PropertyChangedEventArgs("UsersToDisplay"));

                    //Обновляем информацию в табличной форме
                    if (_allCluster!=null)
                    {
                        _resultDictionary = value.CalculateOptimalDirections(_allCluster);
                        UpdateUI(new PropertyChangedEventArgs("ResultDictionary"));
                    }
                    
                }
            }
        }

        
        public List<ClusterAnalyzed> AllClusters
        {
            get { return _allCluster; }
            set 
            {
                if (_allCluster != value)
                {
                    _allCluster = value;

                    UpdateUI(new PropertyChangedEventArgs("AllClusters"));
                }
            }
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

                await dataExtractor.Init();
                AllUsers = dataExtractor.UsersCoords;
                AllClusters = dataExtractor.ClustersCoords;

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
