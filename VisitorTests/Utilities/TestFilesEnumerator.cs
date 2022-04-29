using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorTests
{
    internal abstract class BaseFilesEnumerator : IEnumerable<object[]>
    {
        public abstract string RelativeFolderPath();
        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var filePath in GetTestFilesRecursively(FileReadingTestUtilities.ProjectBaseDirectory + RelativeFolderPath()))
            {
                yield return filePath;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private static IEnumerable<object[]> GetTestFilesRecursively(string directoryPath)
        {
            // Get all files in root directory
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                yield return new object[] { filePath };
            }
            // get all subdirectories
            foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            {
                // get everything from current subdirectory
                foreach (var file in GetTestFilesRecursively(subdirectoryPath))
                {
                    yield return file;
                }
            }

        }
    }

}
