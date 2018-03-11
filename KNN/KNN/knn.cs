﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class knn
    {
        string filename;
        string seperator;
        int resultposition;
        int k;
        bool hasheading;
        string decimalSeperator;
        string thousandsSeperator;
        Results results = new Results();
        Package allInstances = new Package();
        Dictionary<int, Package> InstancesSortedByResult = new Dictionary<int, Package>();
        List<Package> TestPackages = new List<Package>();
        int[,] ConfusionMatrix;

        public knn(string Filename, string Seperator, int Resultposition, int K, bool hasHeading = false, string DecimalSeperator = ",", string ThousandsSeperator = "")
        {
            filename = Filename;
            if (string.IsNullOrEmpty(Seperator)) throw new ArgumentException("Seperator cannot be null or empty");
            seperator = Seperator;
            if (Resultposition < 0) throw new ArgumentException("Resultposition cannot be smaller than 0");
            resultposition = Resultposition;
            if (K <= 0) throw new ArgumentException("k cannnot be 0");
            k = K;
            hasheading = hasHeading;
            decimalSeperator = DecimalSeperator;
            thousandsSeperator = ThousandsSeperator;

            ReadData();
            if (allInstances.Count < k) throw new NumberOfInstancesTooSmallException("Number of Instances cannot be smaller than k");
            SortDataByResult(); //nach Ausreisser aussortieren!!
            PadData();
        }

        public void Test()
        {
            DivideData();
            ConfusionMatrix = new int[results.Count,results.Count];
            KFCTestPackages();
        }

        private void ReadData()
        {
            string deploypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(deploypath, filename);
            FileStream file = new FileStream(filepath, FileMode.Open); //ADD ERRORHANDLING HERE!!!!
            StreamReader sr = new StreamReader(file);
            if (hasheading) sr.ReadLine(); //read and discard headingline if necessary

            int attributecount = -1; //number of valid attributes inculding result
            string f;
            while((f = sr.ReadLine()) != null)
            {
                string[] attributes = f.Split(seperator.ToArray<char>());
                if (attributes.Length <= resultposition) continue;
                Instance i = new Instance(results.Result(attributes[resultposition]));
                int attcount = 1;
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (j == resultposition) continue;
                    double value;
                    string attribute = attributes[j];
                    if (!string.IsNullOrEmpty(thousandsSeperator)) attribute = attribute.Replace(thousandsSeperator, "");
                    if (!string.IsNullOrEmpty(decimalSeperator)) attribute = attribute.Replace(decimalSeperator, ",");
                    if (double.TryParse(attribute, out value))
                    {
                        i.AddAttribute(value);
                        attcount++;
                    }
                    else
                    {
                        i.AddInvalid();
                        attcount++;
                    }
                }
                if (attributecount == -1)
                {
                    attributecount = attcount;
                    allInstances.AddInstance(i);
                }
                else
                {
                    if (attcount == attributecount)
                    {
                        allInstances.AddInstance(i);
                    }
                }

            }

            sr.Close();
        }

        private double Average(int index, Package p)
        {
            double sum = 0;
            int count = 0;

            for(int i = 0; i < p.Count; i++)
            {
                try
                {
                    sum += p.GetInstance(i).GetAttribute(index);
                    count++;
                }
                catch (AttributeInvalidException)
                {
                    continue;
                }
            }

            if (count > 0) return sum / count;
            else return 0;
        }

        private double StandardDeviation(int index, Package p)
        {
            double avg = Average(index, p);
            double differentialsum = 0;

            for (int i = 0; i < p.Count; i++)
            {
                differentialsum += Math.Pow((p.GetInstance(i).GetAttribute(index) - avg), 2);
            }

            if (p.Count < 2) return 0;
            else return Math.Sqrt(differentialsum / (p.Count - 1));
        }

        private void DetectOutlier()
        {
            // give index to statistic1 and statistic2 to get value
            // getattributes from instance

        }

        void PadData()
        {
            for(int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                PadPackage(InstancesSortedByResult[i]);
            }
        }

        void PadPackage(Package p)
        {
            for (int i = 0; i < allInstances.GetInstance(0).Count; i++)
            {
                PadAttribute(i, p);
            }
        }

        void PadAttribute(int index, Package p)
        {
            double avg = Average(index, p);
            Console.WriteLine("Package {0} Attribute {1}: {2}", results.Result(p.GetInstance(0).Result), index, avg);
            for (int i = 0; i < p.Count; i++)
            {
                try
                {
                    p.GetInstance(i).ReviseAttribute(index, avg);
                }
                catch (CorrectAttributeCannotBeCorrectedException)
                {
                    continue;
                }
            }
        }

        void SortDataByResult()
        {
            for(int i = 0; i < results.Count; i++)
            {
                InstancesSortedByResult.Add(i, new Package());
            }

            for(int i = 0; i < allInstances.Count; i++)
            {
                InstancesSortedByResult[allInstances.GetInstance(i).Result].AddInstance(allInstances.GetInstance(i));
            }
        }

        void DivideData()
        {
            for(int i = 0; i < k; i++)
            {
                TestPackages.Add(new Package());
            }
            for(int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                for(int j = 0; j < InstancesSortedByResult[i].Count; j++)
                {
                    TestPackages[j % k].AddInstance(InstancesSortedByResult[i].GetInstance(j));
                }
            }
        }

        double EuclideanDistance(Instance i, Instance j)
        {
            double sum = 0;
            for(int k = 0; k < i.Count; k++)
            {
                sum += Math.Pow((i.GetAttribute(k) - j.GetAttribute(k)), 2);
            }

            return Math.Sqrt(sum);
        }

        void KFCTestPackages()
        {
            for(int i = 0; i < TestPackages.Count; i++)
            {
                List<Package> ToLearn = TestPackages.Except(new List<Package>() { TestPackages.ElementAt(i) }).ToList<Package>();
                TestPackage(ToLearn, TestPackages[i]);
            }
        }

        void TestPackage(List<Package> tl, Package tt)
        {
            for(int i = 0; i < tl.Count; i++)
            {
                
            }
            for(int i = 0; i < tt.Count; i++)
            {
                //int y = Classify(tt.GetInstance(i), tl);
            }
        }

        int Classify(Instance i, Package p)
        {
            return 0;
        }
    }
}
