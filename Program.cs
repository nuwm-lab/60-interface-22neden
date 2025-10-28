using System;
using System.Globalization;

namespace Lab10_AbstractsAndInterfaces
{
    /// <summary>
    /// Маленька константа для порівняння чисел з плаваючою крапкою.
    /// </summary>
    public static class Numeric
    {
        public const double Epsilon = 1e-12;
    }

    /// <summary>
    /// Інтерфейс дробу — лише операція обчислення і рядок-представлення.
    /// (Рекомендація: уникати вводу/виводу всередині моделей.)
    /// </summary>
    public interface IFraction
    {
        double Calculate(double x);
        string Description { get; }
    }

    /// <summary>
    /// Простий дріб: 1 / (a * x)
    /// </summary>
    public class SimpleFraction : IFraction
    {
        public double A { get; }

        public string Description => $"Простий дріб: 1/({A} * x)";

        public SimpleFraction(double a)
        {
            ValidateCoefficient(a, nameof(a));
            A = a;
        }

        public double Calculate(double x)
        {
            double denom = A * x;
            if (Math.Abs(denom) <= Numeric.Epsilon)
                throw new DivideByZeroException("Обчислення привело до ділення на нуль (a * x близьке до нуля).");
            return 1.0 / denom;
        }

        private static void ValidateCoefficient(double value, string name)
        {
            if (Math.Abs(value - 3.0) <= Numeric.Epsilon)
                throw new ArgumentException($"{name} не може дорівнювати 3.", name);
        }
    }

    /// <summary>
    /// Зверху-до-низу вкладений дріб: 1/(a1*x + 1/(a2*x + 1/(a3*x)))
    /// </summary>
    public class ContinuedFraction : IFraction
    {
        public double A1 { get; }
        public double A2 { get; }
        public double A3 { get; }

        public string Description => $"1/({A1}*x + 1/({A2}*x + 1/({A3}*x)))";

        public ContinuedFraction(double a1, double a2, double a3)
        {
            ValidateCoefficient(a1, nameof(a1));
            ValidateCoefficient(a2, nameof(a2));
            ValidateCoefficient(a3, nameof(a3));
            A1 = a1; A2 = a2; A3 = a3;
        }

        public double Calculate(double x)
        {
            double inner = A3 * x;
            if (Math.Abs(inner) <= Numeric.Epsilon)
                throw new DivideByZeroException("Ділення на нуль у внутрішньому шарі (a3 * x близьке до нуля).");

            double mid = A2 * x + 1.0 / inner;
            if (Math.Abs(mid) <= Numeric.Epsilon)
                throw new DivideByZeroException("Ділення на нуль у середньому шарі (a2 * x + 1/(a3 * x) близьке до нуля).");

            double outer = A1 * x + 1.0 / mid;
            if (Math.Abs(outer) <= Numeric.Epsilon)
                throw new DivideByZeroException("Ділення на нуль у зовнішньому шарі (a1 * x + 1/(...) близьке до нуля).");

            return 1.0 / outer;
        }

        private static void ValidateCoefficient(double value, string name)
        {
            if (Math.Abs(value - 3.0) <= Numeric.Epsilon)
                throw new ArgumentException($"{name} не може дорівнювати 3.", name);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Демонстрація роботи дробів ===");
            try
            {
                double a = ReadDouble("Введіть коефіцієнт a (не 3): ", v => Math.Abs(v - 3.0) > Numeric.Epsilon, "a не повинен дорівнювати 3.");
                var simple = new SimpleFraction(a);
                Console.WriteLine(simple.Description);

                double x = ReadDouble("x = ", v => Math.Abs(v) > Numeric.Epsilon, "x не може бути нулем.");
                Console.WriteLine($"Результат простого дробу: {simple.Calculate(x).ToString(CultureInfo.CurrentCulture)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== Продовжуємо з вкладеним дробом ===");
            try
            {
                double a1 = ReadDouble("a1 (не 3): ", v => Math.Abs(v - 3.0) > Numeric.Epsilon, "a1 не повинен дорівнювати 3.");
                double a2 = ReadDouble("a2 (не 3): ", v => Math.Abs(v - 3.0) > Numeric.Epsilon, "a2 не повинен дорівнювати 3.");
                double a3 = ReadDouble("a3 (не 3): ", v => Math.Abs(v - 3.0) > Numeric.Epsilon, "a3 не повинен дорівнювати 3.");

                var cont = new ContinuedFraction(a1, a2, a3);
                Console.WriteLine(cont.Description);

                double x2 = ReadDouble("x = ", v => Math.Abs(v) > 0.0, "Невірне значення x.");
                Console.WriteLine($"Результат вкладеного дробу: {cont.Calculate(x2).ToString(CultureInfo.CurrentCulture)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nГотово.");
        }

        private static double ReadDouble(string prompt, Func<double, bool> validator, string invalidMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double value))
                {
                    if (validator == null || validator(value))
                        return value;
                    Console.WriteLine($"Невірне значення: {invalidMessage}");
                }
                else
                {
                    Console.WriteLine("Неправильний формат числа. Спробуйте ще раз.");
                }
            }
        }
    }
}
