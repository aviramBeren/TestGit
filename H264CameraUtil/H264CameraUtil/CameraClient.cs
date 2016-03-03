using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Forms;

namespace H264CameraUtil
{
   abstract class CameraClient
    {
        //members
        protected CameraParam m_CameraParams;
        private VlcControl m_vlcControl;
        private String m_vlcOption;

        //methods
        public CameraClient(CameraParam cameraParams)
        {
            //initialize local camera params
            m_CameraParams = cameraParams;
            
            //initialize static vlc component 
            if (VlcContext.IsInitialized == false)
                VlcHelper.Initialize();

        }
        protected void AddVlcOption(String vlcOption)
        {
            if (!vlcOption.Equals(""))
            {
                var media = new Vlc.DotNet.Core.Medias.LocationMedia(m_CameraParams.BuildFullAddress());
                media.AddOption(vlcOption);
                m_vlcControl = new VlcControl();
                m_vlcControl.Media = media;
            }
        }
       
        protected void StartCapture()
        {
            m_vlcControl.Play();
        }
        protected void StopCapture()
        {
            m_vlcControl.Stop();
        }
        
       abstract public void StartService();
       abstract public void StopService();
    }
}
