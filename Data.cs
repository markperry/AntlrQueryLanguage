using System.Collections.Generic;

namespace Antlr
{
    public static class Data
    {
        public static List<Dictionary<string, string>> GetData()
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"name", "Bob"},
                    {"age", "20"},
                    {"eye_colour", "blue"}
                },
                
                new Dictionary<string, string>
                {
                    {"name", "Bob"},
                    {"age", "25"},
                    {"eye_colour", "blue"}
                },

                new Dictionary<string, string>
                {
                    {"name", "John"},
                    {"age", "20"},
                    {"eye_colour", "brown"}
                },
                
                new Dictionary<string, string>
                {
                    {"name", "John"},
                    {"age", "25"},
                    {"eye_colour", "brown"}
                },

                new Dictionary<string, string>
                {
                    {"name", "Sally"},
                    {"age", "30"},
                    {"eye_colour", "hazel"}
                },

                new Dictionary<string, string>
                {
                    {"name", "Helen"},
                    {"age", "35"},
                    {"eye_colour", "green"}
                }
            };
        }
    }
}