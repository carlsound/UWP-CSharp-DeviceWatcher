using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_CSharp_DeviceWatcher
{
    class MainPageViewModel
    {
        private Windows.UI.Core.CoreDispatcher m_coreDispatcher;
        private AudioDeviceWatcher m_audioDeviceWatcher;
        public MainPageViewModel(Windows.UI.Core.CoreDispatcher dispatcher)
        {

        }

        ~MainPageViewModel()
        {

        }

        protected void OnNavigateTo(Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {

        }

        protected AudioDeviceWatcher GetAudioInputs()
        {
            return 0;
        }

        private void CollectionChanged()
        {

        }
    }
}
