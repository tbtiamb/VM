using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VMfirstTry
{
    class Matrix
    {
        public static List<List<decimal>> matrix = new List<List<decimal>>();

        public static List<decimal> arrOfX = new List<decimal>();

        public static decimal accuracy;

        public static int step;

        public static bool solutionFlag;

        public static List<decimal> errors = new List<decimal>();

        public static bool isDiagDomination()
        {
            bool isDiagDom = true;
            for (int i = 0; i < matrix.Count; i++)
            {
                decimal diagElem = Math.Abs(matrix[i][i]);
                decimal sumElem = 0;
                for (int j = 0; j < matrix.Count; j++)
                {
                    if (j != i)
                        sumElem = sumElem + Math.Abs(matrix[i][j]);
                }
                if (diagElem >= sumElem)
                    continue;
                else
                    isDiagDom = false;
            }
            return isDiagDom;
        }

        public static void tryDiagDomination()
        {
            for (int oneStrAndAllCollsIndex = 0; oneStrAndAllCollsIndex < matrix.Count; oneStrAndAllCollsIndex++)
            {
                for (int changeStrIndex = 0; changeStrIndex < matrix.Count - 1; changeStrIndex++)
                {
                    List<decimal> strOfMatrix = matrix[changeStrIndex];
                    matrix[changeStrIndex] = matrix[changeStrIndex + 1];
                    matrix[changeStrIndex + 1] = strOfMatrix;
                    if (isDiagDomination())
                        break;
                    for (int fullChangeCollIndex = 0; fullChangeCollIndex < matrix.Count; fullChangeCollIndex++)
                    {
                        for (int j = 0; j < matrix.Count - 1; j++)
                        {
                            for (int i = 0; i < matrix.Count; i++)
                            {
                                decimal tmp = matrix[i][j];
                                matrix[i][j] = matrix[i][j + 1];
                                matrix[i][j + 1] = tmp;
                            }
                            if (isDiagDomination())
                                break;
                        }
                        if (isDiagDomination())
                            break;
                    }
                }
                if (isDiagDomination())
                    break;
            }
        }

        public static void tryToSolveTheEquation()
        {
            List<decimal> arrOfXnextStep = new List<decimal>();
            for (int i = 0; i < matrix.Count; i++)
            {
                arrOfX.Add(0);
                arrOfXnextStep.Add(0);
            }
            step = 1;
            bool divZeroFlag = false;
            decimal lastErr = 0;
            bool lessThenAccuracyFlag = false;
        
            while (!lessThenAccuracyFlag)
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    arrOfXnextStep[i] = matrix[i][matrix.Count];
                    for (int j = 0; j < matrix.Count; j++)
                    {
                        if (j != i)
                            arrOfXnextStep[i] -= matrix[i][j] * arrOfX[j];
                    }
                    if (matrix[i][i] != 0)
                        arrOfXnextStep[i] /= matrix[i][i];
                    else
                    {
                        divZeroFlag = true;
                        break;
                    }
                }
                if (divZeroFlag)
                {
                    solutionFlag = false;
                    break;
                }
                List<decimal> subXes = new List<decimal>();
                 decimal maxOfsubs = 0;
                for (int i = 0; i < arrOfXnextStep.Count; i++)
                {
                    subXes.Add(Math.Abs(Math.Abs(arrOfXnextStep[i]) - Math.Abs(arrOfX[i])));                    
                    if (subXes[i] > maxOfsubs)
                        maxOfsubs = subXes[i];
                }
                if (maxOfsubs > lastErr && step != 1)
                {
                    solutionFlag = false;
                    break;
                }
                lastErr = maxOfsubs;
                errors.Add(maxOfsubs);
                for (int i = 0; i < arrOfXnextStep.Count; i++)
                {
                    arrOfX[i] = arrOfXnextStep[i];
                }
                if (maxOfsubs <= accuracy)
                {
                   lessThenAccuracyFlag = true;
                    solutionFlag = true;
                    break;
                }
                step++;
            }
        }
    }
}
