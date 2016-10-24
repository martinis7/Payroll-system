using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PayrollSystem
{

    public class Employee : IComparable<Employee>
    {
        private const int IdLength = 5;
        private int _id;
        private string _name;
        private string _gender { get; set; }
        private string _email;
        public int _departmentID { get; set; }
        private Salary _salary;

        Regex emailRegex = new Regex(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
		                        		[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
		                        		[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                        + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$");

        public int Id
        {
            get { return _id; }
            set
            {
                if (value.ToString().Length != IdLength)
                {
                    throw new Exception("Invalid ID length");
                }
                this._id = value;
            }
        }

        public String Email
        {
            get { return _email; }
            set
            {
                if (!emailRegex.IsMatch(value))
                {
                    throw new Exception("Invalid Email");
                }
                _email = value;
            }
        }

        internal Salary Salary
        {
            get { return _salary; }
            set { _salary = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Employee(int ID, string Name, string Gender, int departmentID, Salary salary, string Email = "abc@abc.lt")
        {
            this.Id = ID;
            _name = Name;
            _gender = Gender;
            _departmentID = departmentID;
            this.Email = Email;
            _salary = salary;
        }

        public override string ToString()
        {
            return "ID: " + _id + ", Name: " + _name + ", Gender: " + _gender + ", Overtime: " + (this._salary.OvertimeHours) + Salary.ToString();
        }

        int IComparable<Employee>.CompareTo(Employee other)
        {
            return this._name.CompareTo(other._name);
        }
    }
}
