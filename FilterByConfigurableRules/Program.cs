using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Types.Filtering;
using Engine.Types.Selection;
using Newtonsoft.Json;

namespace FilterByConfigurableRules
{
    class Program
    {
        private const string DATA_DIR = @"C:\Code\filter-by-configurable-rules\FilterByConfigurableRules\Data";

        static void Main(string[] args)
        {
            var data = GetDataSource();
            var conditions = GetConditions();
            var mappings = GetMappings();
            
            var filteringEngine = new FilteringEngine();

            var filtered = filteringEngine.Filter(data, conditions);

            PrintCollection(filtered, "Filtered:");

            var selectionEngine = new SelectionEngine();

            var selection = selectionEngine.Select<Person, SelectionType>(filtered, mappings);

            PrintCollection(selection, "Selected:");
        }

        private static string ReadTextFile(string filename)
        {
            var path = Path.Combine(DATA_DIR, filename);

            return File.ReadAllText(path);
        }

        private static Person[] GetDataSource()
        {
            var content = ReadTextFile("data-source.json");

            return JsonConvert.DeserializeObject<Person[]>(content);
        }

        private static ConditionGroup GetConditions()
        {
            var content = ReadTextFile("filter-conditions.json");

            return JsonConvert.DeserializeObject<ConditionGroup>(content);
        }

        private static SelectionMapping[] GetMappings()
        {
            var content = ReadTextFile("selection-mappings.json");

            return JsonConvert.DeserializeObject<SelectionMapping[]>(content);
        }

        private static void PrintCollection<T>(IEnumerable<T> collection, string caption)
        {
            Console.WriteLine(caption);

            foreach (var x in collection)
            {
                Console.WriteLine(JsonConvert.SerializeObject(x));
            }

            Console.WriteLine();
        }
    }
}
