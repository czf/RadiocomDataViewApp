using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiocomDataViewApp.Clients.Mocks
{
    internal static class MockUtils
    {
        private static readonly Random random = new Random();
        internal static string RandomString(int minLength, int maxLength)//https://stackoverflow.com/a/1344258
        {
            int length = random.Next(minLength, maxLength);
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz     0123456789";
            char[] resultArray = new char[length];
            for (int a = 0; a < length; a++)
            {
                resultArray[a] = chars[random.Next(chars.Length)];
                if (a == 0 && resultArray[a] == ' ')
                {
                    a--;
                }
            }
            return new String(resultArray);
        }
    }
}
