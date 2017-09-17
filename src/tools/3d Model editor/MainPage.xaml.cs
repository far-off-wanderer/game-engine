using Windows.UI.Xaml.Controls;

namespace Editor
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Home_ClickedNew(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mainTabs.Items.Add(new Document());
        }

        private void Home_ClickedOpen(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mainTabs.Items.Add(new Document("demo.3dmodel"));
        }
    }
}
