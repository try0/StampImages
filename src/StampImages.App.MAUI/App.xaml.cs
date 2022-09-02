namespace StampImages.App.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            //window.Destroying += OnWindowDestroying;
            return window;
        }

        private void OnWindowDestroying(object sender, EventArgs e)
        {
            try
            {
                string cacheDirPath = FileSystem.Current.CacheDirectory;

                if (Directory.Exists(cacheDirPath))
                {
                    string[] cacheFileNames = Directory.GetFiles(cacheDirPath);

                    foreach (var fileName in cacheFileNames)
                    {
                        File.Delete(fileName);
                    }
                }
            }
            catch (Exception ignore)
            {

            }
        }
    }
}