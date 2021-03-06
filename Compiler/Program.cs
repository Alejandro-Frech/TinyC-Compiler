﻿﻿﻿﻿﻿﻿﻿using System;
using System.IO;
using ParserProject.Semantic;

namespace Compiler
{
    public class Program
    {
        static void Main(string[] args)
        {

            var file = new FileManager();

            //var sourceCode = file.GetSourceCode("./TestProject/Level1/Test3.cs");

            //try
            //{
            //    var lex = new Lexer(new InputString(sourceCode));
            //    var parser = new Parser(lex);
            //    var tree = parser.Parse();
            //    Console.WriteLine("SUCCESS");
            //}


            try
            {
                var treeList = file.GetTreeListFromFiles();
                TypeTable.FillTable(treeList);
                var classList = TypeTable.TypeList;
				StreamWriter writer = new StreamWriter(File.Create(Path.Combine(AppContext.BaseDirectory.
																		  Substring(0, AppContext.BaseDirectory.IndexOf("Compiler", StringComparison.Ordinal)), "TestSourceCode/output.js")));
                writer.WriteLine(file.systemClasses);
				writer.Flush();
                writer.WriteLine(file.predefinedFunctions);
				writer.Flush();
                foreach (var cl in classList.Values)
                {
                    writer.WriteLine(cl.GenerateCode().Code);
                    writer.Flush();
                    //Console.WriteLine(cl.GenerateCode().Code);
                }
                writer.Flush();
                writer.WriteLine("Program.Main('Program');");
                writer.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.ReadLine();
        }
     }
}
