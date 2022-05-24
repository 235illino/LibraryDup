using System.Security.Cryptography;

namespace LibraryDup
{
    public interface IDuplicate
    {
        IReadOnlyCollection<string> Filepaths { get; }
    }

    public class Duplicate : IDuplicate
    {
        private IReadOnlyCollection<string> Filepath;

        public Duplicate(IReadOnlyCollection<string> Filepath)
        {
            this.Filepath = Filepath;
        }

        public IReadOnlyCollection<string> Filepaths
        {
            get
            {
                return Filepath;
            }
        }

    }
    public enum ComparisonMode
    {
        SizeAndName,
        Size
    }
    public interface IDuplicateCheck
    {
        IReadOnlyCollection<IDuplicate> CollectCandidates(string path);

        IReadOnlyCollection<IDuplicate> CollectCandidates(string path, ComparisonMode mode);

        IReadOnlyCollection<IDuplicate> CheckCandidates(IEnumerable<IDuplicate> candidates);
    }

    public class DuplicateCheck : IDuplicateCheck
    {
        public IReadOnlyCollection<IDuplicate> CheckCandidates(IEnumerable<IDuplicate> candidates)
        {
            VerifyCollection(candidates);
            return CheckDuplicateByHash(candidates);
        }

        public IReadOnlyCollection<IDuplicate> CollectCandidates(string path)
        {
            IEnumerable<FileInfo> fileList = GetAllFiles(path);
            return CollectDuplicateBySizeAndName(fileList);
        }

        public IReadOnlyCollection<IDuplicate> CollectCandidates(string path, ComparisonMode mode)
        {
            IReadOnlyCollection<IDuplicate> duplicate = new List<IDuplicate>();
            IEnumerable<FileInfo> fileList = GetAllFiles(path);

            switch (mode)
            {
                case ComparisonMode.Size:
                    return CollectDuplicateBySize(fileList);
                case ComparisonMode.SizeAndName:
                    return CollectDuplicateBySizeAndName(fileList);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        public void InitProgram()
        {
            ConsoleWrite("Hello! This program helps you to find duplicate files in a directory tree\n");
           
            do
            {
                ConsoleWrite("\nPlease, paste the path to exist directory\n");
                string path = ConsoleReadingStrings();
                VerifyPath(path);
                IReadOnlyCollection<IDuplicate> duplicates = ChooseMethod(path);
                ShowResult(duplicates);
                ConsoleWrite("Do you want to check the files for matches on their contents? \n Y/N\n");
                if (String.Equals(ConsoleReadingStrings(), "Y", StringComparison.OrdinalIgnoreCase))
                {
                    ShowResult(CheckCandidates(duplicates));
                }
                ConsoleWrite("Press <Enter> for finish program, if you want to find duplicate files once again press any other key\n");
                
            } while (Console.ReadKey().Key != ConsoleKey.Enter);
        }

        private IReadOnlyCollection<IDuplicate> ChooseMethod(string path)
        {
            
            ConsoleWrite("\nPlease, choose method for finding duplicates:\n" +
                                  "enter 1 for \"default method (searching duplicates by size and name)\"\n" +
                                  "enter 2 for \"moding method (searching duplicates by choosing mode)\"\n");
            int methodSelected = Convert.ToInt32(ConsoleReadingStrings());

            switch (methodSelected)
            {
                case 1:
                    return CollectCandidates(path);
                case 2:
                    ConsoleWrite("Please, choose mode for moding method:\n" +
                                 "enter 1 for \"Size mode\"\n" +
                                 "enter 2 for \"Size and Name mode\"\n");
                    return CollectCandidates(path, ModeSelect());
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodSelected));

            }
        }

        private ComparisonMode ModeSelect()
        {
            int mode = Convert.ToInt32(ConsoleReadingStrings());
            if (mode == 1 || mode == 2)
            {
                return mode == 1 ? ComparisonMode.Size : ComparisonMode.SizeAndName;
            }
            else { throw new ArgumentOutOfRangeException(nameof(mode)); }
        }

        private string ConsoleReadingStrings()
        {
            return Console.ReadLine();
        }

        private void ConsoleWrite(string consoleOut)
        {
            Console.WriteLine(consoleOut);
        }

        private IReadOnlyCollection<IDuplicate> CheckDuplicateByHash(IEnumerable<IDuplicate> candidates)
        {
            List<IDuplicate> duplicate = new List<IDuplicate>();
            foreach (IDuplicate dup in candidates)
            {

                IDuplicate duplicateGroup = new Duplicate(dup.Filepaths.GroupBy(x => CalculateMD5(x))
                               .Where(x => x.Skip(1).Any())
                               .SelectMany(x => x.Select(i => i)).ToList());
                if (duplicateGroup.Filepaths.Count > 1)
                {
                    duplicate.Add(duplicateGroup);
                }
            }
            return duplicate.ToList().AsReadOnly();
        }

        private IReadOnlyCollection<IDuplicate> CollectDuplicateBySize(IEnumerable<FileInfo> fileList)
        {
            return fileList.GroupBy(x => x.Length)
                           .Where(x => x.Count() > 1)
                           .Select(group => new Duplicate(group.Select((i => i.FullName)).ToList()))
                           .ToList()
                           .AsReadOnly();
        }

        private IReadOnlyCollection<IDuplicate> CollectDuplicateBySizeAndName(IEnumerable<FileInfo> fileList)
        {
            return fileList.GroupBy(x => (x.Length, x.Name.ToLower()))
                           .Where(x => x.Count() > 1)
                           .Select(group => new Duplicate(group.Select((i => i.FullName)).ToList()))
                           .ToList()
                           .AsReadOnly();
        }

        public IEnumerable<FileInfo> GetAllFiles(string path)
        {
            return new DirectoryInfo(path).EnumerateFiles("*.*", SearchOption.AllDirectories);
        }

        private string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void VerifyPath(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException("Empty Argument");
            }
            if (!Directory.Exists(path))
            {
                throw new ArgumentException(path, String.Format("Path {0} does not exist!", path));
            }
        }

        private void VerifyCollection(IEnumerable<IDuplicate> candidates)
        {
            if (candidates is null)
            {
                throw new ArgumentNullException("Empty candidates");
            }
            if (candidates.Count() == 0)
            {
                throw new ArgumentException("Nothing to check!");
            }
        }

        public void ShowResult(IReadOnlyCollection<IDuplicate> candidates)
        {
            Console.WriteLine();
            foreach (var candidate in candidates)
            {
                foreach (var dup in candidate.Filepaths)
                {
                    Console.WriteLine(dup);
                }
            }
            Console.WriteLine();
        }
        static void Main()
        {
            DuplicateCheck duplicate = new DuplicateCheck();
            duplicate.InitProgram();
        }


    }



}
