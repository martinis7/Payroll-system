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
            int input;
            Console.WriteLine("Sveiki, atvykę į atlyginimų mokėjimo sistemą!");
            do
            {
                Console.WriteLine("Pasirinkite veiksmą:\n1.Išvesti visus darbuotojus\n2.Surasti darbuotoją pagal vardą\n3.Suriušiuoti darbuotojus pagal kriterijų\n4.Gauti darbuotojo viršvalandžių atlyginimą\n5.Sugrupuoti pagal departamentą\n6.Baigti darbą");
                int.TryParse(Console.ReadLine(), out input);
            } while (MenuChoice(input, employeesList));
        }

        public static List<Employee> GetEmployeesFromFile(string fileName)
        {
            List<Employee> employeesList = new List<Employee>();

            string[] allLines = System.IO.File.ReadAllLines(fileName);
            foreach (string emp in allLines)
            {
                Employee newEmp = new Employee();
                if (TryParseEmployee(emp, out newEmp)) employeesList.Add(newEmp);
            }
            return employeesList;
        }

        public static bool TryParseEmployee(string line, out Employee employee)
        {
            string[] fields = line.Split(' ');
            string errorMsg = " yra netinkamo formato. Darbuotojas nebus sukurtas.";
            if (fields.Length < 9)
            {
                employee = new Employee();
                Console.WriteLine("Pateikta informacija" + errorMsg);
                return false;
            }          
            bool parseCondition = true;
            int ID;
            int departmentId;
            decimal rate;
            int hours;
            int oHours;
            int wHours;

            if (!int.TryParse(fields[0], out ID))
            {
                Console.WriteLine("Darbuotojo ID" + errorMsg);
                parseCondition = false;
            }
            if (!int.TryParse(fields[3], out departmentId))
            {
                Console.WriteLine("Departamento ID" + errorMsg);
                parseCondition = false;
            }
            if (!decimal.TryParse(fields[5], out rate))
            {
                Console.WriteLine("Valandinis koeficientas" + errorMsg);
                parseCondition = false;
            }
            if (!int.TryParse(fields[6], out hours))
            {
                Console.WriteLine("Pradirbtos valandos" + errorMsg);
                parseCondition = false;
            }
            if (!int.TryParse(fields[7], out oHours))
            {
                Console.WriteLine("Viršvalandžiai" + errorMsg);
                parseCondition = false;
            }
            if (!int.TryParse(fields[8], out wHours))
            {
                Console.WriteLine("Išeiginių valandos" + errorMsg);
                parseCondition = false;
            }
            WorkFeatures features = ParseFeatures(line);
            employee =  new Employee(ID, fields[1], fields[2], departmentId, salary: new WorkHours(rate, hours, oHours, wHours, features), Email: fields[4]);
            return parseCondition;
        }

        public static WorkFeatures ParseFeatures(string fields)
        {
            WorkFeatures features = WorkFeatures.None;

            if (fields.IndexOf("children", StringComparison.OrdinalIgnoreCase) >= 0) features |= WorkFeatures.Children;
            if (fields.IndexOf("graduate", StringComparison.OrdinalIgnoreCase) >= 0) features |= WorkFeatures.Graduate;
            if (fields.IndexOf("disability", StringComparison.OrdinalIgnoreCase) >= 0) features |= WorkFeatures.Disability;

            return features;
        }

        public static bool MenuChoice(int input, List<Employee> employeesList)
        {
            switch (input)
            {
                case 1:
                    PrintEmployees(employeesList);
                    return true;
                case 2:
                    FindByNameAndPrint(employeesList);
                    return true;
                case 3:
                    SortEmployees(employeesList);
                    return true;
                case 4:
                    GetOfftimeSalary(employeesList);
                    return true;
                case 5:
                    GroupEmployeesByDepartment(employeesList);
                    return true;
                case 6:
                    return false;
                default:
                    Console.WriteLine("Nepavyko apdoroti pasirinkimo, bandykite iš naujo");
                    return true;
            }
        }

        public static void PrintEmployees(IEnumerable<Employee> List)
        {
            if (!List.Any()) Console.WriteLine("Darbuotojų sąrašas tuščias");
            else
            {
                foreach (Employee emp in List)
                {
                    Console.WriteLine(emp);
                }
            }
        }

        public static IEnumerable<Employee> FindByname(List<Employee> employeesList)
        {
            Console.WriteLine("Įveskite darbuotojo vardą ar jo dalį:");
            string name = Console.ReadLine();
            IEnumerable<Employee> exactEmployee = from Employee in employeesList
                                             where Employee._name.ToLower().Contains(name.ToLower())
                                             select Employee;
            return exactEmployee;
        }

        public static void FindByNameAndPrint(List<Employee> employeesList)
        {
           
            PrintEmployees(FindByname(employeesList));
        }

        public static void SortEmployees(List<Employee> employeesList)
        {
            Console.WriteLine("Pasirinkite rūšiavimo kriterijų:");
            Console.WriteLine("1.Vardas 2.ID 3. Atlyginimas 4.Viršvalandžiai");
            int sort;
            int.TryParse(Console.ReadLine(), out sort);
            if (GetComparer(sort) != null) employeesList.Sort(GetComparer(sort));
            else Console.WriteLine("Nepavyko apdoroti pasirinkimo, bandykite iš naujo.");
        }

        public static void GetOfftimeSalary(List<Employee> employeesList)
        {
                foreach (Employee emp in FindByname(employeesList))
                {
                    Console.WriteLine(emp._name + " atlyginimo dalis, gauta dirbant viršvalandžius: " + emp._salary.OffTimeSalary().ToString("C2"));
                }
        }

        public static IComparer<Employee> GetComparer(int choice)
        {
            switch (choice)
            {
                case 1:
                    return new CompareByName();
                case 2:
                    return new CompareByID();
                case 3:
                    return new CompareBySalary();
                case 4:
                    return new CompareByOvertime();
                default:
                    return null;
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
                Console.WriteLine(department.Department.Name);
                SalaryReport salarySum = new SalaryReport(0, 0, 0);
                foreach (var emp in department.Employees)
                {
                    Console.WriteLine(" " + emp);
                    salarySum = salarySum.Add(emp._salary.CountAllSalary(), emp._salary.OvertimeHours);
                }

                if (salarySum.count != 0)
                {
                    Console.WriteLine("\t\t\t\tOvertime sum: {0}, Salary sum: {1} ", salarySum.overtimeSum, salarySum.salarySum.ToString("C2"));
                    Console.WriteLine("\t\t\t\tOvertime avg: {0}, Salary avg: {1} ", salarySum.overtimeSum / salarySum.count, (salarySum.salarySum / salarySum.count).ToString("C2"));
                }
                Console.WriteLine();
            }
        }
    }
}