using System;

namespace PayrollSystem
{

    [Flags]
    public enum SalaryFeatures
    {
        None = 0,
        Graduate = 1,
        Children = 2,
        Disability = 4
    }

    public struct Salary
    {
        // constants could be modified when using the system
        private const decimal weekendRate = 2.02M;
        private const decimal overtimeRate = 1.25M;
        private const int graduateBonus = 100;
        private const int childrenBonus = 200;
        private const int disabilityBonus = 150;

        private SalaryFeatures features { get; set; }

        private int hoursWorked;
        private decimal hourlyRate;
        private int overtimeHours;
        private int weekendHours;


        public int HoursWorked
        {
            get { return hoursWorked; }
            set
            {
                if (value >= 0)
                {
                    hoursWorked = value;
                }
                else
                {
                    throw new Exception("Hours worked must be positive");
                }
            }
        }

        public decimal HourlyRate
        {
            get { return hourlyRate; }
            set
            {
                if (value >= 0)
                {
                    hourlyRate = value;
                }
                else
                {
                    throw new Exception("Hourly rate must be positive");
                }
            }
        }

        public int OvertimeHours
        {
            get { return overtimeHours; }
            set
            {
                if (value >= 0)
                {
                    overtimeHours = value;
                }
                else
                {
                    throw new Exception("Overtime hours must be positive");
                }
            }
        }

        public int WeekendHours
        {
            get { return weekendHours; }
            set
            {
                if (value >= 0)
                {
                    weekendHours = value;
                }
                else
                {
                    throw new Exception("Weekend hours must be positive");
                }
            }
        }

        public static decimal WeekendRate
        {
            get {return weekendRate;}
        }

        public static decimal OvertimeRate
        {
            get {return overtimeRate; }
        }

        public Salary(decimal hourlyRate, int hoursWorked, int overtimeHours, int weekendHours, SalaryFeatures features)
        {
            this.hourlyRate = hourlyRate;
            this.hoursWorked = hoursWorked;
            this.overtimeHours = overtimeHours;
            this.weekendHours = weekendHours;
            this.features = features;
        }

        public decimal CountAllSalary()
        {
            // base payment
            decimal salary = HourlyRate * HoursWorked;
            
            // overtime payment
            salary += (OvertimeRate * hourlyRate) * OvertimeHours;

            // weekend payment
            salary += (WeekendRate * hourlyRate) * WeekendHours;

            // bonus for various features
            if (features.HasFlag(SalaryFeatures.Graduate)) salary += graduateBonus;
            if (features.HasFlag(SalaryFeatures.Children)) salary += childrenBonus;
            if (features.HasFlag(SalaryFeatures.Disability)) salary += disabilityBonus;

            return salary;
        }

        public override string ToString()
        {
            return " Salary: " + CountAllSalary().ToString("C2");
        }

        public decimal OffTimeSalary()
        {
            return overtimeHours * (hourlyRate * overtimeRate) + weekendHours * (hourlyRate * weekendRate);
        }

    }

    public static class SalaryHelper
    {
        public static decimal OffTimeSalary(this Salary salary)
        {
            return salary.OffTimeSalary();
        }
    }
}
