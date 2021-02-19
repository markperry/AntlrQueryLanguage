using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;

namespace Antlr
{
    public class QueryVisitor : QueryBaseVisitor<object>
    {
        private List<Dictionary<string, string>> _allData = new List<Dictionary<string, string>>();
        private IEnumerable<Dictionary<string, string>> _filterTarget = new List<Dictionary<string, string>>();
        private List<Dictionary<string, string>> _filterResults = new List<Dictionary<string, string>>();

        public override object Visit(IParseTree tree)
        {
            _allData = Data.GetData();
            base.Visit(tree);
            return _filterResults.Distinct().ToList();
        }

        public override object VisitAndExpression(QueryParser.AndExpressionContext context)
        {
            _filterTarget = new List<Dictionary<string, string>>(_allData);
            base.VisitAndExpression(context);
            _filterResults.AddRange(_filterTarget.ToList());
            _filterTarget = new List<Dictionary<string, string>>();
            return new { };
        }

        public override object VisitExpressionPart(QueryParser.ExpressionPartContext context)
        {
            var attribute = context.ATTRIBUTE().GetText();
            var comparison = context.COMPARISON().GetText();
            var value = context.QUOTEDVALUE().GetText();

            _filterTarget = _filterTarget
                .Where(x => Evaluate(x, attribute, comparison, value));

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