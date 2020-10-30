using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace Antlr
{
    public class QueryVisitor : QueryBaseVisitor<object>
    {
        private IEnumerable<Dictionary<string, string>> _allData = new List<Dictionary<string, string>>();
        private HashSet<string> _left = new HashSet<string>();
        private HashSet<string> _right = new HashSet<string>();

        public override object Visit(IParseTree tree)
        {
            _allData = Data.GetData();
            base.Visit(tree);
            return _allData.Where(x => _left.Contains(x["name"])).ToList();
        }

        public override object VisitOrExpression(QueryParser.OrExpressionContext context)
        {
            base.VisitOrExpression(context);

            if (_right.Any())
            {
                _left.UnionWith(_right);
            }

            return new { };
        }

        public override object VisitAndExpression(QueryParser.AndExpressionContext context)
        {
            base.VisitAndExpression(context);

            if (_right.Any())
            {
                _left.IntersectWith(_right);
            }

            return new { };
        }

        public override object VisitExpressionPart(QueryParser.ExpressionPartContext context)
        {
            var attribute = context.ATTRIBUTE().GetText();
            var comparison = context.COMPARISON().GetText();
            var value = context.QUOTEDVALUE().GetText();

            var filtered = _allData
                .Where(x => Evaluate(x, attribute, comparison, value))
                .Select(x => x["name"])
                .ToHashSet();

            if (!_left.Any())
            {
                _left = filtered;
            }
            else
            {
                _right = filtered;
            }

            return base.VisitExpressionPart(context);
        }

        private bool Evaluate(Dictionary<string, string> dictionary, string attribute, string operation, string compare)
        {
            var attributeValue = dictionary[attribute];

            compare = compare.Replace("'", String.Empty);
            if (Int32.TryParse(attributeValue, out var intValue) && Int32.TryParse(compare, out var intComparison))
            {
                return RunComparison(intValue, intComparison, operation);
            }

            return RunComparison(String.Compare(attributeValue, compare, StringComparison.InvariantCultureIgnoreCase),
                0, operation);
        }

        private bool RunComparison(int compare, int p1, string comparison)
        {
            return comparison switch
            {
                ">" => compare > p1,
                ">=" => compare >= p1,
                "<" => compare < p1,
                "<=" => compare <= p1,
                "=" => compare == p1,
                "!=" => compare != p1
            };
        }
    }
}