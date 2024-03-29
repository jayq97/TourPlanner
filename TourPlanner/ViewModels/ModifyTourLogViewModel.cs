﻿using System.Windows.Input;
using TourPlanner.BusinessLayer.TourHandler;
using TourPlanner.Models;
using TourPlanner.Utils;
using TourPlanner.BusinessLayer.DictionaryHandler;
using System.Windows;

namespace TourPlanner.ViewModels
{
    public class ModifyTourLogViewModel : BaseViewModel
    {
        private string currentTourName = string.Empty;
        public string CurrentTourName
        {
            get
            {
                return currentTourName;
            }
            set
            {
                currentTourName = value;
                RaisePropertyChangedEvent(nameof(CurrentTourName));
            }
        }

        private TourLog currentTourLog;
        public TourLog CurrentTourLog
        {
            get
            {
                return currentTourLog;
            }
            set
            {
                currentTourLog = value;
                RaisePropertyChangedEvent(nameof(CurrentTourLog));
            }
        }
        public ICommand ModifyCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ModifyTourLogViewModel(MainViewModel mainViewModel, CurrentTourViewModel currentTourViewModel)
        {
            CurrentTourName = mainViewModel.CurrentTour.Name;
            CurrentTourLog = currentTourViewModel.CurrentTourLog;

            ModifyCommand = new RelayCommand(_ => {
                CurrentTourLog.Difficulty = mainViewModel.TourDictionary.ChangeDifficultyToPassBL(CurrentTourLog.Difficulty);
                TourLog tourLog = new TourLog(CurrentTourLog.Id, CurrentTourLog.Datetime, CurrentTourLog.Comment, CurrentTourLog.Difficulty, CurrentTourLog.TotalTime, CurrentTourLog.Rating);
                mainViewModel.TourHandler.ModifyTourLog(tourLog);
                mainViewModel.SelectedViewModel = new CurrentTourViewModel(mainViewModel);
                MessageBox.Show(
                    mainViewModel.TourDictionary.GetResourceFromDictionary("StringTourLogModified"),
                    mainViewModel.TourDictionary.GetResourceFromDictionary("StringTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            });

            CancelCommand = new RelayCommand(_ => {
                mainViewModel.SelectedViewModel = new CurrentTourViewModel(mainViewModel);
            });
        }
    }
}
