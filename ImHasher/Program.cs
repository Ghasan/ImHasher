using ImHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImHasher
{
    class Program
    {
        static int Main(string[] args)
        {
            var argsDict = new Dictionary<string, string>();

            bool isValid = FillArgs(args, argsDict);

            if (!isValid)
            {
                if (String.IsNullOrWhiteSpace(Error))
                {
                    Console.Error.WriteLine(GetHelp());
                }
                else
                {
                    Console.Error.WriteLine(Error);
                }

                return 1;
            }

            int tolerance = Int32.Parse(argsDict["t"]);
            IImHash hasher;

            if (argsDict["m"] == "avg")
            {
                hasher = new ImAvgHash(tolerance);
            }
            else
            {
                hasher = new ImDiffHash(tolerance);
            }

            bool[] hash1;
            bool[] hash2;

            try
            {
                hash1 = hasher.GetImageHash(argsDict["img1"]);
                hash2 = hasher.GetImageHash(argsDict["img2"]);
            }
            catch (Exception e)
            {
                Error = e.Message;
                Console.Error.WriteLine(Error);
                return 1;
            }

            bool areSimilar = hasher.AreSimilar(hash1, hash2);
            Console.WriteLine(areSimilar.ToString().ToLower());

            return 0;
        }

        // Private

        private static string Error { get; set; }

        private static bool FillArgs(string[] args, Dictionary<string, string> dict)
        {
            if (args.Length == 0)
            {
                return false;
            }

            string command = args.Aggregate((a, b) => a + " " + b);
            Match match = Regex.Match(command, @"^(-(m|t) (\w+) )?(-(?!\2)(m|t) (\w+) )?(\S+) (\S+)$");

            if (!match.Success)
            {
                return false;
            }

            var argsIndex = new Dictionary<string, int>();

            for (int i = 2; i <= 5; i += 3)
            {
                argsIndex[match.Groups[i].Value] = i;
            }

            foreach (string arg in new string[] { "m", "t" })
            {
                if (!argsIndex.ContainsKey(arg))
                {
                    argsIndex[arg] = -1;
                }
            }

            if (argsIndex["m"] != -1 && !new string[] { "diff", "avg" }.Contains(match.Groups[argsIndex["m"] + 1].Value))
            {
                return false;
            }

            if (argsIndex["t"] != -1 && !Int32.TryParse(match.Groups[argsIndex["t"] + 1].Value, out int tolerance))
            {
                return false;
            }

            if (match.Groups.Skip(7).Any(i => !i.Value.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)))
            {
                Error = "Only .jpg files are allowed now.";
                return false;
            }

            dict["m"] = argsIndex["m"] == -1 ? "diff" : match.Groups[argsIndex["m"] + 1].Value;
            dict["t"] = argsIndex["t"] == -1 ? "8" : match.Groups[argsIndex["t"] + 1].Value;
            dict["img1"] = match.Groups[7].Value;
            dict["img2"] = match.Groups[8].Value;

            return true;
        }

        private static string GetHelp()
        {
            return
                "ImHasher: A tool to determine similarity between two images.\n" +
                "\n" +
                "Usage: imhasher [-m {diff|avg}] [-t tolerance] [imgPath1] [imgPath2]\n" +
                "Output: {true|false}\n" +
                "\n" +
                "Options:\n" +
                "  -m: determines the method of comparison. 'diff' for difference method (default) and 'avg' for average method.\n" +
                "  -t: the hamming distance tolerance. Defaults to 8.";
        }
    }
}
