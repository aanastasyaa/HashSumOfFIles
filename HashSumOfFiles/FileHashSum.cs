using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashSumOfFiles
{
    public class FileHashSum
    {
        public string Filename { get; set; }
        public string HashSum { get; set; }
        public string Error { get; set; }

        public FileHashSum(string filename)
        {
            this.Filename = filename;
            this.HashSum = string.Empty;
            this.Error = string.Empty;
        }
    }
}
