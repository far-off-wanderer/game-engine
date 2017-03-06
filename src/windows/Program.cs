namespace windows
{
    using game;
    using System;

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
                game.Run();
        }
    }
}
