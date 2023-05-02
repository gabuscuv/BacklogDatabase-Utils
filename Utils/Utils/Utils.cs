namespace GameListDB
{
    public static class Utils
    {
        public static int MenuInteractive(string sa)
        {
            return 0;
        }

        public static void WriteSection()
        {
            System.Console.Write("\n");
            for (int counter = 0; counter < System.Console.BufferWidth; counter++) { System.Console.Write("-"); }
            System.Console.Write("\n");
        }

        // TODO: make a logger
        public static void Log(string log, [System.Runtime.CompilerServices.CallerMemberName] string functionName = "")
        {
            System.Console.WriteLine("LOG: [" + functionName + "]: " + log);
        }

    };
}