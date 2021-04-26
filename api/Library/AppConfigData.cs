namespace Library
{
    /// <summary>
    /// Object containing setting values from appsettings.json
    /// </summary>
    public class AppConfigData
    {
        public string ConnectionString { get; set; }

        //to use more settings first add a key-value pair in appsettings.json "ConfigData" section then add the coresponding property here
    }
}
