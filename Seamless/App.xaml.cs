namespace Seamless
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Start with the Landing Page (MainPage) inside a Navigation wrapper
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}