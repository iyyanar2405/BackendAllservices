using System;
using System.Security.Cryptography;

namespace AuthProvider.Helpers
{
    public static class GeneratePassCodes
    {
        /// <summary>
        /// Generates a temporary password with the specified length, minimum letter length, minimum special character length, and minimum numeric length.
        /// </summary>
        /// <returns>A randomly generated temporary password.</returns>
        public static string GenerateTemporaryPassword(int passwordLength, int minLetterLength, int minSpecialLength, int minNumericsLength)
        {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTWXYZ";
            string specials = "!@#$%^&*()_-+=[{]};:<>|./?";
            string number = "0123456789";
            string generated = "";
            Random random = new Random();

            try
            {
                //Get total length of string
                int totalLength = (minLetterLength + minSpecialLength + minNumericsLength);

                //Check total length is greater than password length or not
                if (passwordLength < totalLength)
                    throw new Exception("passwordLength must be equal to or greater than (minLetterLength + minSpecialLength + minNumericsLength)");

                //Get remaining count.
                int remainingLetterCounts = passwordLength - totalLength;

                //Add remaining count in letters count.
                minLetterLength += remainingLetterCounts;

                //Generate Temporary password as per length
                for (int i = 1; i <= minLetterLength; i++)
                    generated = generated.Insert(random.Next(generated.Length), letters[random.Next(letters.Length - 1)].ToString());

                for (int i = 1; i <= minSpecialLength; i++)
                    generated = generated.Insert(random.Next(generated.Length), specials[random.Next(specials.Length - 1)].ToString());

                for (int i = 1; i <= minNumericsLength; i++)
                    generated = generated.Insert(random.Next(generated.Length), number[random.Next(number.Length - 1)].ToString());

                return generated;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

    }


}