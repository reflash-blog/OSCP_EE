using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OSProject.Controller;
using OSProject.Model;
using OSProject.Model.Structures;
using OSProject.View;
using Sparrow.Chart;

namespace OSProject.ViewModel
{
    class UserViewModel:ViewModelBase
    {
        #region Constructor
        private static UserViewModel _userWindowViewModel;
        private UserViewModel() { }


        public static UserViewModel InitializeUserWindowViewModel()
        {
            return _userWindowViewModel ?? (_userWindowViewModel = new UserViewModel());
        }
        private ObservableCollection<ResultData> _resultData;
        #endregion

        #region ProgressBar
        private bool _taskRunned;

        public bool TaskRunned
        {
            get { return _taskRunned; }
            set
            {
                _taskRunned = value;
                RaisePropertyChanged("TaskRunned");
            }
        }
        #endregion

        #region DataCollections

        public int SelectedIndex { get; set; }                                                    // Selected item in data grid
        public Settings Settings { get; set; }
        public ObservableCollection<ResultData> ResultData // Result binding
        {
            get
            {
                if (_resultData != null) return _resultData;
                _resultData = InitialCollectionInitialization();
                return _resultData;
            }
            set
            {
                _resultData = value;
                RaisePropertyChanged("ResultData");
            }
        }


        #endregion

        #region Initializers
        private ObservableCollection<ResultData> InitialCollectionInitialization()
        {
            var collection = new ObservableCollection<ResultData>();
            for (var i = 0.0; i < 10; i += 0.1)
            {
                collection.Add(new ResultData { Consumption = Math.Round(i, 2), LevelDeviation = Math.Round(Math.Sin(i), 3), Pressure = Math.Round(Math.Cos(i), 3) });
            }
            return collection;
        }

        
        #endregion

        #region Commands

        #region OpenCommand
        RelayCommand _openCommand = null;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand((p) => OnOpen(p), (p) => CanOpen(p));
                }

