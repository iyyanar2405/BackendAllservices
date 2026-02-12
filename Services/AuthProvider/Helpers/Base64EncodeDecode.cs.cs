using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthProvider.Helpers
{
    public static class Base64EncodeDecode
    {
        public static string Encode(string valueToEncode)
        {
            // convert a string to a byte array
            byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(valueToEncode);
            return Convert.ToBase64String(bytesToEncode);
        }

        public static string Decode(string EncodedValueToDecode)
        {
            // decode a base64 encoded string
            byte[] decodedBytes = Convert.FromBase64String(EncodedValueToDecode);
            // convert the decoded byte array back to a string
            return System.Text.Encoding.UTF8.GetString(decodedBytes);
        }
    }
}
