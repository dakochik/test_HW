using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HW
{
    delegate void ErrorNotificationType(string message);

    class Calculator
    {
        delegate double MathOperation(double a, double b);

        static Dictionary<string, MathOperation> operations = new Dictionary<string, MathOperation> { { "+", (a, b) => checked(a + b) }, { "-", (a, b) => checked(a - b) }, { "*", (a, b) => checked(a * b) }, { "/", (a, b) => a / b }, { "^", (a, b) => checked(Math.Pow(a, b)) } };

        static public event ErrorNotificationType ErrorNotification;

        public static string Calculate(string expr) //string, чтобы иметь возможность сравнить результат с файлом.
        {
            string[] newExpr = expr.Split(' ');
            string result = "";
            try
            {
                if (newExpr[1] == "/" && newExpr[2] == "0")
                {
                    throw new DivideByZeroException();//Иначе он думает, что ноль, это не ноль, а почти ноль и получает бесконечности.
                }
                else
                {
                    result += operations[newExpr[1]](double.Parse(newExpr[0]), double.Parse(newExpr[2]));
                }
            }
            catch (Exception e)
            {
                ErrorNotification(e.Message);
                result += e.Message;
            }
            return result;
        }
    }

    class Program
    {

        const string appendHere = "result.txt";//Текстовый документ для ввода отчета.
        const string readThis = "expressions.txt";//Текстовый документ, из которого читаются решения.
        const string checkRes = "expressions_checker.txt";//Текстовый документ, с которым сравниваются ответы.

        static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine(message + $" {DateTime.Now}");
        }

        static void ResultErrorHandler(string message)
        {
            File.AppendAllText(appendHere, JustMeth(message) + Environment.NewLine);
        }

        static string JustMeth(string message)
        {
            if (message == "Попытка деления на нуль.")
            {
                return "bruh";
            }
            else if (message == "не число")
            {
                return "не число";
            }
            else if (message == "Данный ключ отсутствует в словаре.")
            {
                return "неверный оператор";
            }
            else
            {
                return message;
            }
        }

        static void Main(string[] args)
        {
            Calculator.ErrorNotification += ConsoleErrorHandler;
            Calculator.ErrorNotification += ResultErrorHandler;
            string[] checkTHis = File.ReadAllLines(readThis);
            string[] compareTHis = File.ReadAllLines(checkRes);
            File.WriteAllText(appendHere, "");
            string res = "";
            double resultD;
            int numOfProb = 0;
            for (int i = 0; i < checkTHis.Length; ++i)
            {
                res = Calculator.Calculate(checkTHis[i]);
                //Console.Write(res);
                if (double.TryParse(res, out resultD))
                {
                    if ($"{resultD:F3}" != compareTHis[i])
                    {
                        Console.WriteLine($"Problem №{i + 1}:\t --> {resultD:F3}!={compareTHis[i]}");
                    }
                }
                else
                {
                    ++numOfProb;
                    if (JustMeth(res) != compareTHis[i])
                    {
                        Console.WriteLine($"Problem №{i + 1}:\t --> {JustMeth(res)}!={compareTHis[i]}");
                    }
                }
            }
            Console.WriteLine("Total problems: {0}", numOfProb);
        }
    }
}
