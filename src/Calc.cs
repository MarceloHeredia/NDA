using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NDA
{
    public class Calc
    {
        public static async Task<Double> Pow(double value, double pow)
        {
            var result = await Task.FromResult(Math.Pow(value, pow));
            return result;
        }

        public static async Task<Double> Mult(double x, double y)
        {
            var result = await Task.FromResult(x * y);
            return result;
        }

        public static async Task<Double> Div(double x, double y)
        {
            var result = await Task.FromResult(x / y);
            return result;
        }

        public static async Task<Double> Remainder(double x, double y)
        {
            var result = await Task.FromResult(x % y);
            return result;
        }

        public static async Task<Double> Sum(double x, double y)
        {
            var result = await Task.FromResult(x + y);
            return result;
        }
        public static async Task<Double> Sub(double x, double y)
        {
            var result = await Task.FromResult(x - y);
            return result;
        }

        public static async Task<Int32> Fibo(int position)
        {
            var result = await Task.FromResult(Fibonacci(position));
            return result;
        }

        private static int Fibonacci(int num)
        {
            if (num == 0) return 0;
            if (num == 1) return 1;

            return Fibonacci(num - 1) + Fibonacci(num - 2);
        }
    }
}
