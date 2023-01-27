# Sockets based Chat-App in C#
A basic socket-based chat application written in C#. This application uses the System.Net and System.Net.Sockets namespaces to create a TcpClient and NetworkStream, which are used to connect to a server, send, and receive data over a network socket.

The MainWindow class of the ChatApp namespace connects to the server on button click and the Server class creates a listening socket on a specific IP address and port, waiting for incoming connections.

This application is meant to help users polish their basic socket-based communication skills and is nothing fancy.

# Usage
1. Clone the repository
2. Open the project in Visual Studio
3. Run the application
4. Click the Connect button to connect to the server
5. Start chatting with other connected clients
# Note
Make sure the server is running before connecting the clients.
You can change the IP address and port number in the Server class to match the server's configuration.
