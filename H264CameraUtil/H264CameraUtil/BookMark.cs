using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace H264CameraUtil
{

    class BookMark
    {
        static long IndexCounter = 0;

        BookMark( String officerUserName, String vehicleId, DateTime creationDate, GeoCoordinate location, String audioFileName, List<String> videoFileNames)
        {
            Id = IndexCounter++;
            String OfficerUserName = officerUserName;
            String VehicleId = vehicleId;
            DateTime CreationDate = creationDate;
            GeoCoordinate Location = location;
            String AudioFileName = audioFileName;
            List<String> VideoFileNames = videoFileNames;
        }
        
        public long Id { get; private set; }
        public String OfficerUserName { get; private set; }
        public String VehicleId { get; private set; }
        public DateTime CreationDate { get; private set; }
        public GeoCoordinate Location { get; private set; }
        public String AudioFileName { get; private set; }
        public List<String> VideoFileNames { get; private set; }
       
    }
}
