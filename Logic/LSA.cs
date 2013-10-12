using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    /// <summary>
    /// Класс-помощник для вычисления SVD разложения и получения координат
    /// </summary>
    public class LSA
    {
        #region Field
        //Составные части формулы
        //A = U * W * V^T
        double[,] A;

        double[,] U;
        double[,] VT;
        double[] W;

        //Размер матрицы А
        readonly int m;
        readonly int n;

        //Настройки(для бибилиотеки alglibnet2)
        //Возвращаемые аргументы(возвращаем целиком матрицу U и V^T)
        const int uNeeded = 1;
        const int vtNeeded = 1;
        //Производительность(от 0 до 2)- за счет выделения памяти
        const int additionalMemory = 2;
        #endregion
        #region Properties
        public KeyValuePair<double, double>[] FirtsCoords
        {
            get
            {
                //Пока берем по 2 строки(2-х мерный график)
                KeyValuePair<double, double>[] allTextsCoords = new KeyValuePair<double, double>[n];
                for (int j = 0; j < VT.GetLength(1); j++)
                {
                    allTextsCoords[j] = new KeyValuePair<double, double>(VT[0, j], VT[1, j]);
                }
                return allTextsCoords;
            }
        }
        public KeyValuePair<double, double>[] SecondCoords
        {
            get
            {
                //Пока берем по 2 колонки(2-х мерный график)
                KeyValuePair<double, double>[] allWordsCoords = new KeyValuePair<double, double>[m];
                for (int i = 0; i < U.GetLength(0); i++)
                {
                    allWordsCoords[i] = new KeyValuePair<double, double>(U[i, 0], U[i, 1]);
                }
                return allWordsCoords;
            }
        }
        #endregion
        /// <summary>
        /// Констурктор
        /// </summary>
        /// <param name="A">Текущая матрица для разложения</param>
        public LSA(double[,] A)
        {
            this.A = A;

            m = A.GetLength(0);
            n = A.GetLength(1);

            if (!Calculate())
            {
                throw new Exception("Не удалось выполнить SVD разложение");
            }
        }

        /// <summary>
        /// SVD разложение матрицы A. В результате получаются 3 матрицы.
        /// </summary>
        /// <returns>Результат операции</returns>
        bool Calculate()
        {
            bool result = alglib.rmatrixsvd(A, m, n, uNeeded, vtNeeded, additionalMemory, out W, out U, out VT);
            return result;
        }
    }
}
