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
        //Размер выходных матриц(напр. 2 - 2 координаты, 3-3координаты. Чем меньше размерность, тем меньше влияние шумов(но совсем маленькая тоже не катит))
        #endregion
        #region Properties
        private double[,] _UCoords;
        /// <summary>
        /// Обрезанная до 2-й размерности матрица U LSA разложения.
        /// </summary>
        public double[,] UCoords
        {
            get { return _UCoords; }
        }
        private double[,] _VCoords;
        /// <summary>
        /// Обрезанная до 2-й размерности матрица V LSA разложения.
        /// </summary>
        public double[,] VCoords
        {
            get { return _VCoords; }
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
            _UCoords = new double[m, 2];
            _VCoords = new double[n, 2];

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

            for (int i = 0; i < n; i++)
            {
                _VCoords[i, 0] = VT[0, i];
                _VCoords[i, 1] = VT[1, i];
            }
            for (int i = 0; i < m; i++)
            {
                _UCoords[i, 0] = U[i, 0];
                _UCoords[i, 1] = U[i, 1];
            }

            return result;
        }
    }
}
