using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem
{
    public class SalaryReport
    {
        public decimal salarySum { get; set; }
        public int overtimeSum {get;set;}
        public int count { get; set; }

        public SalaryReport()
        {

        }
        public SalaryReport(decimal salarySum, int overtimeSum, int count)
        {
            this.salarySum = salarySum;
            this.overtimeSum = overtimeSum;
            this.count = count;
        }

        public SalaryReport Add(decimal salary, int overtime)
        {
            SalaryReport report = new SalaryReport();
            report.salarySum = this.salarySum + salary;
            report.overtimeSum = this.overtimeSum + overtime;
            report.count = count + 1;
            return report;
        }
    }
}
