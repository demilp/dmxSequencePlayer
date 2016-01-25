using System;
using System.Configuration;

namespace DMXSequencePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new DmxPlayer();
                string s;
                do
                {
                    s = Console.ReadLine();
                } while (s != "exit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
