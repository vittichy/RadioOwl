using System.IO;
using System.Reflection;

namespace RadioOwlTests
{
    public abstract class TestBase
    {
        public string GetEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"RadioOwlTests.TestData.{fileName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
