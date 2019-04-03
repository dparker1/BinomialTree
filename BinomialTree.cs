using System;
using System.IO;

public class BinomialTree
{
    public static void Main(String[] args)
    {
        string[] input = File.ReadAllLines(@"BinomialInput.txt");
        double q = GetVal(input, 1);
        double r = GetVal(input, 0);
        double sig = GetVal(input, 2);
        double S0 = GetVal(input, 3);
        double K = GetVal(input, 4);
        double T = GetVal(input, 6);
        bool put = (int)GetVal(input, 5) == 1;
        int periods = (int)GetVal(input, 7);

        double dt = T / periods;
        double u = Math.Exp(sig * Math.Sqrt(dt));
        double d = Math.Exp(-sig * Math.Sqrt(dt));
        double p = (Math.Exp((r - q) * dt) - d) / (u - d);
        double disc = Math.Exp(-r * dt);

        Console.WriteLine("\n");
        Console.WriteLine("Discount: " + disc);
        Console.WriteLine("Probability Up: " + p);
        Console.WriteLine("u: " + u);
        Console.WriteLine("d: " + d);
        Console.WriteLine();

        double[,,] prices = new double[periods + 1, periods + 1, 3];
        prices[0, 0, 0] = S0;
        for(int t = 1; t <= periods; t++)
        {
            for(int i = 0; i < t; i++)
            {
                prices[i, t, 0] = prices[i, t - 1, 0] * u;
            }
            prices[t, t, 0] = prices[t - 1, t - 1, 0] * d;
        }

        for(int i = 0; i <= periods; i++)
        {
            prices[i, periods, 1] = put ? Math.Max(0, K - prices[i, periods, 0]) : Math.Max(0, prices[i, periods, 0] - K);
            if(prices[i, periods, 1] > 0)
            {
                prices[i, periods, 2] = 1;
            }
        }

        for(int t = periods - 1; t >= 0; t--)
        {
            for(int i = 0; i <= t; i++)
            {
                double exerNow = put ? Math.Max(0, K - prices[i, t, 0]) : Math.Max(0, prices[i, t, 0] - K);
                double exerLater = disc * (p * prices[i, t + 1, 1] + (1-p) * prices[i + 1, t + 1, 1]);
                prices[i, t, 1] = exerNow > exerLater ? exerNow : exerLater;
                prices[i, t, 2] = exerNow > exerLater ? 1 : 0;
            }
        }

        for(int i = 0; i <= periods; i++)
        {
            for(int t = 0; t < i; t++)
            {
                Console.Write("XXXXXXXXXXXX | ");
            }

            for(int t = i; t <= periods; t++)
            {
                if(prices[i, t, 2] > 0)
                {
                    Console.Write("*" + String.Format("{0,5:F2}", prices[i, t, 0]) + String.Format("{0,6:F2}", prices[i, t, 1]) + " | ");
                }
                else
                {
                    Console.Write(String.Format("{0,6:F2}", prices[i, t, 0]) + String.Format("{0,6:F2}", prices[i, t, 1]) + " | ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private static double GetVal(string[] values, int i)
    {
        return Double.Parse(values[i].Substring(values[i].IndexOf(':') + 1));
    }
}