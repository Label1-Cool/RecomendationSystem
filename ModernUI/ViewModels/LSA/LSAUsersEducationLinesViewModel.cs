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
    public class LSAUsersEducationLinesViewModel : INotifyPropertyChanged
    {
        #region Fields
        private Visibility _isVisibleProgressBar = Visibility.Hidden;
        private bool _isInitialized = false;

        private List<ItemPosition> _allUsersToEducationLine = new List<ItemPosition>();
        private List<ItemPosition> _allEducationLineToUsers = new List<ItemPosition>();
        private List<UserAndDistancesRow> _allUserToEducationLineDistance = new List<UserAndDistancesRow>();

        ObservableCollection<ItemPosition> _usersToDisplay = new ObservableCollection<ItemPosition>();
        private ItemPosition _selectedUser;

        private Dictionary<string, double> _LSADistance = new Dictionary<string, double>();
        private UserAndDistancesRow _selectedPureDistance = new UserAndDistancesRow();

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


        public List<ItemPosition> AllUsersToEducationLine
        {
            get { return _allUsersToEducationLine; }
        }
        public List<ItemPosition> AllEducationLineToUsers
        {
            get { return _allEducationLineToUsers; }
        }

        public List<UserAndDistancesRow> AllUserToEducationLineDistance
        {
            get { return _allUserToEducationLineDistance; }
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
                    if (_allEducationLineToUsers != null)
                    {
                        _LSADistance = value.CalculateOptimalDirections(_allEducationLineToUsers);
                        UpdateUI(new PropertyChangedEventArgs("LSADistance"));

                        _selectedPureDistance = (from item in _allUserToEducationLineDistance
                                         where item.Id == _selectedUser.Id
                                         select item).SingleOrDefault();
                        UpdateUI(new PropertyChangedEventArgs("SelectedPureDistance"));
                        UpdateUI(new PropertyChangedEventArgs("SelectedUser"));
                    }

                }
            }
        }

        /// <summary>
        /// Расстояние мд направлением и пользователем(ЛСА). Для отображения в DataGrid
        /// </summary>
        public Dictionary<string, double> LSADistance
        {
            get { return _LSADistance; }
        }

        /// <summary>
        /// Расстояние мд направлением и пользователем(ЛСА). Для отображения в DataGrid
        /// </summary>
        public UserAndDistancesRow SelectedPureDistance
        {
            get { return _selectedPureDistance; }
        }
        #endregion

        public async Task Init()
        {
            if (!_isInitialized)
            {
                IsVisibleProgressBar = Visibility.Visible;

                var analysis = UserToEducationLineAnalysis.Instance;

                await analysis.CalculateUserToEducationLinesForLSA();
                _allUsersToEducationLine = analysis.UsersToEducationLinesPosition;
                _allEducationLineToUsers = analysis.EducationLinesToUsersPosition;

                _allUserToEducationLineDistance = analysis.UsersToEducationsDistances;

                UpdateUI(new PropertyChangedEventArgs("AllUsersToEducationLine"));
                UpdateUI(new PropertyChangedEventArgs("AllEducationLineToUsers"));

                UpdateUI(new PropertyChangedEventArgs("AllUserToEducationLineDistance"));

                SelectedUser = _allUsersToEducationLine.FirstOrDefault();


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
