using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace Antlr
{
    public class QueryVisitor : QueryBaseVisitor<object>
    {
        private IEnumerable<Dictionary<string,string>> _allData = new List<Dictionary<string, string>>();
        private string _operation = string.Empty;
        private HashSet<string> _left;
        private HashSet<string> _right;
        
        public override object Visit(IParseTree tree)
        {
            _allData = Data.GetData();
            base.Visit(tree);
            return _allData.Where(x => _left.Contains(x["name"])).ToList();
        }

        public override object VisitExpression(QueryParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            switch(_operation)
            {
                case "OR":
                    _left.UnionWith(_right);
                    break;
                case"AND":
                    _left.IntersectWith(_right);
                    break;
            }

            return new { };
        }

        public override object VisitExpressionPart(QueryParser.ExpressionPartContext context)
        {
            var notCompare = !string.IsNullOrWhiteSpace(context.NOT()?.GetText());
            var attribute = context.ATTRIBUTE().GetText();
            var comparison = context.COMPARISON().GetText();
            var value = context.QUOTEDVALUE().GetText();
            
            var filtered = _allData
                .Where(x => Evaluate(x, attribute, comparison, value, notCompare))
                .Select(x => x["name"])
                .ToHashSet();

            if (string.IsNullOrWhiteSpace(_operation))
            {
                _left = filtered;
            }
            else
            {
                _right = filtered;
            }

            return base.VisitExpressionPart(context);
        }

        public override object VisitOperation(QueryParser.OperationContext context)
        {
            _operation = context.GetText();
            return base.VisitOperation(context);
        }

        private bool Evaluate(Dictionary<string,string> dictionary, string attribute, string operation, string compare, bool negateCompare)
        {
            var attributeValue = dictionary[attribute];

            compare = compare.Replace("'", String.Empty);
            if (Int32.TryParse(attributeValue, out var intValue) && Int32.TryParse(compare, out var intComparison))
            {
                return RunComparison(intValue, intComparison, operation, negateCompare);
            }

            return RunComparison(String.Compare(attributeValue, compare, StringComparison.InvariantCultureIgnoreCase), 0, operation, negateCompare);
        }

        private bool RunComparison(int compare, int p1, string comparison, bool negateCompare)
        {
            if (negateCompare)
            {
                p1 *= -1;
            }
            
            return comparison switch
            {
                ">" => compare > p1,
                ">=" => compare >= p1,
                "<" => compare < p1,
                "<=" => compare <= p1,
                "=" => compare == p1,
            };
        }
    }
}