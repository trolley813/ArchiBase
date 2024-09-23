namespace ArchiBase.Utils
{
    public static class CoordinateDisplayExtensions
    {
        public static string ToLatitudeDMS(this double latitude)
        {
            int degrees = (int)latitude;
            int minutes = (int)(latitude * 60) % 60;
            double seconds = latitude * 3600 % 60;
            char direction = latitude > 0 ? 'N' : 'S';
            return string.Format("{0:d1}° {1:d2}' {2:00.000}\" {3}", degrees, minutes, seconds, direction);
        }

        public static string ToLongitudeDMS(this double longitude)
        {
            int degrees = (int)longitude;
            int minutes = (int)(longitude * 60) % 60;
            double seconds = longitude * 3600 % 60;
            char direction = longitude > 0 ? 'E' : 'W';
            return string.Format("{0:d1}° {1:d2}' {2:00.000}\" {3}", degrees, minutes, seconds, direction);
        }
    }
}