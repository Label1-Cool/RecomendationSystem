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
    public class LSAEducationLinesClustersViewModel : INotifyPropertyChanged
    {   
        #region Fields
        private Visibility _isVisibleProgressBar = Visibility.Hidden;
        private bool _isInitialized = false;
        private bool _isSelectAll;
        private List<EducationLineAnalyzed> _allEducationLines = new List<EducationLineAnalyzed>();
        private List<ClusterAnalyzed> _allCluster = new List<ClusterAnalyzed>();

        ObservableCollection<EducationLineAnalyzed> _educationLinesToDisplay = new ObservableCollection<EducationLineAnalyzed>();
        private EducationLineAnalyzed _selectedEducationLine;
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

                    _educationLinesToDisplay.Clear();
                    _resultDictionary.Clear();
                    //обновление отображаемых пользователей на графике и в таблице
                    if (_isSelectAll == true)
                    {
                        foreach (var analysedEducationLine in _allEducationLines)
                        {
                            _educationLinesToDisplay.Add(analysedEducationLine);
                        }
                        //Так, или через observable collections(переписать)
                        //_resultDictionary = new Dictionary<string, double>();
                    }
                    
                    UpdateUI(new PropertyChangedEventArgs("ResultDictionary"));
                }
            }
        }

        
        public List<EducationLineAnalyzed> AllEducationLines
        {
            get { return _allEducationLines; }
            set
            {
                if (_allEducationLines != value)
                {
                    _allEducationLines = value;

                    UpdateUI(new PropertyChangedEventArgs("AllEducationLines"));
                }
            }
        }


        public ObservableCollection<EducationLineAnalyzed> EducationLinesToDisplay
        {
            get { return _educationLinesToDisplay; }
            set
            {
                if (_educationLinesToDisplay != value)
                {
                    _educationLinesToDisplay = value;
                }
            }
        }


        public EducationLineAnalyzed SelectedEducationLine
        {
            get { return _selectedEducationLine; }
            set
            {
                if (_selectedEducationLine != value)
                {
                    _selectedEducationLine = value;

                    //обновление отображаемых пользователей
                    _educationLinesToDisplay.Clear();
                    _educationLinesToDisplay.Add(value);

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

                await dataExtractor.CalculateEducationLinesToClusterForLSA();
                AllEducationLines = dataExtractor.EducationLinesAnalysed;
                AllClusters = dataExtractor.EducationLinesClustersAnalysed;

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
