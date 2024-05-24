
using System.Text.RegularExpressions;


namespace Ecommerce.Service.src.Shared
{
    public static class ValidationHelper
    {
        // Regular expressions for validation
        private static readonly Regex ImageUrlRegex = new(@"^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$", RegexOptions.IgnoreCase);
        private static readonly Regex PhoneRegex = new(@"^(\+358|0)\d{8,9}$"); // only Finnish phone number
        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        private static readonly Regex StringRegex = new(@"^[a-zA-Z0-9\s]+$");
        private static readonly Regex PostalCodeRegex = new(@"^\d{5}$"); // only Finnish postal code

        /// <summary>
        /// Validates if the provided URL is a valid image URL.
        /// </summary>
        public static bool IsImageUrlValid(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return ImageUrlRegex.IsMatch(url);
        }

        /// <summary>
        /// Validates if the provided phone number is a valid Finnish phone number.
        /// </summary>
        public static bool IsPhoneValid(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return PhoneRegex.IsMatch(phone);
        }

        /// <summary>
        /// Validates if the provided email address is valid.
        /// </summary>
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// Validates if the provided string contains only alphanumeric characters and spaces.
        /// </summary>
        public static bool IsStringValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return StringRegex.IsMatch(input);
        }

        /// <summary>
        /// Validates if the provided postal code is a valid Finnish postal code.
        /// </summary>
        public static bool IsPostalCodeValid(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode)) return false;
            return PostalCodeRegex.IsMatch(postalCode);
        }

        /// <summary>
        /// Validates if the provided name is a 2-20 chars letters
        /// </summary>
        public static bool IsValidName(string name)
        {
            var pattern = @"^[a-zA-Z]{2,20}$";
            return Regex.IsMatch(name, pattern);
        }

        /// <summary>
        /// Validates if the provided password has at 6 characters, at most 20, with at least onw capital letter ,one lower case letter, and one number
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,20}$";
            return Regex.IsMatch(password, pattern);
        }
    }
}