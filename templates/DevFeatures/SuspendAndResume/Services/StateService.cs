﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Storage;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Activation;

namespace ItemNamespace.Services
{
    /// <summary>
    /// This class exposes methods and events to save and restore the application's state. 
    /// For more information regarding the application lifecycle and how to handle suspend and resume, please see: 
    /// https://docs.microsoft.com/windows/uwp/launch-resume/app-lifecycle
    /// </summary>
    public class StateService
    {
        private static readonly Lazy<StateService> stateService = new Lazy<StateService>(() => new StateService());

        public static StateService Instance { get{ return stateService.Value; } }

        private const string stateFilename = "pageState.json";

        /// <summary>
        /// Saves the application state (current page and it's data) to local folder
        /// </summary>
        /// <returns></returns>
        internal async Task SaveStateAsync()
        {
            var pageState = new Dictionary<String, Object>();
            Type page = SaveState?.Target.GetType();
            var saveStateArgs = new SaveStateEventArgs(pageState, page);

            SaveState?.Invoke(this, saveStateArgs);
            await SettingsStorageService.SaveAsync<SaveStateEventArgs>(stateFilename, saveStateArgs, Windows.Storage.ApplicationData.Current.LocalFolder);
        }

        /// <summary>
        /// Restores the application state from local folder, navigates to stored page and restores it's data
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        internal async Task RestoreStateAsync(ApplicationExecutionState previousState, string arguments)
        {
            if (previousState == ApplicationExecutionState.Terminated)
            {
                var saveState = await SettingsStorageService.ReadAsync<SaveStateEventArgs>(stateFilename, Windows.Storage.ApplicationData.Current.LocalFolder);

                if (saveState != null && saveState.Page != null)
                {
                    NavigateToPage(saveState.Page, arguments);

                    //Restore page state
                    RestoreState?.Invoke(this, new RestoreStateEventArgs(saveState.PageState));
                }  
            }
        }

        private void NavigateToPage(Type page, string arguments)
        {
            NavigationService.Navigate(page, arguments);
        }

        //TODO UWPTEMPLATES: Subscribe to this event in pages in OnNavigatedTo event handler
        //to save page data, unsubscribe in OnNavigatedFrom

        public event SaveStateEventHandler SaveState;
        
        public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);


        //TODO UWPTEMPLATES: Subscribe to this event in pages in OnNavigatedTo event handler
        //to restore page data, unsubscribe in OnNavigatedFrom

        public event RestoreStateEventHandler RestoreState;

        public delegate void RestoreStateEventHandler(object sender, RestoreStateEventArgs e);

        public class RestoreStateEventArgs : EventArgs
        {
           
            public Object PageState { get; private set; }

            public RestoreStateEventArgs(Object pageState)
                : base()
            {
                this.PageState = pageState;
            }
        }

        public class SaveStateEventArgs : EventArgs
        {
            public Object PageState { get; set; }

            public Type Page { get; private set; }

            public SaveStateEventArgs(Object pageState, Type page)
                : base()
            {
                this.PageState = pageState;
                this.Page = page;
            }
        }        
    }
}
