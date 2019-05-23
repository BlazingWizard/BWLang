using System;
using System.IO;
using System.Text;
using BW.Lexer;
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

            var sm = new StackMachine(parser._polis);
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
