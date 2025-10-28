using System;
using System.Globalization;

namespace Lab10_AbstractsAndInterfaces
{
    public interface IFraction
    {
        void SetCoefficients(); // retained for compatibility (UI should set values via overloads)
        void Show();
        double Calculate(double x);
    }

    // Абстрактний базовий клас
    public abstract class AbstractFraction : IFraction
    {
        public abstract void SetCoefficients();
        public abstract void Show();
        public abstract double Calculate(double x);

        // Деструктор видалено — непередбачувана робота IO у фіналізаторах погана практика.
    }

    // Простий дріб 1/(a*x)
    public class SimpleFraction : AbstractFraction
    {
        private double _a;

        public double A
        {
            get => _a;
            private set
            {
                if (Math.Abs(value - 3.0) < 1e-12)
                    throw new ArgumentException("Коефіцієнт a не може дорівнювати 3 (обмеження умови).", nameof(value));
                _a = value;
            }
        }

        public SimpleFraction() { }
        public SimpleFraction(double a) => A = a;

        // Оригінальний інтерфейсний метод залишено, але він не читає з консолі — UI має викликати перевантаження
        public override void SetCoefficients()
        {
            throw new InvalidOperationException("SetCoefficients() без параметрів не підтримується для SimpleFraction. Використовуйте SetCoefficients(double a) з UI.");
        }

        // Метод для програмного встановлення коефіцієнтів
        public void SetCoefficients(double a) => A = a;

        public override void Show()
        {
            Console.WriteLine(ToString());
        }

        public override double Calculate(double x)
        {
            double denom = _a * x;
            if (Math.Abs(denom) < 1e-12)
                throw new DivideByZeroException("Обчислення привело до ділення на нуль: a * x близьке до нуля.");
            return 1.0 / denom;
        }

        public override string ToString()
        {
            return $"Простий дріб: 1/({_a} * x)";
        }
    }

    // "Тригонометричний / підхідний" дріб (в оригіналі названо ContinuedFraction) 
    // Тут реалізовано дробоподібну структуру 1/(a1*x + 1/(a2*x + 1/(a3*x)))
    public class TrigonometricApproachFraction : AbstractFraction
    {
        private double _a1, _a2, _a3;

        public double A1
        {
            get => _a1;
            private set
            {
                if (Math.Abs(value - 3.0) < 1e-12)
                    throw new ArgumentException("Коефіцієнт a1 не може дорівнювати 3 (обмеження умови).", nameof(value));
                _a1 = value;
            }
        }

        public double A2
        {
            get => _a2;
            private set
            {
                if (Math.Abs(value - 3.0) < 1e-12)
                    throw new ArgumentException("Коефіцієнт a2 не може дорівнювати 3 (обмеження умови).", nameof(value));
                _a2 = value;
            }
        }

        public double A3
        {
            get => _a3;
            private set
            {
                if (Math.Abs(value - 3.0) < 1e-12)
                    throw new ArgumentException("Коефіцієнт a3 не може дорівнювати 3 (обмеження умови).", nameof(value));
                _a3 = value;
            }
        }

        public TrigonometricApproachFraction() { }

        public TrigonometricApproachFraction(double a1, double a2, double a3)
        {
            SetCoefficients(a1, a2, a3);
        }

        public override void SetCoefficients()
        {
            throw new InvalidOperationException("SetCoefficients() без параметрів не підтримується для TrigonometricApproachFraction. Використовуйте SetCoefficients(double a1, double a2, double a3) з UI.");
        }

        public void SetCoefficients(double a1, double a2, double a3)
        {
            A1 = a1;
            A2 = a2;
            A3 = a3;
        }

        public override void Show()
        {
            Console.WriteLine(ToString());
        }

        public override double Calculate(double x)
        {
            double inner = _a3 * x;
            if (Math.Abs(inner) < 1e-12)
                throw new DivideByZeroException("Ділення на нуль в найбільш внутрішньому шарі: a3 * x близьке до нуля.");

            double mid = _a2 * x + 1.0 / inner;
            if (Math.Abs(mid) < 1e-12)
                throw new DivideByZeroException("Ділення на нуль в середньому шарі: a2 * x + 1/(a3 * x) близьке до нуля.");

            double outer = _a1 * x + 1.0 / mid;
            if (Math.Abs(outer) < 1e-12)
                throw new DivideByZeroException("Ділення на нуль в зовнішньому шарі: a1 * x + 1/(...) близьке до нуля.");

            return 1.0 / outer;
        }

        public override string ToString()
        {
            return $"Тригонометрично-підхідний дріб: 1/({_a1}*x + 1/({_a2}*x + 1/({_a3}*x)))";
        }
    }

    class Program
    {
        static void Main()
        {
            // Створюємо екземпляри (демонстрація поліморфізму)
            IFraction fraction1 = new SimpleFraction();
            IFraction fraction2 = new TrigonometricApproachFraction();

            Console.WriteLine("=== Простий дріб ===");
            // Зчитування з UI винесено сюди — класи моделі не звертаються до Console.
            double a = ReadDouble("Введіть коефіцієнт a (не 3): ", v => Math.Abs(v - 3.0) > 1e-12, "a не повинен дорівнювати 3.");
            // Викликаємо метод класу, який приймає параметри
            ((SimpleFraction)fraction1).SetCoefficients(a);
            fraction1.Show();

            double x = ReadDouble("x = ", v => Math.Abs(v) > 0.0, "x не може бути нулем (уточнення: це допомагає уникнути ділення на нуль при обчисленнях).");
            try
            {
                Console.WriteLine($"Результат: {fraction1.Calculate(x).ToString(CultureInfo.CurrentCulture)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при обчисленні: {ex.Message}");
            }

            Console.WriteLine("\n=== Тригонометрично-підхідний дріб ===");
            double a1 = ReadDouble("Введіть a1 (не 3): ", v => Math.Abs(v - 3.0) > 1e-12, "a1 не повинен дорівнювати 3.");
            double a2 = ReadDouble("Введіть a2 (не 3): ", v => Math.Abs(v - 3.0) > 1e-12, "a2 не повинен дорівнювати 3.");
            double a3 = ReadDouble("Введіть a3 (не 3): ", v => Math.Abs(v - 3.0) > 1e-12, "a3 не повинен дорівнювати 3.");

            ((TrigonometricApproachFraction)fraction2).SetCoefficients(a1, a2, a3);
            fraction2.Show();

            x = ReadDouble("x = ", v => true, "Неправильне значення x.");
            try
            {
                Console.WriteLine($"Результат: {fraction2.Calculate(x).ToString(CultureInfo.CurrentCulture)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при обчисленні: {ex.Message}");
            }

            Console.WriteLine("\nГотово.");
        }

        // Утиліта для безпечного зчитування double з перевіркою
        private static double ReadDouble(string prompt, Predicate<double> validator, string invalidMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out double value))
                {
                    try
                    {
                        if (validator(value))
                            return value;
                        Console.WriteLine($"Невірне значення: {invalidMessage}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка валідації: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Неправильний формат числа. Спробуйте ще раз.");
                }
            }
        }
    }
}
