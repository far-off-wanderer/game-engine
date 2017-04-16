namespace uwp
{
    using Game;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class GamePage : Page
    {
		readonly Game _game;

		public GamePage()
        {
            this.InitializeComponent();

			var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }
    }
}
