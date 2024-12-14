using System.Security.Cryptography;

namespace BankingApp.Services.Account.Utils
{
    public static class AccountNumberGenerator
    {
        private const int AccountNumberLength = 10;

        public static string Generate()
        {
            if (AccountNumberLength <= 0)
            {
                throw new ArgumentException("Length must be a positive integer.", nameof(AccountNumberLength));
            }

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[AccountNumberLength - 1];
                rng.GetBytes(randomBytes);

                string accountNumber = string.Empty;
                foreach (byte b in randomBytes)
                {
                    // Convert each byte to a digit (0-9)
                    accountNumber += (b % 10).ToString();
                }

                // Add a Luhn checksum digit
                int checksumDigit = CalculateLuhnChecksum(accountNumber);
                accountNumber += checksumDigit;

                return accountNumber;
            }
        }

        private static int CalculateLuhnChecksum(string number)
        {
            int sum = 0;
            bool doubleDigit = true;

            // Traverse the number from right to left
            for (int i = number.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(number[i].ToString());

                if (doubleDigit)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
                doubleDigit = !doubleDigit;
            }

            // The checksum digit is the number that makes the sum a multiple of 10
            int checksum = (10 - (sum % 10)) % 10;
            return checksum;
        }
    }
}
