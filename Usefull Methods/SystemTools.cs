using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefullMethods
{
    public static class SystemTools
    {
        private static Random random = new Random();
        public static string RandomString(int i_Size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, i_Size)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int GetRandomGoldAmount()
        {
            random = new Random((int)DateTime.Now.Ticks);
            int amount = random.Next(0, 100);
            return amount * 100;
        }

        public static List<int> GetRandomCards(int i_Size)
        {
            random = new Random((int)DateTime.Now.Ticks);

            var lottoNumbers = Enumerable.Range(0, 51).ToList();

            List<int> SelectedNumbers = new List<int>(i_Size);

            for (var i = 0; i < i_Size; i++)
            {
                var number = GetNumber(lottoNumbers.ToArray());

                while (SelectedNumbers.Contains(number))
                {
                    number = GetNumber(lottoNumbers.ToArray());
                }

                SelectedNumbers.Add(number);
            }

            return SelectedNumbers;
        }

        //Removes numbers from the array until only one is left, and returns it
        public static int GetNumber(int[] arr)
        {
            if (arr.Length > 1)
            {
                //Remove random number from array
                var r = random.Next(0, arr.Length);
                var list = arr.ToList();
                list.RemoveAt(r);

                return GetNumber(list.ToArray());
            }

            return arr[0];
        }

        public static void GetRandomLocation(out int ChestX, out int ChestY, int CasinoTopX, int CainoTopY, int CasinoButtomX, int CasinoButtomY)
        {
            random = new Random((int)DateTime.Now.Ticks);
            ChestX = random.Next(CasinoTopX, CasinoButtomX);
            ChestY = random.Next(CainoTopY, CasinoButtomY);
        }
    }
}