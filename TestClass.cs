using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PayrollSystem
{
    /// <summary>
    /// Employees in file should be represented as follows (1 per line):
    /// ID name gender departmentID email hourlyRate hoursWorked overtimeHours weekendHours featuresOfEmployee
    /// </summary>
    class TestClass
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // read employees from file
            string fileName = @"C:\Users\Martynas\Documents\Visual Studio 2015\Projects\PayrollSystem\Employees.txt";
            List<Employee> employeesList = GetEmployeesFromFile(fileName);

            // command-line interface
            Console.WriteLine("Sveiki, atvykę į atlyginimų mokėjimo sistemą!");
            int input;
            bool run = true;
            while (run)
            {
                Console.WriteLine("Pasirinkite veiksmą:\n1.Išvesti visus darbuotojus\n2.Surasti darbuotoją pagal vardą\n3.Suriušiuoti darbuotojus pagal kriterijų\n4.Gauti darbuotojo viršvalandžių atlyginimą\n5.Sugrupuoti pagal departamentą\n6.Baigti darbą");
                int.TryParse(Console.ReadLine(), out input);
                MenuChoice(input, ref run, employeesList);
            }
        }

        public static List<Employee> GetEmployeesFromFile(string fileName)
        {
            List<Employee> employeesList = new List<Employee>();

            string[] allLines = System.IO.File.ReadAllLines(fileName);
            foreach (string emp in allLines)
            {
                employeesList.Add(CreateEmployee(emp));
            }
            return employeesList;
        }

        public static Employee CreateEmployee(string line)
        {
            string[] fields = line.Split(' ');
            int ID = int.Parse(fields[0]);
            int departmentId;
            int.TryParse(fields[3], out departmentId);
            decimal rate;
            decimal.TryParse(fields[5], out rate);
            int hours;
            int.TryParse(fields[6], out hours);
            int oHours;
            int.TryParse(fields[7], out oHours);
            int wHours;
            int.TryParse(fields[8], out wHours);
            SalaryFeatures features = GetFeaturesByString(line);
            return new Employee(ID, fields[1], fields[2], departmentId, salary: new Salary(rate, hours, oHours, wHours, features), Email: fields[4]);
        }

        public static SalaryFeatures GetFeaturesByString(string fields)
        {
            SalaryFeatures features = SalaryFeatures.None;

            if (fields.IndexOf("children", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Children;
            if (fields.IndexOf("graduate", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Graduate;
            if (fields.IndexOf("disability", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Disability;

            return features;
        }

        public static void MenuChoice(int input, ref bool run, List<Employee> employeesList)
        {
            switch (input)
            {
                case 1:
                    PrintEmployees(employeesList);
                    break;
                case 2:
                    FindByNameAndPrint(employeesList);
                    break;
                case 3:
                    SortEmployees(employeesList);
                    break;
                case 4:
                    GetOfftimeSalary(employeesList);
                    break;
                case 5:
                    GroupEmployeesByDepartment(employeesList);
                    break;
                case 6:
                    run = false;
                    break;
                default:
                    Console.WriteLine("Nepavyko apdoroti pasirinkimo, bandykite iš naujo");
                    break;
            }
        }

        public static void PrintEmployees(IEnumerable<Employee> List)
        {
            foreach (Employee emp in List)
            {
                Console.WriteLine(emp);
            }
        }

        public static void FindByNameAndPrint(List<Employee> employeesList)
        {
            Console.WriteLine("Įveskite darbuotojo vardą:");
            string name = Console.ReadLine();
            IEnumerable<Employee> exactEmp = from Employee in employeesList
                                             where Employee.Name.ToLower().Contains(name.ToLower())
                                             select Employee;
            PrintEmployees(exactEmp);
            if (!exactEmp.Any()) Console.WriteLine("Darbuotojas neegzistuoja arba įvestas neteisingas vardas");
        }

        public static void SortEmployees(List<Employee> employeesList)
        {
            Console.WriteLine("Pasirinkite rūšiavimo kriterijų:");
            Console.WriteLine("1.Vardas 2.ID 3. Atlyginimas 4.Viršvalandžiai");
            int sort = int.Parse(Console.ReadLine());
            SmartEmployeeComparer scc = new SmartEmployeeComparer();
            switch (sort)
            {
                case 1:
                    employeesList.Sort();
                    break;
                case 2:
                    scc.SortBy = SmartEmployeeComparer.CompareField.Id;
                    employeesList.Sort(scc);
                    break;
                case 3:
                    scc.SortBy = SmartEmployeeComparer.CompareField.Salary;
                    employeesList.Sort(scc);
                    break;
                case 4:
                    scc.SortBy = SmartEmployeeComparer.CompareField.Overtime;
                    employeesList.Sort(scc);
                    break;
                default:
                    Console.WriteLine("Nepavyko apdoroti pasirinkimo, bandykite iš naujo");
                    break;
            }
        }

        public static void GetOfftimeSalary(List<Employee> employeesList)
        {
            Console.WriteLine("Įveskite darbuotojo vardą:");
            string empName = Console.ReadLine();
            IEnumerable<Employee> exactemployee = from Employee in employeesList
                                           where Employee.Name.ToLower().Contains(empName.ToLower())
                                           select Employee;
            if (exactemployee.Any())
            {
                foreach (Employee emp in exactemployee)
                {
                    Console.WriteLine(emp.Name + " atlyginimo dalis, gauta dirbant viršvalandžius: " + emp.Salary.OffTimeSalary().ToString("C2"));
                }
            }
            else
            {
                Console.WriteLine("Darbuotojas neegzistuoja arba ivestas neteisingas vardas");
            }
        }

        public static void GroupEmployeesByDepartment(List<Employee> employeesList)
        {
            var employeesByDepartment = Department.GetAllDepartments()
                                                           .GroupJoin(employeesList,
                                                           d => d.ID,
                                                           e => e._departmentID,
                                                           (department, employees) => new
                                                           {
                                                               Department = department,
                                                               Employees = employees
                                                           });

            foreach (var department in employeesByDepartment)
            {
                decimal salarySum = 0;
                int overtimeSum = 0;
                int i = 0;

                Console.WriteLine(department.Department.Name);
                foreach (var emp in department.Employees)
                {
                    Console.WriteLine(" " + emp);
                    salarySum += emp.Salary.CountAllSalary();
                    overtimeSum += emp.Salary.OvertimeHours;
                    i++;
                }
                if (i == 0) Console.WriteLine("Šiame departamente nerastas nei vienas darbuotojas");
                else
                {
                    Console.WriteLine("\t\t\t\tOvertime sum: {0}, Salary sum: {1} ", overtimeSum, salarySum.ToString("C2"));
                    Console.WriteLine("\t\t\t\tOvertime avg: {0}, Salary avg: {1} ", overtimeSum / i, (salarySum / i).ToString("C2"));
                }
                Console.WriteLine();
            }
        }
    }
}
