namespace Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            const double Word2ColorProbability = 0.1;
            const double UnknownColorProbability = 0.1;

            var colors = new Dictionary<string, string>();
            using (var sr = new StreamReader("Source/colors.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var colorDescr = line.Split(' ');
                    var name = colorDescr[0];
                    var color = colorDescr[1].Substring(1);

                    colors.Add(color, name);
                }
            }

            var random = new Random();

            using (var source = new StreamReader("Source/sourceText.txt", Encoding.UTF8))
            using (var target = new StreamWriter("Source/source.txt"))
            {
                string line;
                while ((line = source.ReadLine()) != null)
                {
                    var words = line.Split(new[] {' ', '\t'});
                    var newLine = string.Join(
                        " ",
                        words.Select(word =>
                        {
                            if (random.NextDouble() < Word2ColorProbability)
                            {
                                string color = random.NextDouble() < UnknownColorProbability
                                    ? random.Next(256 * 256 * 256).ToString("x6")
                                    : colors.Keys.ToList()[random.Next(colors.Count - 1)];

                                switch (random.Next(2))
                                {
                                    case 0:
                                        if (color[0] == color[1] &&
                                            color[2] == color[3] &&
                                            color[4] == color[5])
                                        {
                                            return "#" + color[0] + color[2] + color[4];
                                        }

                                        return "#" + color;

                                    case 1:
                                        return string.Format(
                                            "rgb({0}, {1}, {2})",
                                            Convert.ToInt32(color.Substring(0, 2), 16),
                                            Convert.ToInt32(color.Substring(2, 2), 16),
                                            Convert.ToInt32(color.Substring(4, 2), 16));
                                }
                            }

                            return word;
                        }));

                    target.WriteLine(newLine);
                }
            }
        }
    }
}
