using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMassMailToLargePopulations
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = new ProcessEmployeesAndStudents();
            process.DoWork();
        }
    }
}
