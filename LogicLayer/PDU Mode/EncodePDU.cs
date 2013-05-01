using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer
{
    public class EncodePDU
    {
		#region Methods (3) 

		// Public Methods (1) 

        public static string GetMessage(string msg)
        {
            string result = "";

            for (int i = 0; i < msg.Length; i += 8)
            {
                int step = 8;
                if (msg.Length - i < 9)
                {
                    step = msg.Length - i;
                }

                string part = msg.Substring(i, step);
                ASCIIEncoding ascii = new ASCIIEncoding();
                //UnicodeEncoding ascii = new UnicodeEncoding();
                byte[] tempBytes = ascii.GetBytes(part);
                string[] part2 = new string[tempBytes.Length];

                for (int j = 0; j < tempBytes.Length; j++)
                {
                    part2[j] = tempBytes[j].ToString();
                }

                result += Convert8Bit(part2);
            }

            return result;
        }
		// Private Methods (2) 

        private static string Convert8Bit(string[] _8bit)
        {

            string[] num = _8bit;
            string[] numR = new string[8];
            string[] numL = new string[8];

            string[] result1 = new string[7];
            string[] result2 = new string[7];
            string result = "";

            for (int j = 0; j < num.Length; j++)
            {
                string temp = ConvertToBinary(num[j]);
                char[] tempChar = new char[temp.Length];
                temp.CopyTo(0, tempChar, 0, temp.Length);

                Array.Reverse(tempChar);
                for (int i = 0; i < tempChar.Length; i++)
                {
                    if (i < j)
                    {
                        numR[j] = tempChar[i] + numR[j];
                    }
                    else
                    {
                        numL[j] = tempChar[i] + numL[j];
                    }
                }
            }

            int step = 7;
            if (num.Length < 8)
            {
                step = num.Length;
            }
            // Convert Binary to Hex
            for (int i = 0; i < step; i++)
            {
                result1[i] = numR[i + 1] + numL[i];

                string temp = result1[i];
                char[] tempChar = new char[temp.Length];
                temp.CopyTo(0, tempChar, 0, temp.Length);
                Array.Reverse(tempChar);

                int DecNumber = 0;

                for (int j = 0; j < tempChar.Length; j++)
                {
                    DecNumber += Convert.ToInt32((Convert.ToInt32(tempChar[j].ToString())) * Math.Pow(2, j));
                }

                result2[i] = DecNumber.ToString("X2");
                result += result2[i];
            }
            //
            return result;
        }

        private static string ConvertToBinary(string number)
        {
            long number1;
            long.TryParse(number, out number1);

            long baghimande;
            long kharejghesmat;

            string BinaryNumber = "";

            do
            {
                kharejghesmat = number1 / 2;
                baghimande = number1 % 2;
                number1 = kharejghesmat;

                BinaryNumber = baghimande.ToString() + BinaryNumber;
            } while (kharejghesmat >= 2);

            BinaryNumber = kharejghesmat.ToString() + BinaryNumber;

            while (BinaryNumber.Length<7)
            {
                BinaryNumber = "0" + BinaryNumber;
            }

            return BinaryNumber;
        }

		#endregion Methods 
    }
}
