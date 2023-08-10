using Marble.Core;

namespace Marble.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FileTest()
        {
            using (var fs = new FileStream("input.txt", FileMode.Open))
            {
                var gameCases = FileHelper.ReadFile(fs);
                HashSet<GameCase> cases = new();
                foreach (var gameCase in gameCases)
                {
                    cases.Add(gameCase);
                }
                Assert.That(cases.Count, Is.EqualTo(4));
            }
        }
    }
}