                return _openCommand;
            }
        }

        private bool CanOpen(object parameter)
        {
            return true;
        }

        private async void OnOpen(object parameter)
        {
            await Open();
        }

        public async Task Open()
        {
            TaskRunned = true;
            ResultData = await FileSystemInteraction.OpenFile();
            TaskRunned = false;
        }

        #endregion

        #region OpenExcelCommand
        RelayCommand _openExcelCommand = null;
        public ICommand OpenExcelCommand
        {
            get
            {
                if (_openExcelCommand == null)
                {
                    _openExcelCommand = new RelayCommand((p) => OnOpenExcel(p), (p) => CanOpenExcel(p));
                }

                return _openExcelCommand;
            }
        }

        private bool CanOpenExcel(object parameter)
        {
            return true;
        }

        private async void OnOpenExcel(object parameter)
        {
            await OpenExcel();
        }

        public async Task OpenExcel()
        {
            TaskRunned = true;
            ResultData = await ExcelInteraction.OpenFile();
            TaskRunned = false;
        }

        #endregion

        #region SaveExcelCommand
        RelayCommand _saveExcelCommand = null;
        public ICommand SaveExcelCommand
        {
            get
            {
                if (_saveExcelCommand == null)
                {
                    _saveExcelCommand = new RelayCommand((p) => OnSaveExcel(p), (p) => CanSaveExcel(p));
                }

                return _saveExcelCommand;
            }
        }

        private bool CanSaveExcel(object parameter)
        {
            return true;
        }

        private async void OnSaveExcel(object parameter)
        {
            await SaveExcel();
        }

        public async Task SaveExcel()
        {
            TaskRunned = true;
            await ExcelInteraction.SaveFile(ResultData);
            TaskRunned = false;
        }

        #endregion

        #region SaveCommand
        RelayCommand _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanSave(p));
                }

                return _saveCommand;
            }
        }

        private bool CanSave(object parameter)
        {
            return true;
        }

        private async void OnSave(object parameter)
        {
            await Save();
        }

        public async Task Save()
        {
            TaskRunned = true;
            await FileSystemInteraction.SaveFile(ResultData);
            TaskRunned = false;
        }

        #endregion

        #region SaveAsCommand
        RelayCommand _saveAsCommand = null;
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand((p) => OnSaveAs(p), (p) => CanSaveAs(p));
                }

                return _saveAsCommand;
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return true;
        }

        private async void OnSaveAs(object parameter)
        {
            await SaveAs();
        }

        public async Task SaveAs()
        {
            TaskRunned = true;
            await FileSystemInteraction.SaveAsFile(ResultData);
            TaskRunned = false;
        }

        #endregion

        #region HelpCommand
        RelayCommand _helpCommand = null;
        public ICommand HelpCommand
        {
            get
            {
                if (_helpCommand == null)
                {
                    _helpCommand = new RelayCommand((p) => OnHelp(p), (p) => CanHelp(p));
                }

                return _helpCommand;
            }
        }

        private bool CanHelp(object parameter)
        {
            return File.Exists(@"help.chm");
        }

        private void OnHelp(object parameter)
        {
            Help();
        }

        public void Help()
        {
            Process.Start(@"help.chm");
        }

        #endregion 

        #region CloseCommand
        RelayCommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand((p) => OnClose(p), (p) => CanClose(p));
                }

                return _closeCommand;
            }
        }

        private bool CanClose(object parameter)
        {
            return true;
        }

        private void OnClose(object parameter)
        {
            Close(parameter as UserWindow);
        }

        public void Close(UserWindow window)
        {
            window.Close();
        }

        #endregion 

        #region DeleteItemCommand
        RelayCommand _deleteItemCommand = null;
        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand((p) => OnDeleteItem(p), (p) => CanDeleteItem(p));
                }

                return _deleteItemCommand;
            }
        }

        private bool CanDeleteItem(object parameter)
        {
            return true;
        }

        private  void OnDeleteItem(object parameter)
        {
            DeleteItem();
        }

        public void DeleteItem()
        {
            ResultData.RemoveAt(SelectedIndex);
            RaisePropertyChanged("ResultData"); RaisePropertyChanged("LineSeries"); RaisePropertyChanged("AreaSeries");
        }

        #endregion

        #region MoveUpCommand
        RelayCommand _moveUpCommand = null;
        public ICommand MoveUpCommand
        {
            get
            {
                if (_moveUpCommand == null)
                {
                    _moveUpCommand = new RelayCommand((p) => OnMoveUp(p), (p) => CanMoveUp(p));
                }

                return _moveUpCommand;
            }
        }

        private bool CanMoveUp(object parameter)
        {
            return true;
        }

        private void OnMoveUp(object parameter)
        {
            MoveUp();
        }

        public void MoveUp()
        {
            if (SelectedIndex == 0) return;
            ResultData.Move(SelectedIndex, SelectedIndex - 1);
            RaisePropertyChanged("ResultData"); RaisePropertyChanged("LineSeries"); RaisePropertyChanged("AreaSeries");
        }

        #endregion

        #region MoveDownCommand
        RelayCommand _moveDownCommand = null;
        public ICommand MoveDownCommand
        {
            get
            {
                if (_moveDownCommand == null)
                {
                    _moveDownCommand = new RelayCommand((p) => OnMoveDown(p), (p) => CanMoveDown(p));
                }

                return _moveDownCommand;
            }
        }

        private bool CanMoveDown(object parameter)
        {
            return true;
        }

        private void OnMoveDown(object parameter)
        {
            MoveDown();
        }

        public void MoveDown()
        {
            if (SelectedIndex == ResultData.Count) return;
            ResultData.Move(SelectedIndex, SelectedIndex + 1);
            RaisePropertyChanged("ResultData"); RaisePropertyChanged("LineSeries"); RaisePropertyChanged("AreaSeries");
        }

        #endregion

        #endregion
    }
}
