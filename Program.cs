using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace VMfirstTry
{
    class Program
    {
        public static decimal correctInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                decimal res;
                bool answ = Decimal.TryParse(input, out res);
                if (!answ)
                    Console.WriteLine("Некорректный ввод");
                else
                    return res;
            }
        }

        public static int correctIntInput()
        {
            string pattern = @"^\-?\d+$";
            int result;
            while (true)
            {
                string input = Console.ReadLine();
                if (!Regex.IsMatch(input, pattern) || Convert.ToInt32(input) > 20)
                    Console.WriteLine("Некорректный ввод");
                else
                {
                    result = Convert.ToInt32(input);
                    return result;
                }

            }
        }

        public enum status { disable, enable, main, fromFile, byHand, oneStepToStart, tryToSolveTheEquation, randomInput };

        #region input/output
        public static void inputMatrix()
        {
            Console.Write("Введите размерность матрицы: ");
            int n = correctIntInput();
            for (int i = 0; i < n; i++)
            {
                List<decimal> strOfMatrix = new List<decimal>();
                Matrix.matrix.Add(strOfMatrix);
                for (int j = 0; j < n + 1; j++)
                {
                    if (j != n)
                        Console.Write("Введите коэффициент при X{0}{1}: ", i + 1, j + 1);
                    else
                        Console.Write("Введите свободный член уравнения: ");
                    strOfMatrix.Add(Program.correctInput());
                }
            }
        }

        public static void randomInput()
        {
            Console.Write("Введите размерность матрицы: ");
            int n = correctIntInput();
            Random rand = new Random();
            for (int i = 0; i < n; i++)
            {
                List<decimal> strOfMatrix = new List<decimal>();
                Matrix.matrix.Add(strOfMatrix);
                for (int j = 0; j < n + 1; j++)
                {
                    strOfMatrix.Add((decimal)rand.Next(-100, 100));
                }
            }
        }

        public static void inputFromFile(string path)
        {
            XmlDocument fileWithMatrix = new XmlDocument();
            fileWithMatrix.Load(path);
            XmlNodeList xmlList = fileWithMatrix.GetElementsByTagName("str");  
                for (int i = 0; i < xmlList.Count; i++)
                {
                    List<decimal> strOfMatrix = new List<decimal>();
                    foreach (XmlNode node in xmlList[i])
                        strOfMatrix.Add(Decimal.Parse(node.InnerText));
                    Matrix.matrix.Add(strOfMatrix);
                }
                XmlNodeList accuracy = fileWithMatrix.GetElementsByTagName("accuracy");
                Matrix.accuracy = Decimal.Parse(accuracy[0].InnerText);
        }

        public static void printMatrix()
        {
            Console.WriteLine("Матрица: ");
            for (int i = 0; i < Matrix.matrix.Count + 1; i++)
                if (i != Matrix.matrix.Count)
                    Console.Write("{0, 11}|", "x" + (i + 1));
                else
                    Console.Write("{0, 11}|", "b");
            Console.Write("\n");
            for (int i = 0; i < Matrix.matrix.Count; i++)
            {
                for (int j = 0; j < Matrix.matrix.Count + 1; j++)
                {
                    Console.Write("{0, 11}|", Matrix.matrix[i][j]);
                }
                Console.Write("\n");
            }
        }

        public static void printResult()
        {
            if (Matrix.solutionFlag)
            {
                Console.Write("|{0, 6}|", "№ шага");
                for (int i = 0; i < Matrix.matrix.Count; i++)
                    Console.Write("{0, 11}|", "X" + (i + 1));
                Console.Write("\n");
                Console.Write("|{0, 6}|", (Matrix.step - 1) + "  ");
                for (int i = 0; i < Matrix.arrOfX.Count; i++)
                {
                    Console.Write("{0, 11}|", Decimal.Round(Matrix.arrOfX[i], 8));
                }
                Console.WriteLine("\n");
                Console.Write("|{0, 6}|{1, 11}|", "№ шага", "Погрешности");
                Console.Write("\n");
                for (int i = 0; i < Matrix.errors.Count; i++)
                {
                    Console.WriteLine("|{0, 6}|{1, 11}|", i + 1, Decimal.Round(Matrix.errors[i], 8));
                }
            }
            else
                Console.WriteLine("Эту систему уравнений невозможно решить методом простых итераций\n");
        }
        #endregion

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            int st = (int)status.main;
            while (st != (int)status.disable)
            {
                while (st == (int)status.main)
                {
                    Console.WriteLine("Ввести матрицу вручную или считать из файла?\n1. Вручную\n2. Из файла\n3. Заполнить рандомом\n0. Выйти");
                    int result = correctIntInput();
                    switch (result)
                    {
                        case 1:
                            st = (int)status.byHand;
                            break;
                        case 2:
                            st = (int)status.fromFile;
                            break;
                        case 3:
                            st = (int)status.randomInput;
                            break;
                        case 0:
                            st = (int)status.disable;
                            break;
                        default:
                            continue;
                    }
                }
                while (st == (int)status.byHand)
                {
                    Matrix.matrix.Clear();
                    Matrix.errors.Clear();
                    Matrix.arrOfX.Clear();
                    inputMatrix();
                    printMatrix();
                    Console.Write("Введите точность: ");
                    Matrix.accuracy = Program.correctInput();
                    st = (int)status.oneStepToStart;
                }
                while (st == (int)status.fromFile)
                {
                    bool correctFile = false;
                    while (!correctFile)
                    {
                        bool allBad = true;
                        Matrix.matrix.Clear();
                        Matrix.errors.Clear();
                        Matrix.arrOfX.Clear();
                        Console.WriteLine("1. Использовать файл по умолчанию\n2. Прописать путь\n");
                        int choice = correctIntInput();
                        switch (choice)
                        {
                            case 1:
                                try { inputFromFile("C:/Users/A/Desktop/123.xml"); }
                                catch (Exception)
                                {
                                    Console.WriteLine("Некорректный формат элементов, исправьте файл или выберите новый");
                                    allBad = false;
                                    break;
                                }
                                break;
                            case 2:
                                string path = Console.ReadLine();
                                /*if (!File.Exists(path))
                                {
                                    Console.WriteLine("Такого файла нет");
                                    continue;
                                }
                                else
                                    inputFromFile(path);
                                break;*/
                                try { inputFromFile(path); }
                                catch (Exception)
                                {
                                    Console.WriteLine("Проблемы с файлом");
                                    allBad = false;
                                    break;
                                }
                                break;
                            default:
                                continue;
                        }
                        if (!allBad)
                            continue;
                        else
                            correctFile = true;
                    }
                    printMatrix();
                    st = (int)status.oneStepToStart;
                }
                while (st == (int)status.oneStepToStart)
                {
                    Console.WriteLine("1. Рассчитать\n2. Назад\n0. Выйти");
                    int choice = correctIntInput();
                    switch (choice)
                    {
                        case 1:
                            st = (int)status.tryToSolveTheEquation;
                            break;
                        case 2:
                            st = (int)status.main;
                            break;
                        case 0:
                            st = (int)status.disable;
                            break;
                        default:
                            continue;
                    }
                }
                while (st == (int)status.randomInput)
                {
                    Matrix.matrix.Clear();
                    Matrix.errors.Clear();
                    Matrix.arrOfX.Clear();
                    randomInput();
                    printMatrix();
                    Console.Write("Введите точность: ");
                    Matrix.accuracy = Program.correctInput();
                    Matrix.tryDiagDomination();
                    st = (int)status.oneStepToStart;
                }
                while (st == (int)status.tryToSolveTheEquation)
                {
                    if (Matrix.isDiagDomination())
                    {
                        //Console.Write("Введите точность: ");
                        //Matrix.accuracy = Program.correctInput();
                        Matrix.tryToSolveTheEquation();
                    }
                    else
                    {
                        Console.WriteLine("Нет диагонального преобладания\n\n1. Попытаться преобразовать матрицу\n2. Не преобразовывать\n0. В главное меню");
                        int choice = correctIntInput();
                        switch (choice)
                        {
                            case 1:
                                Matrix.tryDiagDomination();
                                if (!Matrix.isDiagDomination())
                                    Console.WriteLine("Не удается добиться диагонального преобладания\n");
                                else
                                    printMatrix();
                                //Console.Write("Введите точность: ");
                                //Matrix.accuracy = Program.correctInput();
                                Matrix.tryToSolveTheEquation();
                                break;
                            case 2:
                                Matrix.tryToSolveTheEquation();
                                break;
                            case 0:
                                st = (int)status.main;
                                break;
                            default:
                                continue;
                        }
                    }
                    printResult();
                    st = (int)status.main;
                }

            }
        }
    }
}
