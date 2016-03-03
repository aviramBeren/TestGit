using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace H264CameraUtil
{
    class CameraPortForwarding : CameraClient
    {
        
        private CameraParam cp;
        private int m_localPort;
        public CameraPortForwarding(CameraParam cameraParams,int localPort) : base(cameraParams) 
        {
            m_localPort = localPort;
        }
        public override void StartService()
        {
            // Get host name
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // Enumerate IP addresses
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                Task.Factory.StartNew(() =>
                {

                    IPAddress localIpAddress = IPAddress.Parse(ipaddress.ToString());
                    try
                    {
                        RouterServices myPortForworder = new RouterServices();
                        myPortForworder.Start(new IPEndPoint(localIpAddress, m_localPort),
                            new IPEndPoint(IPAddress.Parse(m_CameraParams.m_IpAddress), m_CameraParams.m_PortNumber));

                    }
                    catch (Exception e)
                    {
                        //Logger.Error(e);
                    }
                });
            }
        }
        public override void StopService()
        {
           
        }
    
    }
     public class RouterServices
    {
        //uses
        //new TcpForwarderSlim().Start(
        //new IPEndPoint(source_IP, source_PORT),
        //new IPEndPoint(Destenetion_IP, Destenetion_PORT));
        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public void Start(IPEndPoint local, IPEndPoint remote)
        {
            _mainSocket.Bind(local);
            _mainSocket.Listen(10);

            while (true)
            {
                var source = _mainSocket.Accept();
                var destination = new RouterServices();
                var state = new State(source, destination._mainSocket);
                destination.Connect(remote, source);
                source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
            }
        }

        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var state = new State(_mainSocket, destination);
            try
            {
                _mainSocket.Connect(remoteEndpoint);

                _mainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
            }
            catch (Exception e)
            {
                //Logger.Error(e);
            }
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead <= 0) return;
                state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
            }
            catch
            {
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private class State
        {
            public Socket SourceSocket { get; private set; }
            public Socket DestinationSocket { get; private set; }
            public byte[] Buffer { get; private set; }

            public State(Socket source, Socket destination)
            {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[8192];
            }
        } 
    }
}
