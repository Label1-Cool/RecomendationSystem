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

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {   
        #region Fields
        Progress<int> progressIndicator = new Progress<int>();
        static int _currentProgress = 0;

        private Visibility _isVisibleProgressBar = Visibility.Visible;
        private bool _isSelectAll;
        private List<UserAnalyzed> _allUsers = new List<UserAnalyzed>();
        private List<ClusterAnalyzed> _allCluster = new List<ClusterAnalyzed>();
        ObservableCollection<UserAnalyzed> _usersToDisplay = new ObservableCollection<UserAnalyzed>();
        private UserAnalyzed _selectedUser;
        private Dictionary<string, double> _resultDictionary = new Dictionary<string, double>();

        DataExtractor dataExtractor = new DataExtractor();
        #endregion

        #region Properties

        
        public int CurrentProgress
        {
            get{return _currentProgress;}
            set
            {
                if (_currentProgress!=value)
                {
                    _currentProgress = value;

                    if (_currentProgress == 100)
                    {
                        IsVisibleProgressBar = Visibility.Hidden;
                    }
                    UpdateUI(new PropertyChangedEventArgs("CurrentProgress"));
                }
            }
        }

        
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
                        foreach (var user in _allUsers)
                        {
                            _usersToDisplay.Add(user);
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

        public MainViewModel()
        {
            progressIndicator.ProgressChanged += progressIndicator_ProgressChanged;
            Init();
        }

        void progressIndicator_ProgressChanged(object sender, int e)
        {
            CurrentProgress = e;
        }

        private async void Init()
        {
            await dataExtractor.Init(progressIndicator);
            AllUsers = dataExtractor.UsersCoords;
            AllClusters = dataExtractor.ClustersCoords;
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
