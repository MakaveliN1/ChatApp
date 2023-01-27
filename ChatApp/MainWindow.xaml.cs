using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ChatApp
{
   
    public partial class MainWindow : Window
    {
        NetworkStream ns;
        Thread thread;
        TcpClient client;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private void ConnectButton_Click(object obj, RoutedEventArgs e)
        {
            if (client != null && client.Connected)
            {
                try
                {
                    // Disconnect logic
                    client.Client.Shutdown(SocketShutdown.Send);
                    thread.Join();
                    ns.Close();
                    client.Close();
                    ChatListBox.Items.Add("SYS: Disconnected from Server");
                    ConnectButton.Content = "Connect";
                    IPTextBox.IsEnabled = true;
                    PortTextBox.IsEnabled = true;
                }
                catch (Exception)
                {
                    ChatListBox.Items.Add("SYS: You are already Disconnected from Server");
                }
            }
            
            else
            {
                try
                {

                    IPAddress ip = IPAddress.Parse(IPTextBox.Text);
                    int port = Int32.Parse(PortTextBox.Text);
                    client = new TcpClient();
                    client.Connect(ip, port);
                    ChatListBox.Items.Add("SYS: Connected to Server");
                    ConnectButton.Content = "Disconnect";
                    IPTextBox.IsEnabled = false;
                    PortTextBox.IsEnabled = false;

                    ns = client.GetStream();
                    thread = new Thread(o => ReceiveData((TcpClient)o));

                    thread.Start(client);
                }
                catch (Exception)
                {
                    ChatListBox.Items.Add("SYS: Connection error, Incorrect IP:Port Combination or Server not running");
                }
            }

        }

        private void SendButton_Click(object obj, RoutedEventArgs e)
        {
            if (client == null)
            {
                ChatListBox.Items.Add("SYS: Please Connect before Sending Messages");
            }
            else if (!client.Connected)
            {
                ChatListBox.Items.Add("SYS: Not Connected to Server");
            }
            else
            {
                string messageToSend = UsernameTextBox.Text + ": " + MessageTextBox.Text;
                byte[] buffer = Encoding.ASCII.GetBytes(messageToSend);
                ns.Write(buffer, 0, buffer.Length);
                MessageTextBox.Text = "";
            }
        }

        private void ReceiveData(TcpClient client)
        {
            try
            {
                NetworkStream ns = client.GetStream();
                byte[] receivedBytes = new byte[1024];
                int byte_count;

                while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ChatListBox.Items.Add(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
                    });
                }
            }
            catch (Exception e)
            {
                ConnectButton.Dispatcher.Invoke(() => ConnectButton.Content = "Connect");

                ChatListBox.Dispatcher.Invoke(() => ChatListBox.Items.Add("SYS: Error Receiving Data, Incorrect IP:Port Combination or Server not running"));
            }
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            ns.Close();
            client.Close();
        }
        private void MessageTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
                if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private void MessageTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text == "Message")
            {
                MessageTextBox.Text = "";
            }
        }
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
            {
                UsernameTextBox.Text = "";
            }
        }

        
    }
}


    

