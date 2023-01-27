
using System.Net.Sockets;
using System;
using System.Windows;
using System.Threading;
using System.IO;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clientSocket.Connect(IPTextBox.Text, Int32.Parse(PortTextBox.Text));
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Message from Client$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                // Start the listener thread
                Thread listenerThread = new Thread(new ThreadStart(startListening));
                listenerThread.Start();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Could not connect to the server. Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(MessageTextBox.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Could not send message. Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void startListening()
        {
            try
            {
                while (true)
                {
                    // check if the application is still open before updating the GUI
                    if (this.IsLoaded)
                    {
                        NetworkStream serverStream = clientSocket.GetStream();
                        byte[] inStream = new byte[10025];
                        serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                        string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                        ChatListBox.Dispatcher.BeginInvoke(new Action(() => ChatListBox.Items.Add(returndata)));
                    }
                    else
                    {
                        // Stop listening if the application is closed
                        break;
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error receiving message. Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
    

