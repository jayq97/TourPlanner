﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BusinessLayer.TourHandler;
using TourPlanner.Models;
using TourPlanner.Utils;
using TourPlanner.BusinessLayer.DictionaryHandler;
using TourPlanner.BusinessLayer.TourAttributes;
using TourPlanner.BusinessLayer;
using TourPlanner.BusinessLayer.APIRequest;
using TourPlanner.BusinessLayer.Exceptions;

namespace TourPlanner.ViewModels
{
    public class AddTourViewModel : BaseViewModel
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if ((name != value))
                {
                    name = value;
                    RaisePropertyChangedEvent(nameof(Name));
                }
            }
        }

        private string start = string.Empty;
        public string Start
        {
            get { return start; }
            set
            {
                if ((start != value))
                {
                    start = value;
                    RaisePropertyChangedEvent(nameof(Start));
                }
            }
        }

        private string destination = string.Empty;
        public string Destination
        {
            get { return destination; }
            set
            {
                if ((destination != value))
                {
                    destination = value;
                    RaisePropertyChangedEvent(nameof(Destination));
                }
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set
            {
                if ((description != value))
                {
                    description = value;
                    RaisePropertyChangedEvent(nameof(Description));
                }
            }
        }

        private string transportType = string.Empty;
        public string TransportType
        {
            get { return transportType; }
            set
            {
                if ((transportType != value))
                {
                    transportType = value;
                    RaisePropertyChangedEvent(nameof(TransportType));
                }
            }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
                RaisePropertyChangedEvent(nameof(ErrorText));
            }
        }

        private bool available;
        public bool Available
        {
            get
            {
                return available;
            }
            set
            {
                available = value;
                RaisePropertyChangedEvent(nameof(Available));
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AddTourViewModel(MainViewModel mainViewModel)
        {
            TransportType = mainViewModel.TourDictionary.GetResourceFromDictionary("StringTourCar");
            Available = true;

            AddCommand = new RelayCommand(async _ =>
            {
                Available = false;
                if (string.IsNullOrEmpty(Start) || string.IsNullOrEmpty(Destination))
                {
                    ErrorText = mainViewModel.TourDictionary.GetResourceFromDictionary("StringErrorNotFilled");
                    Available = true;
                    return;
                }

                TransportType = mainViewModel.TourDictionary.ChangeTransportTypeToPassBL(TransportType);

                try
                {
                    TourAPIData tourAPIdata = await MapquestAPIRequest.RequestDirection(Start, Destination, TransportType);
                    int statusCode = tourAPIdata.StatusCode;
                    List<object> messages = tourAPIdata.Message;
                    double distance = tourAPIdata.Distance;
                    TimeSpan time = TimeSpan.FromSeconds(tourAPIdata.Time);

                    Tour newTour = new Tour(Name, Description, Start, Destination, TransportType, distance, time);
                    mainViewModel.TourHandler.AddNewTour(newTour);
                    mainViewModel.RefreshTourList(mainViewModel.TourHandler.GetTours());
                    mainViewModel.SelectedViewModel = new WelcomeViewModel(mainViewModel);

                    MessageBox.Show(
                        mainViewModel.TourDictionary.GetResourceFromDictionary("StringTourAdded"),
                        mainViewModel.TourDictionary.GetResourceFromDictionary("StringTitle"), 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                } 
                catch (MapquestAPIErrorException ex)
                {
                    mainViewModel.Log4NetLogger.Error(ex.Message);
                    ErrorText = $"{nameof(MapquestAPIErrorException)}: {ex.Message} {Environment.NewLine}" +
                    $"Response-Code: {ex.ErrorCode} {Environment.NewLine}" +
                    $"Response-Message: {ex.ErrorMessage}";
                }
                catch (MapquestAPIInvalidValuesException ex)
                {
                    mainViewModel.Log4NetLogger.Error(ex.Message);
                    ErrorText = $"{nameof(MapquestAPIInvalidValuesException)}: {ex.Message} {Environment.NewLine}" +
                    $"Values: Distance({ex.InvalidDistance}), Time({TimeSpan.FromSeconds(ex.InvalidTime)} {Environment.NewLine})" +
                    $"{mainViewModel.TourDictionary.GetResourceFromDictionary("StringErrorInvalidValuesResponse")}";
                }
                catch (TourAlreadyExistsException ex)
                {
                    mainViewModel.Log4NetLogger.Error(ex.Message);
                    ErrorText = $"{mainViewModel.TourDictionary.GetResourceFromDictionary("StringErrorTourNameAlreadyExists")}";
                }
                Available = true;
            });

            CancelCommand = new RelayCommand(_ => {
                mainViewModel.SelectedViewModel = new WelcomeViewModel(mainViewModel);
            });
        }
    }
}
