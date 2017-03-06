using System;

namespace windows
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new game.Game1())
                game.Run();
        }
    }
}
