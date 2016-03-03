using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H264CameraUtil
{
    public class CameraParams
    {
        public CameraParam[] m_cameraParams;
    }

    [Serializable]
    public class CameraParam
    {
        #region members
        /// [PROTOCOL]://[IP_ADDRESS]:[PORT]/[PATH]
        /// example: rtsp://127.0.0.1:2554/stream_0
        public String m_IpAddress;
        public int m_PortNumber;
        public String m_Path;
        public StreamingProtocolType m_ProtocolType; 
        #endregion
        
        #region methods
        public CameraParam()
        {
            m_IpAddress = "127.0.0.1";
            m_PortNumber = 554;
            m_Path = "stream0";
            m_ProtocolType = StreamingProtocolType.RTSP;
        }
        public CameraParam(String ipAddress, int portNumber, String path, StreamingProtocolType protocolType)
        {
            m_IpAddress = ipAddress;
            m_PortNumber = portNumber;
            m_Path = path;
            m_ProtocolType = protocolType;
        }
        
        public override string ToString()
        {
            return "IpAddress: " + m_IpAddress + " Port#" + m_PortNumber + " Path:" + m_Path + " ProtocolType:" + m_ProtocolType;
        }
        //Build the full address of the camera
        //build the stream source address : [PROTOCOL]://[IP_ADDRESS]:[PORT]/[PATH]


        public String BuildFullAddress()
        {
            // [PROTOCOL]://[IP_ADDRESS]:[PORT]/[PATH]
            

            return String.Format("{0}://{1}:{2}/{3}", m_ProtocolType, m_IpAddress, m_PortNumber, m_Path);
        }

        #endregion
    }
}
