using System;

namespace Lab10_AbstractsAndInterfaces
{
    public interface IFraction
    {
        void SetCoefficients();
        void Show();
        double Calculate(double x);
    }

    // Абстрактний базовий клас
    public abstract class AbstractFraction : IFraction
    {
        public abstract void SetCoefficients();
        public abstract void Show();
        public abstract double Calculate(double x);

        // Деструктор
        ~AbstractFraction()
        {
            Console.WriteLine("Об'єкт знищено.");
        }
    }

    // Простий дріб 1/(a*x)
    public class SimpleFraction : AbstractFraction
    {
        private double _a;

        public SimpleFraction() { }
        public SimpleFraction(double a) => _a = a;

        public override void SetCoefficients()
        {
            Console.Write("Введіть коефіцієнт a: ");
            _a = double.Parse(Console.ReadLine());
        }

        public override void Show()
        {
            Console.WriteLine($"Простий дріб: 1/({_a}*x)");
        }

        public override double Calculate(double x)
        {
            if (Math.Abs(_a * x) < 1e-12)
                throw new DivideByZeroException("Ділення на нуль!");
            return 1 / (_a * x);
        }
    }

    // Продовжений дріб 1/(a1*x + 1/(a2*x + 1/(a3*x)))
    public class ContinuedFraction : AbstractFraction
    {
        private double _a1, _a2, _a3;

        public ContinuedFraction() { }
        public ContinuedFraction(double a1, double a2, double a3)
        {
            _a1 = a1; _a2 = a2; _a3 = a3;
        }

        public override void SetCoefficients()
        {
            Console.Write("Введіть a1: "); _a1 = double.Parse(Console.ReadLine());
            Console.Write("Введіть a2: "); _a2 = double.Parse(Console.ReadLine());
            Console.Write("Введіть a3: "); _a3 = double.Parse(Console.ReadLine());
        }

        public override void Show()
        {
            Console.WriteLine($"Продовжений дріб: 1/({_a1}*x + 1/({_a2}*x + 1/({_a3}*x)))");
        }

        public override double Calculate(double x)
        {
            double inner = _a3 * x;
            if (Math.Abs(inner) < 1e-12) throw new DivideByZeroException();
            double mid = _a2 * x + 1 / inner;
            if (Math.Abs(mid) < 1e-12) throw new DivideByZeroException();
            double outer = _a1 * x + 1 / mid;
            if (Math.Abs(outer) < 1e-12) throw new DivideByZeroException();

            return 1 / outer;
        }
    }

    class Program
    {
        static void Main()
        {
            IFraction fraction1 = new SimpleFraction();
            IFraction fraction2 = new ContinuedFraction();

            Console.WriteLine("=== Простий дріб ===");
            fraction1.SetCoefficients();
            fraction1.Show();
            Console.Write("x = ");
            double x = double.Parse(Console.ReadLine());
            Console.WriteLine($"Результат: {fraction1.Calculate(x)}");

            Console.WriteLine("\n=== Продовжений дріб ===");
            fraction2.SetCoefficients();
            fraction2.Show();
            Console.Write("x = ");
            x = double.Parse(Console.ReadLine());
            Console.WriteLine($"Результат: {fraction2.Calculate(x)}");
        }
    }
}
