namespace Nightmare_Survival
{
    public static class Program
    {
        private static void Main()
        {
            using var game = new GameClass();
            game.Run();
        }
    }
}