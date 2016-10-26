using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PayrollSystem
{

    public class Employee
    {
        private const int IdLength = 5;
        private int _id;
        public string _name { get; set; }
        private string _gender { get; set; }
        private string _email;
        public int _departmentID { get; set; }
        public WorkHours _salary { get; set; }

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

        public Employee(int ID, string Name, string Gender, int departmentID, WorkHours salary, string Email = "abc@abc.lt")
        {
            this.Id = ID;
            _name = Name;
            _gender = Gender;
            _departmentID = departmentID;
            _salary = salary;
            this.Email = Email;
        }

        public Employee()
        {

        }

        public override string ToString()
        {
            return "ID: " + _id + ", Name: " + _name + ", Gender: " + _gender + ", Overtime: " + (_salary.OvertimeHours) + _salary.ToString();
        }
    }
}