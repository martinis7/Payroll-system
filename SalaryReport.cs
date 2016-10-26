using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem
{
    public class SalaryReport
    {
        private decimal salarySum;
        private int overtimeSum;

        public decimal SalarySum
        {
            get
            {
                return SalarySum;
            }

            set
            {
                SalarySum = value;
            }
        }

        public int OvertimeSum
        {
            get
            {
                return overtimeSum;
            }

            set
            {
                overtimeSum = value;
            }
        }

        public SalaryReport Add(SalaryReport reportToAdd)
        {
            SalarySum += reportToAdd.SalarySum;
            overtimeSum += reportToAdd.overtimeSum;
            return this;
        }

        public override string ToString()
        {
            return "\t\t\t\tOvertime sum: " + overtimeSum + " Salary sum: " + salarySum.ToString("C2");
        }
    }
}
