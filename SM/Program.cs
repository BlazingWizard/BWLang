using System;
using System.IO;
using System.Text;
using BW.Lexer;
using BW.Optimization;
using BW.SyntaxAnalayzer;

namespace BW.SM
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                throw new FileLoadException();
            }
            var programText = File.ReadAllText(filePath);
            
            Tokinaizer tokinaizer = new Tokinaizer(programText);
            var tokens = tokinaizer.Tokinaize();
            
            var parser = new Parser(tokens);
            parser.CreatePolis();

            var opt = new Optimazer(parser._polis);
            var opt_polis = opt.Optimaze();

            var sm = new StackMachine(opt_polis);
            sm.PerformProgram();

            var log = new StringBuilder();
            log.AppendLine("============");
            log.AppendLine("TOKEN INFO");
            log.AppendLine("============");
            log.AppendLine(string.Join("\r", tokens));
            log.AppendLine("============");
            log.AppendLine("POLIS INFO");
            log.AppendLine("============");
            log.AppendLine(string.Join("\r", parser._polis));
            log.AppendLine("============");
            log.AppendLine("OPTIMAZED POLIS INFO");
            log.AppendLine("============");
            log.AppendLine(string.Join("\r", opt_polis));

            var logDir = filePath.Replace(filePath.Substring(filePath.LastIndexOf("\\")), "");
            var logWriter = new StreamWriter(Path.Combine(logDir, "log.txt"));
            using (logWriter)
            {
                logWriter.Write(log.ToString());
            }

            Console.Read();
        }
    }
}
