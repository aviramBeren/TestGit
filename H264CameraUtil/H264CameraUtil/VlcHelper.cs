using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Forms;


namespace H264CameraUtil
{
    public enum VlcMode
    {       
        HEADSET_MICROPHONE,
        LOCAL,
        BLUETOOTH
    }
    public static class VlcHelper
    {
        //TODO:: remove absolute path
        static string _LibVlcDllsPath = "C:\\Program Files (x86)\\VideoLAN\\VLC\\";
        static string _LibVlcPluginsPath = "C:\\Program Files (x86)\\VideoLAN\\VLC\\plugins\\";
        static string _LibVlcDllsPathX64 = "C:\\Program Files\\VideoLAN\\VLC\\";
        static string _LibVlcPluginsPathX64 = "C:\\Program Files\\VideoLAN\\VLC\\plugins\\";
        

        public static void Initialize()
        {
            

            //Important!!!
            //Set libvlc.dll and libvlccore.dll directory path                
            if (System.IO.Directory.Exists(_LibVlcDllsPath) && System.IO.Directory.Exists(_LibVlcPluginsPath))
            {
                VlcContext.LibVlcDllsPath = _LibVlcDllsPath;
                VlcContext.LibVlcPluginsPath = _LibVlcPluginsPath;
            }
            else if (System.IO.Directory.Exists(_LibVlcDllsPathX64) && (System.IO.Directory.Exists(_LibVlcPluginsPathX64)))
            {
                VlcContext.LibVlcDllsPath = _LibVlcDllsPathX64;
                VlcContext.LibVlcPluginsPath = _LibVlcPluginsPathX64;
            }
            else
            {
                //TODO:: instead of throwing exception -> install VLC program.
                Logger.Error( "Error: Failed to find local VLC installation!");
            }
            //Set the vlc plugins directory path
            //Set the startup options
            VlcContext.StartupOptions.IgnoreConfig = true;
            VlcContext.StartupOptions.LogOptions.LogInFile = false;
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
            VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.None;
            VlcContext.StartupOptions.AddOption("--network-caching=300");
            VlcContext.Initialize();
            
        }
        public static void ChangeMode(VlcMode mode)
        {
            VlcContext.StartupOptions.IgnoreConfig = true;
            switch (mode)
            {
                case VlcMode.HEADSET_MICROPHONE:
                    {
                        VlcContext.StartupOptions.AddOption("--dshow-adev=Headset Microphone (2- USB Audi");
                        break;
                    }
                case VlcMode.LOCAL:
                    {
                        VlcContext.StartupOptions.AddOption("--dshow-adev=Microphone (High Definition Aud");
                        break;
                    }
                case VlcMode.BLUETOOTH:
                    {
                        VlcContext.StartupOptions.AddOption("--dshow-adev=Bluetooth Microphone (Bluetooth");
                        break;
                    }
            }
            VlcContext.StartupOptions.AddOption("--live-caching=1000");
            VlcContext.StartupOptions.AddOption("--dshow-vdev=none");
        }

    }
}