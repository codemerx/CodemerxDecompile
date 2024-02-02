namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader reader = File.OpenText("numbers.txt"))
            {
                Console.WriteLine($"{reader.ReadToEnd()}");
            }
        }
    }
}