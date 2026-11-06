using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDTO.Utilities
{
    public class RandomPassword
    {
        #region Fields
        const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string UPPER_CASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "123456789";
        const string SPECIALS = @"@#$%^&()";
        private readonly bool _useLowercase, _useUppercase, _useNumbers, _useSpecial;
        private readonly int _passwordSize;
        #endregion
        #region Constructor
        public RandomPassword()
        {
            _useLowercase = true;
            _useUppercase = true;
            _useNumbers = true;
            _useSpecial = true;
            _passwordSize = 12;
        }
        public RandomPassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial, int password)
        {
            _useLowercase = useLowercase;
            _useUppercase = useUppercase;
            _useNumbers = useNumbers;
            _useSpecial = useSpecial;
            _passwordSize = password;
        }
        #endregion
        #region Methods
        public string Next()
        {
            string charSet = "";
            System.Random _random = new Random();
            int counter;
            if (_useUppercase) charSet += UPPER_CASE;
            if (_useLowercase) charSet += LOWER_CASE;
            if (_useNumbers) charSet += NUMBERS;
            if (_useSpecial) charSet += SPECIALS;
            var newPasswordBuilder = new StringBuilder(_passwordSize);
            for (counter = 0; counter < _passwordSize - 3; counter++)
            {
                newPasswordBuilder.Append(charSet[_random.Next(charSet.Length - 1)]);
            }
            newPasswordBuilder.Append(SPECIALS[_random.Next(SPECIALS.Length - 1)]);
            newPasswordBuilder.Append(NUMBERS[_random.Next(NUMBERS.Length - 1)]);
            newPasswordBuilder.Append(UPPER_CASE[_random.Next(UPPER_CASE.Length - 1)]);
            return newPasswordBuilder.ToString();
        }
        #endregion
    }
}
