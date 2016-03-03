using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H264CameraUtil
{
    class CameraStream: CameraClient
    {
        static String option = ":sout=#rtp{sdp=rtsp://:8554/test}";
        private CameraParam cp;
        public CameraStream(CameraParam cameraParams,String port, String streamName)
            : base(cameraParams) 
        {
           String  option =  ":sout=#rtp{sdp=rtsp://:" + port + "/" + streamName +"}";
           Logger.Info("Start Stream to:" + @"rtsp://:" + port + "/" + streamName);
           AddVlcOption(option);
        }
        public override void StartService()
        {
            StartCapture();
        }
        public override void StopService()
        {
            StopCapture();

        }
    }
}
