using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using CommandLine;

namespace GoNet
{
    class ErrorHandler : IAntlrErrorListener<IToken>
    {
        private List<string> m_lines;
        public ErrorHandler(StreamReader sr)
        {
            long oPosition = sr.BaseStream.Position;
            sr.BaseStream.Position = 0;
            m_lines = new List<string>();

            string line = null;
            while ((line = sr.ReadLine()) != null)
                m_lines.Add(line);

            sr.BaseStream.Position = oPosition;
        }
        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.WriteLine("ERROR {0}:{1}  -  {2}", line, charPositionInLine, msg);

            if (line <= m_lines.Count)
            {
                var oldCol = Console.ForegroundColor;
                var sline = m_lines[line - 1];
                int pos = 0;
                Console.Write("     ");
                for (int i = 0; i < sline.Length; i++)
                {
                    if (i >= offendingSymbol.Column && i < (offendingSymbol.Column + (offendingSymbol.StopIndex - offendingSymbol.StartIndex) + 1))
                    {
                        if (Console.ForegroundColor != ConsoleColor.Red)
                            Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (Console.ForegroundColor == ConsoleColor.Red)
                        Console.ForegroundColor = oldCol;

                    if (sline[i] == '\t')
                    {
                        if (i < charPositionInLine)
                            pos += 4;
                        Console.Write("    ");
                    }
                    else
                    {
                        if (i < charPositionInLine)
                            pos++;
                        Console.Write(sline[i]);
                    }
                }
                if (Console.ForegroundColor == ConsoleColor.Red)
                    Console.ForegroundColor = oldCol;

                Console.WriteLine();
                Console.Write("     ");
                for (int i = 0; i < pos; i++)
                    Console.Write(' ');
                Console.WriteLine("^");
            }
        }
    }

    class Options
    {
        public enum EBuildMode
        {
            Default,
            BuildLibrary
        }

        [Option('b', "build-type", DefaultValue = EBuildMode.Default)]
        public EBuildMode BuildMode { get; set; }

        [Option('d', "dynamic-link", DefaultValue = true, MutuallyExclusiveSet ="link")]
        public bool DynamicLink { get; set; }
        [Option('s', "static-link", DefaultValue =false,MutuallyExclusiveSet ="link")]
        public bool StaticLink { get; set; }
    }

    class Program
    {
        private const string StandardLibraryLocation = "StandardLibrary";

        private static bool IsInteralLibrary(string name)
        {
            return name == "unsafe";
        }
        private static bool BuildLibrary(string name)
        {
            if (IsInteralLibrary(name))
                throw new NotImplementedException();

            var dir = Path.Combine(Environment.CurrentDirectory, StandardLibraryLocation, name);

            // create ast root
            var root = new AST.Root();
            var builder = new AstBuilder(root);

            int count = 1;

            foreach(var file in Directory.EnumerateFiles(dir, "*.go"))
            {
                // parse each file
                var basename = Path.GetFileNameWithoutExtension(file);
                var index = basename.LastIndexOf('_');
                var fileFlag = string.Empty;
                if(index != -1)
                    fileFlag = basename.Substring(index + 1);

                if (string.IsNullOrEmpty(fileFlag))
                {
                    using(var fileStream = new StreamReader(file, Encoding.UTF8))
                    {
                        var parser = new Parser.GolangParser(
                            new CommonTokenStream(
                                new Parser.GolangLexer(
                                    new AntlrInputStream(fileStream))));

                        parser.RemoveErrorListeners();
                        parser.AddErrorListener(new ErrorHandler(fileStream));
                        builder.Visit(parser.source_file());
                    }

                    if (--count == 0)
                        break;
                }
            }

            var resolver = new AstResolver();

            // constant subsitution
            resolver.Process(root);
            // type assignment
            // type check

            // compile ast to il/lib

            //Console.WriteLine(root.Packages.First().Value.Imports.Count);

            return false;
        }

        private static void StandAloneTest(string text)
        {
            var inputStream = new AntlrInputStream(text);
            var lexer = new Parser.GolangLexer(inputStream);
            
            var tokens = lexer.GetAllTokens();
            Console.WriteLine("=== Lexer matches ===");
            foreach(var token in tokens)
            {
                Console.WriteLine("    {0} ({1})", token.Text, lexer.RuleNames[token.Type-1]);
            }

            lexer.Reset();
            var cts = new CommonTokenStream(lexer);
            var parser = new Parser.GolangParser(cts);

            parser.Trace = true;
            parser.statement();
        }

        static void Main(string[] args)
        {
            //StandAloneTest("(padding[0:m])");
            //StandAloneTest("f.buf.Write(padding[0:m])");
            Console.WriteLine(Utils.BigRationalExtensions.Parse("100.125e2"));

            BuildLibrary("math");
        }
    }
}
