namespace AhirMilap.DAL.CommonService
{
    public static class SecurityManager
    {
        #region CONST

        private const string StrTamperProofKey = "!trustmaster";

        #endregion

        #region Encryption / Decryption

        /// <summary>
        /// The tamper proof string encode.
        /// </summary>
        /// <param name="strValue">
        /// The string value.
        /// </param>
        /// <param name="strKey">
        /// The string key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TamperProofStringEncode(string strValue, string strKey)
        {
            System.Security.Cryptography.HMACSHA1 mac3Des = new System.Security.Cryptography.HMACSHA1();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3Des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strKey));
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(strValue)) + Convert.ToChar("-") + Convert.ToBase64String(mac3Des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strValue)));
        }

        /// <summary>
        /// The tamper proof string decode.
        /// </summary>
        /// <param name="strValue">
        /// The string value.
        /// </param>
        /// <param name="strKey">
        /// The string key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// exception Argument Exception
        /// </exception>
        public static string TamperProofStringDecode(string strValue, string strKey)
        {
            String strDataValue;
            strValue = strValue.Trim();
            strValue = strValue.Replace(" ", "+");

            System.Security.Cryptography.HMACSHA1 mac3Des = new System.Security.Cryptography.HMACSHA1();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3Des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strKey));

            try
            {
                strDataValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(strValue.Split(Convert.ToChar("-"))[0]));
            }
            catch (Exception)
            {
                //throw new ArgumentException("Invalid TamperProofString. " + ex.Message);
                return strValue;
            }
            return strDataValue;
        }

        /// <summary>
        /// Encode string
        /// </summary>
        /// <param name="strValue">
        /// This is string parameter
        /// </param>
        /// <returns>
        /// returns a string value
        /// </returns>
        public static string Encrypt(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
                return string.Empty;
            return TamperProofStringEncode(strValue, StrTamperProofKey);
        }

        /// <summary>
        /// Decode String
        /// </summary>
        /// <param name="strValue">
        /// This is string value
        /// </param>
        /// <returns>
        /// returns a string value
        /// </returns>
        public static string Decrypt(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return string.Empty;
            }

            return TamperProofStringDecode(strValue, StrTamperProofKey);
        }

        #endregion
    }
}
