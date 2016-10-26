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
                employeesList.Add(ParseEmployee(emp));
            }
            return employeesList;
        }

        public static Employee ParseEmployee(string line)
        {
            string[] fields = line.Split(' ');
            string errorMsg = " yra netinkamo formato. Skaičiavimai gali būti nekorektiški";
            int ID;
            int departmentId;
            decimal rate;
            int hours;
            int oHours;
            int wHours;

            int.TryParse(fields[0], out ID);
            if (!int.TryParse(fields[3], out departmentId))
            {
                Console.WriteLine("Departamento ID" + errorMsg);
            }
            if (!decimal.TryParse(fields[5], out rate))
            {
                Console.WriteLine("Valandinis koeficientas" + errorMsg);
            }
            if (!int.TryParse(fields[6], out hours))
            {
                Console.WriteLine("Pradirbtos valandos" + errorMsg);
            }
            if (!int.TryParse(fields[7], out oHours))
            {
                Console.WriteLine("Viršvalandžiai" + errorMsg);
            }
            if (!int.TryParse(fields[8], out wHours))
            {
                Console.WriteLine("Išeiginių valandos" + errorMsg);
            }
            SalaryFeatures features = ParseFeatures(line);
            return new Employee(ID, fields[1], fields[2], departmentId, salary: new WorkHours(rate, hours, oHours, wHours, features), Email: fields[4]);
        }

        public static SalaryFeatures ParseFeatures(string fields)
        {
            SalaryFeatures features = SalaryFeatures.None;

            if (fields.IndexOf("children", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Children;
            if (fields.IndexOf("graduate", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Graduate;
            if (fields.IndexOf("disability", StringComparison.OrdinalIgnoreCase) >= 0) features |= SalaryFeatures.Disability;

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

            switch (sort)
            {
                case 1:
                    employeesList.Sort();
                    break;
                case 2:
                    employeesList.Sort(new CompareByID());
                    break;
                case 3:
                    employeesList.Sort(new CompareBySalary());
                    break;
                case 4:
                    employeesList.Sort(new CompareByOvertime());
                    break;
                default:
                    Console.WriteLine("Nepavyko apdoroti pasirinkimo, bandykite iš naujo");
                    break;
            }
        }

        public static void GetOfftimeSalary(List<Employee> employeesList)
        {
                foreach (Employee emp in FindByname(employeesList))
                {
                    Console.WriteLine(emp._name + " atlyginimo dalis, gauta dirbant viršvalandžius: " + emp._salary.OffTimeSalary().ToString("C2"));
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
            //cia galima paprastinti koda. pvz, mes sumuojame cia 2 dalykus
            //padarom klase su 2 properciais, galime ja pvz pavadint SalaryReport
            //defininam jai metoda Add, kuris susumuoja this ir paduota kita SalaryReport
            //bei grazina susumuota. Per pratybas tuomet parodysiu kaip pakeist vidini foreach loopa 1 eilute 
            foreach (var department in employeesByDepartment)
            {
                decimal salarySum = 0;
                int overtimeSum = 0;
                int i = 0;
                Console.WriteLine(department.Department.Name);

                foreach (var emp in department.Employees)
                {
                    Console.WriteLine(" " + emp);
                    salarySum += emp._salary.CountAllSalary();
                    overtimeSum += emp._salary.OvertimeHours;
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