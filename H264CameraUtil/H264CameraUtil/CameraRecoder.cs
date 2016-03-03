using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace H264CameraUtil
{
    class ChapterInfo
    {
        public String m_VideoID;
        public DateTime m_TimeStamp;
        public override string ToString()
        {
            return "VideoID: " + m_VideoID;
        }
    }

    class CameraRecorder : CameraClient
    {
        private String m_FolderName;
        private String m_Prefix;
        private String m_Extention;
        private SreviceStatus m_SreviceStatus;
        private ChapterInfo m_ChapterInfo;
        
        public CameraRecorder(CameraRecorder other): base(other.m_CameraParams) 
        {
            
            m_Prefix = other.m_Prefix;
            m_Extention = other.m_Extention ;
            m_SreviceStatus = SreviceStatus.SERVICE_OFF;
            
        }

        public CameraRecorder(CameraParam cameraParams, String folderName, String prefix, String ext)
            : base(cameraParams) 
        {
            m_FolderName = folderName;
            m_Prefix = prefix;
            m_Extention = ext;
            m_SreviceStatus = SreviceStatus.SERVICE_OFF;
            
        }
        public override void StartService()
        {

            if (m_SreviceStatus == SreviceStatus.SERVICE_ON)
            {
                Logger.Error("CameraRecorder - Logic Error: Trying to turn on a running Srevice.");
            }
            try
            {
                m_ChapterInfo = CreateChapterInfo();
                String op = ":sout=#file{dst=" + m_ChapterInfo.m_VideoID + ",no-overwrite}";
                Logger.Info("Create File :" + m_ChapterInfo.m_VideoID);
                AddVlcOption(op);
                StartCapture();
                m_SreviceStatus = SreviceStatus.SERVICE_ON;
            }
            catch(Exception e)
            {
                Logger.Error("CameraRecorder - failed to Start Camera:" + m_CameraParams.ToString() + " recorder Service: " + e.ToString());
            }

            Logger.Info("CameraRecorder -Start Service on:" + m_CameraParams.ToString());
            
        }

        public void createNewChapter()
        {
            if (m_SreviceStatus == SreviceStatus.SERVICE_OFF)
            {
                Logger.Error("CameraRecorder - Logic Error: Trying to create new Chapter while Service is off!");
                throw new System.InvalidOperationException("CameraRecorder - Logic Error: Trying to create new Chapter while Service is off!");
            }
            
            StopService();
            StartService();
            Logger.Info("CameraRecorder - Start New Chapter:" + m_CameraParams.ToString());
        }
        public ChapterInfo GetCurrentChapterInfo()
        {
            if (m_SreviceStatus == SreviceStatus.SERVICE_OFF)
            {
                Logger.Error("CameraRecorder - Logic Error: Trying to get current Chapter Information while Service is off!");
                throw new System.InvalidOperationException("CameraRecorder - Logic Error: Trying to get current Chapter Information while Service is off!");
            }

            return m_ChapterInfo;
        }

        public ChapterInfo CreateChapterInfo()
        {
           
            ChapterInfo chapterInfo = new ChapterInfo();
            chapterInfo.m_TimeStamp = DateTime.Now;

            String time = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", chapterInfo.m_TimeStamp);
            chapterInfo.m_VideoID = String.Format("{0}\\{1}_{2}.{3}", m_FolderName, m_Prefix, time, m_Extention);

            return chapterInfo;
            
        }
        public override void StopService()
        {
            
            try
            {
                StopCapture();
                m_SreviceStatus = SreviceStatus.SERVICE_OFF;
            }
            catch(Exception e)
            {
                Logger.Error("CameraRecorder - failed to Stop Camera:" + m_CameraParams.ToString() + " recorder Service: " + e.ToString());
            }
            Logger.Info("CameraRecorder - Stop Service on :" + m_CameraParams.ToString());
        }
    }
}
