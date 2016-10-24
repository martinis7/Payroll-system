using System;

namespace PayrollSystem
{
    public class SmartEmployeeComparer : IComparer<Employee>
    {
        public enum CompareField
        {
            Id,
            Salary,
            Overtime,
        }

        public CompareField SortBy;

        public int Compare(Employee x, Employee y)
        {
            switch (SortBy)
            {
                case CompareField.Id:
                    return x.Id.CompareTo(y.Id);
                case CompareField.Salary:
                    return (int)x.Salary.CountAllSalary() - (int)y.Salary.CountAllSalary();
                case CompareField.Overtime:
                    return x.Salary.OvertimeHours.CompareTo(y.Salary.OvertimeHours);
            }
            return x.Id.CompareTo(y.Id);
        }
    }
}