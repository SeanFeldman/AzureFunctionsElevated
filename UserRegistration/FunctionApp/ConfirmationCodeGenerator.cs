using System;
using System.Linq;

namespace FunctionApp
{
    public static class ConfirmationCodeGenerator
    {
        private static readonly Random random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate(int length = 4) =>
            new string(Enumerable.Repeat(Chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}