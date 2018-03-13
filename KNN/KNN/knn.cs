using System;
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
        bool hasheading;
        double strict;
        string decimalSeperator;
        string thousandsSeperator;

        Results results = new Results();
        Package allInstances = new Package();
        Dictionary<int, Package> InstancesSortedByResult = new Dictionary<int, Package>();
        List<Package> TestPackages = new List<Package>();
        int[,] ConfusionMatrix;

        public knn(string Filename, string Seperator, int Resultposition, bool hasHeading = false, double OutlierStrictness = 2, string DecimalSeperator = ",", string ThousandsSeperator = "")
        {
            filename = Filename;
            if (string.IsNullOrEmpty(Seperator)) throw new ArgumentException("Seperator cannot be null or empty");
            seperator = Seperator;
            if (Resultposition < 0) throw new ArgumentException("Resultposition cannot be smaller than 0");
            resultposition = Resultposition;
            hasheading = hasHeading;
            strict = OutlierStrictness;
            decimalSeperator = DecimalSeperator;
            thousandsSeperator = ThousandsSeperator;

            ReadData();
            SortDataByResult();
            PadData();
            DetectOutlier();
            SortDataByResult();
        }

        public void Test(int knn, int kfc)
        {
            if (knn <= 0) throw new ArgumentException("knn cannot be smaller or equal to 0");
            if (kfc <= 0 || kfc > allInstances.Count) throw new ArgumentException("kfc cannot be smaller or equal to 0 and not bigger than the number of instances");
            DivideData(kfc);
            ConfusionMatrix = new int[results.Count,results.Count];
            KFCTestPackages(knn);
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
                Instance i = ReadTestInstance(f);
                if (i == null) continue;

                if (attributecount == -1)
                {
                    attributecount = i.Count;
                    allInstances.AddInstance(i);
                }
                else
                {
                    if (i.Count == attributecount)
                    {
                        allInstances.AddInstance(i);
                    }
                }

            }

            sr.Close();
        }

        private Instance ReadTestInstance(string f)
        {
            string[] attributes = f.Split(seperator.ToArray<char>());
            if (attributes.Length <= resultposition) return null;
            Instance i = new Instance(results.Result(attributes[resultposition]));
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
                }
                else
                {
                    i.AddInvalid();
                }
            }
            return i;
        }

        private Instance ReadInstance(string f)
        {
            string[] attributes = f.Split(seperator.ToArray<char>());
            Instance i = new Instance(-1);
            for (int j = 0; j < attributes.Length; j++)
            {
                double value;
                string attribute = attributes[j];
                if (!string.IsNullOrEmpty(thousandsSeperator)) attribute = attribute.Replace(thousandsSeperator, "");
                if (!string.IsNullOrEmpty(decimalSeperator)) attribute = attribute.Replace(decimalSeperator, ",");
                if (double.TryParse(attribute, out value))
                {
                    i.AddAttribute(value);
                }
                else
                {
                    i.AddInvalid();
                }
            }
            return i;
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
            allInstances.Clear();
            for (int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                DetectOutlierPerPackage(i, InstancesSortedByResult[i]);
            }
        }

        private void DetectOutlierPerPackage(int pNr, Package p)
        {
            Dictionary<int, double> avg = new Dictionary<int, double>();
            Dictionary<int, double> devi = new Dictionary<int, double>();

            for (int i = 0; i < p.Count; i++)
            {
                Instance instance = p.GetInstance(i);
                bool ok = true;
                for (int iatt = 0; iatt < instance.Count; iatt++)
                {
                    if (!avg.ContainsKey(iatt) || !devi.ContainsKey(iatt))
                    {
                        double average = Average(iatt, p);
                        avg.Add(iatt, average);
                        double deviation = StandardDeviation(iatt, p);
                        devi.Add(iatt, deviation);
                    }

                    double attr = instance.GetAttribute(iatt);
                    if (attr < (avg[iatt] + (devi[iatt] * strict)) && attr > (avg[iatt] - (devi[iatt] * strict)))
                    {
                        ok = true;
                    }
                    else
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    allInstances.AddInstance(instance);
                }
            }
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
            Console.WriteLine("Package {0} Attribute {1} avg: {2}", results.Result(p.GetInstance(0).Result), index, avg);
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
            if (InstancesSortedByResult.Any()) InstancesSortedByResult.Clear();
            for(int i = 0; i < results.Count; i++)
            {
                InstancesSortedByResult.Add(i, new Package());
            }

            for(int i = 0; i < allInstances.Count; i++)
            {
                InstancesSortedByResult[allInstances.GetInstance(i).Result].AddInstance(allInstances.GetInstance(i));
            }
        }

        void DivideData(int kfc)
        {
            for(int i = 0; i < kfc; i++)
            {
                TestPackages.Add(new Package());
            }
            for(int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                InstancesSortedByResult[i].Randomize();
                for(int j = 0; j < InstancesSortedByResult[i].Count; j++)
                {
                    TestPackages[j % kfc].AddInstance(InstancesSortedByResult[i].GetInstance(j));
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

        void KFCTestPackages(int knn)
        {
            for(int i = 0; i < TestPackages.Count; i++)
            {
                List<Package> ToLearn = TestPackages.Except(new List<Package>() { TestPackages.ElementAt(i) }).ToList<Package>();
                TestPackage(ToLearn, TestPackages[i], knn);
            }
        }

        void TestPackage(List<Package> tl, Package tt, int knn)
        {
            Package p = new Package();
            for (int i = 0; i < tl.Count; i++)
            {
                p = Package.Concat(p, tl[i]);
            }


            for(int i = 0; i < tt.Count; i++)
            {
                int y = Classify(tt.GetInstance(i), p, knn);
                ConfusionMatrix[tt.GetInstance(i).Result, y]++;
            }
        }

        public string Classify(string s, int knn)
        {
            Instance i = ReadInstance(s);
            return results.Result(Classify(i, allInstances, knn));
        }

        int Classify(Instance i, Package p, int knn)
        {
            Dictionary<Instance, double> Distances = new Dictionary<Instance, double>();
            for(int j = 0; j < p.Count; j++)
            {
                Distances.Add(p.GetInstance(j), EuclideanDistance(i, p.GetInstance(j)));
            }

            List<Instance> Sorted = (from entry in Distances orderby entry.Value ascending select entry.Key).ToList<Instance>();
            int[] ResultCount = new int[results.Count];
            for(int j = 0; j < knn; j++)
            {
                try
                {
                    ResultCount[Sorted.ElementAt(j).Result]++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
                
            }

            int Max = ResultCount.Max();
            List<int> HighestResultCounts = new List<int>();
            for(int j = 0; j < ResultCount.Length; j++)
            {
                if (ResultCount[j] == Max) HighestResultCounts.Add(j);
            }

            if (HighestResultCounts.Count == 1) return HighestResultCounts[0];
            else
            {
                Random rnd = new Random();
                int r = rnd.Next(HighestResultCounts.Count);
                return HighestResultCounts[r];
            }
            
        }



        public void DisplayConfusionMatrix()
        {
            if(ConfusionMatrix == null)
            {
                Console.WriteLine("No KFC test has been done yet");
                return;
            }
            double sum = 0;
            double correct = 0;
            Console.WriteLine("\nConfusion Matrix:");
            for (int i = 0; i < results.Count; i++)
            {
                for(int j = 0; j < results.Count; j++)
                {
                    Console.Write("{0} ", ConfusionMatrix[i,j]);
                    if (i == j) correct += ConfusionMatrix[i, j];
                    sum += ConfusionMatrix[i, j];
                }
                Console.WriteLine("");
            }

            Console.WriteLine("");
            double acc = correct / sum;
            Console.WriteLine("Accuracy: {0}", acc);
        }
    }
}
