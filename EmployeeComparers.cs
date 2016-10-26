using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollSystem
{
    public class CompareByID : IComparer<Employee>
    {
        int IComparer<Employee>.Compare(Employee x, Employee y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }

    public class CompareBySalary : IComparer<Employee>
    {
        int IComparer<Employee>.Compare(Employee x, Employee y)
        {
            return (int)x._salary.CountAllSalary() - (int)y._salary.CountAllSalary();
        }
    }

    public class CompareByOvertime: IComparer<Employee>
    {
        int IComparer<Employee>.Compare(Employee x, Employee y)
        {
            return x._salary.OvertimeHours.CompareTo(y._salary.OvertimeHours);
        }
    }
}
