using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Concurrent;


namespace H264CameraUtil
{
   
    class Program
    {
        static CameraParams CameraParams;

        static private Object thisLock = new Object();

        public static void CreateNewChapters(ref List<CameraRecorder> currentCameraRecorders)
        {
            List<CameraRecorder> newCameraRecorders = new List<CameraRecorder>();

            InitializeRecorders(newCameraRecorders);

            #region FOR EACH CAMERA PARAM START RECORDING
            Parallel.ForEach(newCameraRecorders, (cameraRecorder) =>
            {
                cameraRecorder.StartService();
            });

            #endregion
            List<CameraRecorder> oldCameraRecorders;
            Thread.Sleep(1000 * H264CameraUtil.Properties.Settings.Default.RECORDER_OVERLAP_DURATION / 2);
            lock (thisLock)
            {
                oldCameraRecorders = currentCameraRecorders;
                currentCameraRecorders = newCameraRecorders;
            }
            Thread.Sleep(1000 * H264CameraUtil.Properties.Settings.Default.RECORDER_OVERLAP_DURATION / 2);
            #region FOR EACH CAMERA PARAM STOP RECORDING
            Parallel.ForEach(oldCameraRecorders, (cameraRecorder) =>
            {
                cameraRecorder.StopService();
            });
            #endregion

            currentCameraRecorders = newCameraRecorders;
        }

        public static void KeyListener(List<CameraRecorder> cameraRecorders, List<CameraStream> cameraStreamers)
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            Thread.Sleep(1000 * H264CameraUtil.Properties.Settings.Default.KEY_LISTENER_DELAY);
            Console.Clear();
            Console.WriteLine("press the 'x' key to quit.");
            Console.WriteLine("\npress the 'SpaceBar ' to create a Bookmark.");
            Console.WriteLine("\npress the 'c ' to create a new Chapter.");
            Console.WriteLine("\npress the 'o' to create a X seconds overlap new Chapters.");
            Console.WriteLine("**********************************************************************");
            do
            {

                while (Console.KeyAvailable == false)
                    Thread.Sleep(250); // Loop until input is entered.
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Spacebar)
                {
                                     
                    //The TimeStamp off the BookMark
                    DateTime BookMarkTimeStamp = DateTime.Now;

                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Create A New BookMark On The Following Videos:", cki.Key);
                    ConcurrentBag<ChapterInfo> chapterInfos = new ConcurrentBag<ChapterInfo>();
                    ParallelLoopResult result = Parallel.ForEach(cameraRecorders, cameraRecorder =>
                    {
                        chapterInfos.Add(cameraRecorder.GetCurrentChapterInfo());
                    });


                    foreach(var ci in chapterInfos )
                    {
                        TimeSpan span = BookMarkTimeStamp - ci.m_TimeStamp;
                        Console.WriteLine("     " + ci.ToString() + " with an offset of:" + (int)span.TotalMilliseconds + "ms");
                    }
                    Console.WriteLine("----------------------------------------------------------------------");    
                }
                if (cki.Key == ConsoleKey.C)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Create A new Chapter", cki.Key);
                    Parallel.ForEach(cameraRecorders, cameraRecorder =>
                    {
                       cameraRecorder.createNewChapter();
                    });

