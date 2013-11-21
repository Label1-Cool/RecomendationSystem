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
    public class LSAUsersEducationLinesViewModel : INotifyPropertyChanged
    {
        #region Fields
        private Visibility _isVisibleProgressBar = Visibility.Hidden;
        private bool _isInitialized = false;

        private List<UserEducationLineAnalysed> _allUsersToEducationLine = new List<UserEducationLineAnalysed>();
        private List<EducationLineToUserAnalysed> _allEducationLineToUsers = new List<EducationLineToUserAnalysed>();

        ObservableCollection<UserEducationLineAnalysed> _usersToDisplay = new ObservableCollection<UserEducationLineAnalysed>();
        private UserEducationLineAnalysed _selectedUser;

        private Dictionary<string, double> _distance = new Dictionary<string, double>();

        #endregion

        #region Properties

        public Visibility IsVisibleProgressBar
        {
            get { return _isVisibleProgressBar; }
            set
            {
                if (_isVisibleProgressBar != value)
                {
                    _isVisibleProgressBar = value;

                    UpdateUI(new PropertyChangedEventArgs("IsVisibleProgressBar"));
                }
            }
        }


        public List<UserEducationLineAnalysed> AllUsersToEducationLine
        {
            get { return _allUsersToEducationLine; }
        }
        public List<EducationLineToUserAnalysed> AllEducationLineToUsers
        {
            get { return _allEducationLineToUsers; }
        }

        public UserEducationLineAnalysed SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;

                    //Обновляем информацию в табличной форме
                    if (_allEducationLineToUsers != null)
                    {
                        _distance = value.CalculateOptimalDirections(_allEducationLineToUsers);
                        UpdateUI(new PropertyChangedEventArgs("Distance"));
                    }

                }
            }
        }

        /// <summary>
        /// Расстояние мд направлением и пользователем. Для отображения в DataGrid
        /// </summary>
        public Dictionary<string, double> Distance
        {
            get { return _distance; }
        }

        #endregion

        public async Task Init()
        {
            if (!_isInitialized)
            {
                IsVisibleProgressBar = Visibility.Visible;

                var dataExtractor = DataExtractor.Instance;

                await dataExtractor.CalculateUserToEducationLinesForLSA();
                _allUsersToEducationLine = dataExtractor.UsersToEducationLinesAnalysed;
                _allEducationLineToUsers = dataExtractor.EducationLinesToUsersAnalysed;

                UpdateUI(new PropertyChangedEventArgs("AllUsersToEducationLine"));
                UpdateUI(new PropertyChangedEventArgs("AllEducationLineToUsers"));

                IsVisibleProgressBar = Visibility.Hidden;
                _isInitialized = true;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void UpdateUI(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion
    }
}
