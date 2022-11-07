using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilLib
{
    public static class AgatEncoding
    {
        public static string Decode(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; ++i)
            {
                sb.Append(DecodeChar(bytes[i]));
            }
            return sb.ToString();
        }

        public static byte[] Encode(string str)
        {
            var bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                bytes[i] = EncodeChar(str[i]);
            }
            return bytes;
        }

        private static string DecodeChar(byte b)
        {
            char? c = CharTable[b];
            if (c != null)
                return c.Value.ToString();
            return string.Format("%{0:X2}", b);
        }

        private static byte EncodeChar(char c)
        {
            for (int i = 0; i < CharTable.Length; ++i)
            {
                char? c1 = CharTable[i];
                if (c1 != null && c1.Value == c)
                    return (byte)i;
            }
            throw new Exception(string.Format("Unable to encode '{0}'", c));
        }

        private static readonly char?[] CharTable =
        {
            null, null, null, null, null, null, null, null, // 00
            null, null, null, null, null, null, null, null, // 08
            null, null, null, null, null, null, null, null, // 10
            null, null, null, null, null, null, null, null, // 18
            null, null, null, null, null, null, null, null, // 20
            null, null, null, null, null, null, null, null, // 28
            null, null, null, null, null, null, null, null, // 30
            null, null, null, null, null, null, null, null, // 38
            null,  'a',  'b',  'c',  'd',  'e',  'f',  'g', // 40
             'h',  'i',  'j',  'k',  'l',  'm',  'n',  'o', // 48
             'p',  'q',  'r',  's',  't',  'u',  'v',  'w', // 50
             'x',  'y',  'z',  '{',  '|',  '}', null, null, // 58
             'ю',  'а',  'б',  'ц',  'д',  'е',  'ф',  'г', // 60
             'х',  'и',  'й',  'к',  'л',  'м',  'н',  'о', // 68
             'п',  'я',  'р',  'с',  'т',  'у',  'ж',  'в', // 70
             'ь',  'ы',  'з',  'ш',  'э',  'щ',  'ч',  'ъ', // 78
            null, null, null, null, null, null, null, null, // 80
            null, null, null, null, null, '\n', null, null, // 88
            null, null, null, null, null, null, null, null, // 90
            null, null, null, null, null, null, null, null, // 98
             ' ',  '!',  '"',  '#', null,  '%',  '&', '\'', // A0
             '(',  ')',  '*',  '+',  ',',  '-',  '.',  '/', // A8
             '0',  '1',  '2',  '3',  '4',  '5',  '6',  '7', // B0
             '8',  '9',  ':',  ';',  '<',  '=',  '>',  '?', // B8
             '@',  'A',  'B',  'C',  'D',  'E',  'F',  'G', // C0
             'H',  'I',  'J',  'K',  'L',  'M',  'N',  'O', // C8
             'P',  'Q',  'R',  'S',  'T',  'U',  'V',  'W', // D0
             'X',  'Y',  'Z',  '[', '\\',  ']',  '^',  '_', // D8
             'Ю',  'А',  'Б',  'Ц',  'Д',  'Е',  'Ф',  'Г', // E0
             'Х',  'И',  'Й',  'К',  'Л',  'М',  'Н',  'О', // E8
             'П',  'Я',  'Р',  'С',  'Т',  'У',  'Ж',  'В', // F0
             'Ь',  'Ы',  'З',  'Ш',  'Э',  'Щ',  'Ч',  'Ъ', // F8
        };
    }
}
