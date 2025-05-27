using Reloaded.Memory.Sigscan;
using Reloaded.Memory.Sources;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RoadcraftStutterRemover
{

    class Program
    {
        /// <summary>
        /// Original code
        ///"Roadcraft - Retail.exe"+18B1E80 - 41 81 FC 19020000     - cmp r12d,00000219 { 537 }
        ///"Roadcraft - Retail.exe"+18B1E87 - 0F84 9F000000         - je ""Roadcraft - Retail.exe""+18B1F2C { ->"Roadcraft - Retail.exe"+18B1F2C }
        /// 
        /// The original code reloads all hid devices when a WM_DEVICECHANGE (0x219) message was found
        /// This patches this check to never find the message, therefore removing the lag and input loss
        /// A pattern search is used to hopefully make this compatible with future patches
        /// </summary>


        private static readonly string Pattern = "41 81 FC 19 02 00 00 0F 84"; //cmp edx,0x219;jne

        private static readonly byte[] Patch =
            { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }; //or eax,01;nop;nop;nop

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1) Patch the game executable (recommended)");
            Console.WriteLine(
                "2) Patch the running game, no changes to game .exe (circumvents Microsoft Store file protection)");
            Console.Write("# ");
            int input;
            while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out input) || (input != 1 && input != 2))
            {
                Console.Write("# ");
            }

            Console.WriteLine();

            if (input == 1)
            {
                MessageBox.Show(
                    "Please select Roadcraft - Retail.exe\nHere's the default path for a steam installation:\nC:\\Program Files (x86)\\Steam\\steamapps\\common\\RoadCraft\\root\\bin\\pc\\");

                var fd = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    Filter = "Roadcraft Applicaton|Roadcraft - Retail.exe",
                    Title = "Select Roadcraft - Retail.exe",
                    RestoreDirectory = true
                };

                if (fd.ShowDialog() != DialogResult.OK)
                {
                    Environment.Exit(0);
                }

                var fn = fd.FileName;
                var data = File.ReadAllBytes(fn);
                var scanner = new Scanner(data);
                var offset = scanner.CompiledFindPattern(Pattern);
                if (offset.Found)
                {
                    Console.WriteLine("Found patch location. Patching...");
                    for (var i = 0; i < Patch.Length; i++)
                    {
                        data[offset.Offset + i] = Patch[i];
                    }

                    File.WriteAllBytes(fn, data);
                    Console.WriteLine("Patch successful!");
                }
                else
                {
                    Console.WriteLine("Patch not found. Already patched? Exiting.");
                }

                Console.WriteLine("Press any key to exit.");

            }
            else if (input == 2)
            {
                Console.WriteLine("Looking for Roadcraft - Retail.exe");
                var p = Process.GetProcessesByName("Roadcraft - Retail.exe");
                while (p.Length == 0)
                {
                    Console.WriteLine("Waiting for Roadcraft - Retail.exe");
                    Thread.Sleep(1000);
                    p = Process.GetProcessesByName("Roadcraft - Retail.exe");
                }

                var snowRunnerProcess = p.First();
                var scanner = new Scanner(snowRunnerProcess, snowRunnerProcess.MainModule);
                var offset = scanner.CompiledFindPattern(Pattern);
                Console.WriteLine("Searching for patch location");
                if (offset.Found)
                {
                    Console.WriteLine("Found patch location. Patching game in memory...");
                    try
                    {
                        var memory = new ExternalMemory(snowRunnerProcess);
                        var baseAddress = snowRunnerProcess.MainModule.BaseAddress + offset.Offset;
                        memory.WriteRaw(baseAddress, Patch);
                        Console.WriteLine("Patch in memory successful!");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(
                            "Patching in memory failed. Try running RoadcraftStutterPatcher as Administrator");
                    }

                }
                else
                {
                    Console.WriteLine("Patch not found. Already patched? Exiting.");
                }
            }

            Console.ReadKey();

        }
    }
}