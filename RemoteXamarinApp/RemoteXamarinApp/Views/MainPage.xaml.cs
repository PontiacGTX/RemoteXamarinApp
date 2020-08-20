
using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace RemoteXamarinApp
{
   
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.ContentPage
    {

       // PlayerPage playerPage;
        bool startedConnection = false;
        TcpClient client = new TcpClient();
        TcpListener listener;
        BinaryFormatter formatter = new BinaryFormatter();
        private System.Threading.Tasks.Task listening;
        private System.Threading.Tasks.Task GetImage;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationTokenSource tokenSource1 = new CancellationTokenSource();
        string _ip;
        int port;
        
        public MainPage()
        {
            InitializeComponent();

        }
 


        private  void  DisableMainControls()
        {
            this.lblTitle.IsVisible = false;
            this.lblTitle.IsEnabled = false;
            this.IPAddress.IsEnabled = false;
            this.IPAddress.IsVisible = false;
            this.Port.IsEnabled = false;
            this.Port.IsVisible = false;
            Connect.BackgroundColor =Xamarin.Forms.Color.Red;
            Connect.Text = "(Esperando por Host) Cancelar";

        }
        private void EnableMainControls()
        {

            this.lblTitle.IsVisible = true;
            this.lblTitle.IsEnabled = true;
            this.IPAddress.IsEnabled = true;
            this.IPAddress.IsVisible = true;
            
            this.Port.IsEnabled = true;
            this.Port.IsVisible = true;
            Connect.BackgroundColor = Xamarin.Forms.Color.Red;
            Connect.Text = "Conectar";

        }
        private void EnableVideoControl()
        {
            this.imageControl.IsEnabled = true;
            this.imageControl.IsVisible = true;
            this.IsVisible = true;
           
        }
        private void DisableVideoControl()
        {
            this.imageControl.IsEnabled = false;
            this.imageControl.IsVisible = false;
            this.IsVisible = false;
          
        }
        private async void NavigateButton_OnClicked(object sender, EventArgs e)
        {

            try
            {
                if (!startedConnection)
                {
                    
                    //connection = new ConnectionIPData();
                    //connection._Port = int.Parse(this.Port.Text);
                    //connection._IPAddress = this.IPAddress.Text;
                    //ModelIPDetails.details = connection;
                    //BindingContext = connection;

                    try
                    {
                        client = new TcpClient();
                        await client.ConnectAsync(this._ip = this.IPAddress.Text, this.port = int.Parse( this.Port.Text));

                        if (client.Connected)
                            GetImage = new Task(ReceiveImage, tokenSource.Token);
                        startedConnection = true;
                        DisableMainControls();
                        EnableVideoControl();
                        GetImage.Start();

                    }
                    catch (Exception ex)
                    {
                       await DisplayAlert($"Encontro un problema", ex.ToString(), "Ok");
                    }
                    

                }
                else
                {
                    StopListening();
                    DisableVideoControl();
                    EnableMainControls();
                }
                //await Navigation.PushAsync(playerPage ?? (playerPage = new PlayerPage(this.IPAddress.Text, int.Parse(this.Port.Text))));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "" + ex.ToString(), "Ok");
            }
        }


       
        


        private Task<ImageSource> GetImageAsync(System.IO.Stream stream)
        {
            TaskCompletionSource<ImageSource> tcs = new TaskCompletionSource<ImageSource>();
            var received =   tcs.TrySetResult(ImageSource.FromStream(() => (MemoryStream)formatter.Deserialize(client.GetStream())));
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

                         imageControl.Source = await GetImageAsync(client.GetStream()); 

                       
                    }));
                }
            }
            catch (Exception ex)
            {

                StopListening();
            }
        }
        
        private void StopListening()
        {
         
            try
            {
                listener.Stop();
                client.Dispose();
                client.Close();
                if (listening.Status != TaskStatus.RanToCompletion)
                    tokenSource.Cancel();
                if (GetImage.Status != TaskStatus.RanToCompletion)
                    tokenSource1.Cancel();
            }
            catch (Exception ex)
            {
                DisplayAlert($"Encontro un problema", ex.ToString(), "Ok");
            }
        }

        
    }
}
