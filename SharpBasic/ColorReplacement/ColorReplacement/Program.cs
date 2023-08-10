namespace ColorReplacement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {

            var colors = new Dictionary<string, string>();
            var colorsCount = new Dictionary<string, int>();

            using (var source = new StreamReader("Data/colors.txt", Encoding.UTF8))
            {
                string line;
                while ((line = source.ReadLine()) != null)
                {
                    var colorDescr = line.Split(' ');
                    var name = colorDescr[0];
                    var color = colorDescr[1].Substring(1);

                    colors.Add(color, name);
                    colorsCount.Add(color, 0);
                }
            }

            using (var source = new StreamReader("Data/source.txt", Encoding.UTF8))
            using (var target = new StreamWriter("Data/target.txt"))
            {
                // read source.txt, replaces colors, writes target.txt, collects data about replaced colors
                // take into account that colors can be appeared in two formats:
                // #d46784
                // rgb(199, 21, 133)

                string line;
                while ((line = source.ReadLine()) != null)
                {
                    Regex myReg = new Regex(@"rgb\((\d{1,3}),\s(\d{1,3}),\s(\d{1,3})\)");
                    MatchCollection matches = myReg.Matches(line);
                    foreach (Match match in matches)
                    {
                        line = myReg.Replace(line, string.Format(
                                            "#{0}{1}{2}",
                                            Convert.ToInt32(match.Groups[1].Value, 10).ToString("x"),
                                            Convert.ToInt32(match.Groups[2].Value, 10).ToString("x"),
                                            Convert.ToInt32(match.Groups[3].Value, 10).ToString("x")),1);
                    }
                    var words = line.Split(new[] { ' ', '\t' });
                    var newLine = string.Join(
                        " ",
                        words.Select(word =>
                        {
                            foreach (var item in colors)
                            {
                                if (word.IndexOf(item.Key,StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    colorsCount[item.Key]++;
                                    return item.Value;
                                }
                            }

                            return word;
                        }));

                    target.WriteLine(newLine);
                }
            }

            using (var target = new StreamWriter("Data/used_colors.txt"))
            {
                // write data about replaced colors
                // format should be about the same like for source.txt but third column with count of using of this color should be added

                foreach (var item in colors)
                {
                    target.WriteLine(string.Format("{0} {1} {2}", item.Key, item.Value, colorsCount[item.Key]));
                }
            }
        }
    }
}
