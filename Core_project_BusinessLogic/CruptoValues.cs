namespace Core_project_BusinessLogic
{
    public class EncryptionSettings : IDisposable
    {
        public string Key { get; set; }
        public string IV { get; set; }

        // public EncryptionSettings()
        // {
        //     Key = "d7k0hlZM29w9X5RUH18uy4o6F6F4bPB4rVL2Bt6Ief0=";
        //     IV = "nlQBLWvHIyGoDsjJEhPeqg==";
        // }
        //string connection = "user id=sa;data source=softa;persist security info=True;initial catalog=tic_coreproject_2025;password=SqL@2017;Encrypt=True;TrustServerCertificate=True";
         public EncryptionSettings()
        {
            Key = "KmWx9kZicbf/e3ryNRKiBEjHZ5wN5/TjQDJtEOf/v9c=";
            IV = "3etx9dQd2tMOOiVgOHwmjA==";
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EncryptionSettings()
        {
            // Simply call Dispose(False).
            Dispose(false);
        }

    }
}