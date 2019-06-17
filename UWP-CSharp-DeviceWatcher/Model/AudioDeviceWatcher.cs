using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// Needed for the DeviceWatcherTrigger
using Windows.ApplicationModel.Background;

// https://github.com/microsoft/Windows-universal-samples/tree/master/Samples/DeviceEnumerationAndPairing

namespace UWP_CSharp_DeviceWatcher
{
    enum AudioDeviceType {Input, Output };
    class AudioDeviceWatcher
    {
        private Windows.Devices.Enumeration.DeviceInformationCollection m_deviceInformationCollection;
        private Windows.Devices.Enumeration.DeviceWatcher m_deviceWatcher;
        private Windows.UI.Core.CoreDispatcher m_coreDispatcher;
        private AudioDeviceType m_deviceType;
        private Windows.Devices.Enumeration.DeviceClass m_deviceClass;
        private Windows.UI.Xaml.Data.Binding m_uiBinding;
        private Windows.ApplicationModel.Background.DeviceWatcherTrigger m_deviceWatcherTrigger;

        private string backgroundTaskName = "DeviceEnumerationCs_BackgroundTaskName";
        private Windows.ApplicationModel.Background.IBackgroundTaskRegistration backgroundTaskRegistration = null;

        public AudioDeviceWatcher(AudioDeviceType ioType, Windows.UI.Core.CoreDispatcher dispatcher)
        {
            m_deviceType = ioType;
            m_coreDispatcher = dispatcher;
            StartWatcher();
        }

        ~AudioDeviceWatcher()
        {
            StopWatcher();
        }

        private void StartWatcher()
        {
            DeviceWatcherEventKind[] triggerEventKinds = { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Remove, DeviceWatcherEventKind.Update };

            switch (m_deviceType)
            {
                case AudioDeviceType.Input:
                {
                    m_deviceClass = DeviceClass.AudioRender;


                    break;
                }
                case AudioDeviceType.Output:
                {
                    m_deviceClass = DeviceClass.AudioRender;
                    break;
                }
            }
            m_deviceWatcher = DeviceInformation.CreateWatcher(m_deviceClass);

            // Get the background trigger for this watcher
            m_deviceWatcherTrigger = m_deviceWatcher.GetBackgroundTrigger(triggerEventKinds);

            // Register this trigger for our background task
            RegisterBackgroundTask(m_deviceWatcherTrigger);
        }

        private void RegisterBackgroundTask(DeviceWatcherTrigger deviceWatcherTrigger)
        {
            BackgroundTaskBuilder taskBuilder;

            // First see if we already have this background task registered. If so, unregister
            // it before we register it with the new trigger.

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (backgroundTaskName == task.Value.Name)
                {
                    UnregisterBackgroundTask(task.Value);
                }
            }

            taskBuilder = new BackgroundTaskBuilder();
            taskBuilder.Name = backgroundTaskName;
            taskBuilder.TaskEntryPoint = "BackgroundDeviceWatcherTaskCs.BackgroundDeviceWatcher";
            taskBuilder.SetTrigger(deviceWatcherTrigger);

            backgroundTaskRegistration = taskBuilder.Register();
            backgroundTaskRegistration.Completed += new BackgroundTaskCompletedEventHandler(OnTaskCompleted);
        }

        private async void OnTaskCompleted(BackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            await m_coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //rootPage.NotifyUser("Background task completed", NotifyType.StatusMessage);
            });
        }

        private void StopWatcher()
        {
            if (null != backgroundTaskRegistration)
            {
                UnregisterBackgroundTask(backgroundTaskRegistration);
            }
        }

        private void UnregisterBackgroundTask(IBackgroundTaskRegistration taskReg)
        {
            taskReg.Unregister(true);
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values["eventCount"] = (uint)0;
        }
    }
}