                    Console.WriteLine("----------------------------------------------------------------------");
                }
                if (cki.Key == ConsoleKey.O)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Create A new OverLaps Chapters", cki.Key);
                    Thread thread = new Thread(() => CreateNewChapters(ref cameraRecorders));
                    thread.Start();
                    Console.WriteLine("----------------------------------------------------------------------");
                }
                


            } while (cki.Key != ConsoleKey.X);
        }

        private static void ReadCameraParamters()
        {
            #region HOW_TO_CREATE_SERIALIZATION_XML BY DEFAULT DISABLED
            /*
            CameraParam cp1 = new CameraParam("192.168.1.152", 554, "stream0", 0);
            CameraParams cameraParams = new CameraParams();
            cameraParams.m_cameraParams = new CameraParam[1];
            cameraParams.m_cameraParams[0] = cp1;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(cameraParams.GetType());
            TextWriter writer = new StreamWriter(@"c:\log\cameraParams.xml");
            x.Serialize(writer, cameraParams);
            Console.WriteLine();
            Console.ReadLine();
            */
            #endregion

            #region READ CAMERA PARAMS FROM THE CAMERA_PARAMS_FILE_NAME

            // Construct an instance of the XmlSerializer with the type of object that is being de-serialized.
            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(CameraParams));
            // To read the file, create a FileStream.
            FileStream myFileStream = new FileStream(H264CameraUtil.Properties.Settings.Default.CAMERA_PARAMS_FILE_NAME, FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            CameraParams = (CameraParams)mySerializer.Deserialize(myFileStream);

            #endregion
        }

        private static void InitializeRecorders(List<CameraRecorder> cameraRecorders)
        {

            #region FOR EACH CAMERA PARAM CREATE RECORDER
            for (int i = 0; i < CameraParams.m_cameraParams.Length; i++)
            {
                cameraRecorders.Add(new CameraRecorder(CameraParams.m_cameraParams[i],
                                                H264CameraUtil.Properties.Settings.Default.CAMERA_RECORDER_OUTPUT_FOLDER,
                                                H264CameraUtil.Properties.Settings.Default.CAMERA_RECORDER_OUTPUT_PREFIX + i.ToString(),
                                                H264CameraUtil.Properties.Settings.Default.CAMERA_RECORDER_OUTPUT_EXT));
            }
            #endregion
        }

        private static void InitializeStreamers( List<CameraStream> cameraStreamers)
        {


            #region FOR EACH CAMERA PARAM CREATE STREAMER
            for (int i = 0; i < CameraParams.m_cameraParams.Length; i++)
            {
                cameraStreamers.Add(new CameraStream(CameraParams.m_cameraParams[i],
                                                H264CameraUtil.Properties.Settings.Default.CAMERA_STREAMER_PORT,
                                                H264CameraUtil.Properties.Settings.Default.CAMERA_STREAMER_NAME + i.ToString()));
            }
            #endregion
        }

        private static void InitializeViewers(List<CameraViewer> cameraViewers)
        {


            #region FOR EACH CAMERA PARAM CREATE VIEWER
            for (int i = 0; i < CameraParams.m_cameraParams.Length; i++)
            {
                cameraViewers.Add(new CameraViewer(CameraParams.m_cameraParams[i]));
            }
            #endregion
        }

        static void Main(string[] args)
        {
            
            ReadCameraParamters();

            #region FOR EACH CAMERA PARAM START RECORDING
            List<CameraRecorder> cameraRecorders = new List<CameraRecorder>();
            if(H264CameraUtil.Properties.Settings.Default.RUN_RECORDER_SERVICE)
            {
                
                InitializeRecorders(cameraRecorders);
                Parallel.ForEach(cameraRecorders, (cameraRecorder) =>
                {
                    cameraRecorder.StartService();
                });
            }
           
            #endregion
            
            #region FOR EACH CAMERA PARAM START STREMING
            List<CameraStream> cameraStreamers = new List<CameraStream>();
            if (H264CameraUtil.Properties.Settings.Default.RUN_STREAMER_SERVICE)
            {
                InitializeStreamers(cameraStreamers);
                Parallel.ForEach(cameraStreamers, (cameraStreamer) =>
                {
                    cameraStreamer.StartService();
                });
            }
            #endregion
             
           
            #region FOR EACH CAMERA PARAM START VIEWERS
            List<CameraViewer> cameraViewers = new List<CameraViewer>();
            if (H264CameraUtil.Properties.Settings.Default.RUN_VIEWER_SERVICE)
            {
                InitializeViewers(cameraViewers);
                Parallel.ForEach(cameraViewers, (cameraViewer) =>
                {
                    cameraViewer.StartService();
                });
            }
            #endregion
           
            KeyListener(cameraRecorders, cameraStreamers);
            
            #region FOR EACH CAMERA PARAM STOP RECORDING
            Parallel.ForEach(cameraRecorders, (cameraRecorder) =>
            {
                cameraRecorder.StopService();
            });
            #endregion
            #region FOR EACH CAMERA PARAM STOP STREMING
            Parallel.ForEach(cameraStreamers, (cameraStreamer) =>
            {
                cameraStreamer.StopService();
            });
            #endregion           
            #region FOR EACH CAMERA PARAM STOP VIEWERS
            Parallel.ForEach(cameraViewers, (cameraViewer) =>
            {
                cameraViewer.StopService();
            });
            #endregion
             
        }
    }
}
