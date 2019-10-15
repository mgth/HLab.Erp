using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Notify;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.Update
{
    public class UpdaterWpf : NotifierObject, IUpdater
    {
        [Import]
        private IApplicationInfoService _info;


        public String Message
        {
            get => N.Get<string>();
            set => N.Set(value);
        }

        public String FileName
        {
            get => N.Get<string>();
            set => N.Set(value);
        }
        // http://www.chmp.org/sites/default/files/apps/sampling/
        public String Url
        {
            get => N.Get<string>();
            set => N.Set(value);
        }
        public Version NewVersion
        {
            get => N.Get<Version>();
            set => N.Set(value);
        }
        public double Progress
        {
            get => N.Get<double>();
            set => N.Set(value);
        }
        public bool Updated
        {
            get => N.Get<bool>(() => false);
            set => N.Set(value);
        }

        public void Update()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;

            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(Url + filename), path);
            });
            thread.Start();
        }
        private void RunUpdate()
        {
            var filename = FileName.Replace("{version}", NewVersion.ToString());
            var path = Path.GetTempPath() + filename;
            var startInfo = new ProcessStartInfo(path) { Verb = "runas" };
            try
            {
                Process.Start(startInfo);
                Updated = true;
            }
            catch (Win32Exception)
            {
                Message = "L'execution a échouée";
            }
            catch (WebException)
            {
                Message = "Le téléchargement a échoué";
            }
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            Progress = bytesIn / totalBytes * 100;
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            RunUpdate();
        }
        public void CheckVersion()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(Url + "version");

                request.Method = "GET";

                var response = request.GetResponse() as HttpWebResponse;

                var streamResponse = response?.GetResponseStream();
                if (streamResponse == null) return;

                var streamRead = new StreamReader(streamResponse);

                var version = Version.Parse(streamRead.ReadToEnd());

                streamResponse.Close();
                streamRead.Close();
                response.Close();

                NewVersion = version;
            }
            catch (UriFormatException e)
            {
                Message = e.Message;
            }
            catch (WebException e)
            {
                Message = e.Message;
            }
            catch (ArgumentException e)
            {
                Message = e.Message;
            }
        }
        [TriggerOn(nameof(NewVersion))]
        public bool NewVersionFound => N.Get(() => NewVersion > CurrentVersion);
        public Version CurrentVersion => N.Get(() => _info.Version);
    }

    public class ApplicationUpdateViewModel : ViewModel<ApplicationUpdateViewModel,UpdaterWpf>
    {
        public void Show()
        {
            var view = new ApplicationUpdateView
            {
                DataContext = this
            };
            // TODO : view.ShowDialog();
        }


        public ICommand UpdateCommand => _updateCommand.Get();
        private IProperty<ICommand> _updateCommand = H.Property<ICommand>(c => c
            .On(e => e.Model.NewVersionFound)
            .Command(e => e.Model.Update(),e=>e.Model.NewVersionFound)
        );
    }
}
