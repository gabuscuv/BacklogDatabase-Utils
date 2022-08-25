namespace GamelistDB
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
    };
}