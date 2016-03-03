using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H264CameraUtil
{
    class CameraViewer : CameraClient
    {
        static String option = ":sout=#display";
        private CameraParam cp;
        public CameraViewer(CameraParam cameraParams)
            : base(cameraParams)
        {
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
