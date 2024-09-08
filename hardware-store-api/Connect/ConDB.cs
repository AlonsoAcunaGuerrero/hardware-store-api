namespace hardware_store_api.Connect
{
    public class ConDB
    {
        private static string conectionData;

        private ConDB()
        {
            
        }

        public static string getConnection()
        {
            if(conectionData == null)
            {
                conectionData = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            }

            return conectionData;
        }
    }
}
