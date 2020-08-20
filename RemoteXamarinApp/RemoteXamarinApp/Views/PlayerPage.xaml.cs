using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteXamarinApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerPage : ContentPage
    {

        TcpClient client = new TcpClient();
        TcpListener listener;
        BinaryFormatter formatter = new BinaryFormatter();
        private System.Threading.Tasks.Task listening;
        private System.Threading.Tasks.Task GetImage;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationTokenSource tokenSource1 = new CancellationTokenSource();
        string _ip;

        private int port;

        MainPage mainPage;

        public PlayerPage()
        {

        }
        public PlayerPage(string ip, int port)
        {

            InitializeComponent();

            try
            {
                client = new TcpClient();
                client.Connect(ip, this.port = port);

                if (client.Connected)
                    GetImage = new Task(ReceiveImage);

                GetImage.Start();

            }
            catch (Exception ex)
            {
                DisplayAlert($"Encontro un problema", ex.ToString(), "Ok");
            }


        }


        private Task<ImageSource> GetImageAsync(System.IO.Stream stream)
        {
            TaskCompletionSource<ImageSource> tcs = new TaskCompletionSource<ImageSource>();
            tcs.SetResult(ImageSource.FromStream(() => (MemoryStream)formatter.Deserialize(client.GetStream())));
            return tcs.Task;
        }

        private async void ReceiveImage()
        {
            try
            {

                while (client.Connected)
                {

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(async () =>
                    {

                        using (MemoryStream ms = new MemoryStream())
                        {
                            imageControl.Source = ImageSource.FromStream((() => ms));
                            ms?.Dispose();
                        }
                        imageControl.Source = await GetImageAsync(client.GetStream());

                    }));
                }
            }
            catch
            {
                StopListening();
            }
        }

       
        private void StopListening()
        {
            listener.Stop();
            client.Close();
            if (listening.Status ==TaskStatus.Running)
                tokenSource.Cancel();
            if (GetImage.Status==TaskStatus.Running)
                tokenSource1.Cancel();
        }

        
        private void Button_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushAsync(mainPage ?? (mainPage = new MainPage())).GetAwaiter().GetResult();
            Navigation.PopAsync();
           
        }
    }
}