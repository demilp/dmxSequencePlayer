using Bypass;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace DMXSequencePlayer
{
    public class DmxPlayer : IDisposable
    {
        string[] sequence;
        BypassClient client;
        public DmxPlayer()
        {
            try
            {
                client = new BypassClient(ConfigurationManager.AppSettings["serverIp"], int.Parse(ConfigurationManager.AppSettings["serverPort"]), ";", ConfigurationManager.AppSettings["bypassId"]);
                client.CommandReceivedEvent += OnData;
            }
            catch (Exception e)
            {
                throw new Exception("Error on socket creation: "+e.Message);
            }
        }

        private void OnData(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.comando.ToLower().StartsWith("play"))
                {
                    string[] s = e.comando.Split('|');
                    if (s.Length > 1)
                    {
                        sequence = File.ReadAllLines(ConfigurationManager.AppSettings["filePath" + s[1]]);
                    }
                    else
                    {
                        sequence = File.ReadAllLines(ConfigurationManager.AppSettings["filePath"]);
                    }


                    if (thread != null)
                    {
                        thread.Abort();
                    }
                    thread = new Thread(Play);
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        Thread thread;
        int currentLine;
        bool run;
        public void Play()
        {
            Console.WriteLine("Play");
            run = true;
            currentLine = 0;
            while (currentLine < sequence.Length && run)
            {
                if (sequence[currentLine].StartsWith("sleep", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    Console.WriteLine(sequence[currentLine]);
                    Thread.Sleep(int.Parse(sequence[currentLine].Substring(sequence[currentLine].IndexOf(" ") + 1)));
                }
                else if (sequence[currentLine].StartsWith("dmx", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    Console.WriteLine(sequence[currentLine]);
                    client.SendData(sequence[currentLine].Substring(sequence[currentLine].IndexOf(" ") + 1), "", "dmx");
                }
                else if (sequence[currentLine].ToLower() == "exit")
                {
                    run = false;
                    Console.WriteLine("----EXIT----");
                    Console.WriteLine("");
                }
                currentLine++;
                currentLine %= sequence.Length;

            }
        }

        public void Dispose()
        {
            client.Close();
        }
    }
}
