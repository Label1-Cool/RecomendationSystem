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
        private List<EducationLineToClusterAnalyzed> _allEducationLines = new List<EducationLineToClusterAnalyzed>();
        private List<ClusterAnalyzed> _allCluster = new List<ClusterAnalyzed>();

        private EducationLineToClusterAnalyzed _selectedEducationLine;
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

        
        public List<EducationLineToClusterAnalyzed> AllEducationLines
        {
            get { return _allEducationLines; }
        }


        public EducationLineToClusterAnalyzed SelectedEducationLine
        {
            get { return _selectedEducationLine; }
            set
            {
                if (_selectedEducationLine != value)
                {
                    _selectedEducationLine = value;

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

                var dataExtractor = DataExtractor.Instance;

                if (dataExtractor.EducationLinesAnalysed == null || dataExtractor.EducationLinesClustersAnalysed == null)
                {
                    await dataExtractor.CalculateEducationLinesToClusterForLSA();

                    _allEducationLines = dataExtractor.EducationLinesAnalysed;
                    _allCluster = dataExtractor.EducationLinesClustersAnalysed;
                    UpdateUI(new PropertyChangedEventArgs("AllEducationLines"));
                    UpdateUI(new PropertyChangedEventArgs("AllClusters"));
                }
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
