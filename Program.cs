using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Antlr
{
    class Program
    {
        private const string Input = "name != 'Bob'";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Checking filter: {Input}");

                // Create the Antlr Lexer and Tokenizer
                AntlrInputStream inputStream = new AntlrInputStream(Input);
                QueryLexer queryLexer = new QueryLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(queryLexer);
                QueryParser queryParser = new QueryParser(commonTokenStream);

                // Invoke the visitor pattern on the parsed AST
                IParseTree context = queryParser.query();
                QueryVisitor visitor = new QueryVisitor();
                var filteredData = visitor.Visit(context) as List<Dictionary<string, string>>;
                foreach (var item in filteredData)
                {
                    Console.WriteLine($"Name:{item["name"]}, Age:{item["age"]}, Eye Colour:{item["eye_colour"]}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}