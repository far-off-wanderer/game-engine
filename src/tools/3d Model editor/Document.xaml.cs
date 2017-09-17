using Windows.UI.Xaml.Controls;

namespace Editor
{
    public sealed partial class Document : PivotItem
    {
        string filename = default(string);

        public Document() : this(filename: default(string))
        {
        }

        public Document(string filename)
        {

            this.InitializeComponent();

            this.Filename = filename;
        }

        public string Filename
        {
            get => filename;
            private set
            {
                filename = value;
                Header = value ?? "*unnamed*";
            }
        }
    }
}
