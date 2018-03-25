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
        Package Outliers = new Package();
        Dictionary<int, Package> InstancesSortedByResult = new Dictionary<int, Package>();
        List<Package> TestPackages = new List<Package>();
        List<Double> StdDeviation = new List<double>();
        List<Double> Avgs = new List<double>();
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
            Statistics();
            NormalizeData();
            SortDataByResult();
            PadData();
            DetectOutlier();
            Statistics();
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
            //gets path of file with dataset
            string deploypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(deploypath, filename);
            FileStream file = new FileStream(filepath, FileMode.Open); //ADD ERRORHANDLING HERE!!!!
            StreamReader sr = new StreamReader(file);
            //read and discard headingline if necessary
            if (hasheading) sr.ReadLine();

            //number of valid attributes inculding result
            int attributecount = -1; //number of valid attributes inculding result
            string f;
            //reads one instance per line and adds it to list if instance has all attributes
            while ((f = sr.ReadLine()) != null)
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
            //getting string array filled with attributes of instance  
            string[] attributes = f.Split(seperator.ToArray<char>());
            //instance that has less attributes than normal get dumped
            if (attributes.Length <= resultposition) return null;
            //creates new Instance with index of given result
            Instance i = new Instance(results.Result(attributes[resultposition]));
            for (int j = 0; j < attributes.Length; j++)
            {
                if (j == resultposition) continue;
                double value;
                string attribute = attributes[j];
                //replaces thousand and decimal seperator if given
                if (!string.IsNullOrEmpty(thousandsSeperator)) attribute = attribute.Replace(thousandsSeperator, "");
                if (!string.IsNullOrEmpty(decimalSeperator)) attribute = attribute.Replace(decimalSeperator, ",");
                //trys to parse value, depending on result adds it to attributes or invalid
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
            //getting string array filled with attributes of instance 
            string[] attributes = f.Split(seperator.ToArray<char>());
            //creates new Instance with index of -1
            Instance i = new Instance(-1);
            for (int j = 0; j < attributes.Length; j++)
            {
                double value;
                string attribute = attributes[j];
                //replaces thousand and decimal seperator if given
                if (!string.IsNullOrEmpty(thousandsSeperator)) attribute = attribute.Replace(thousandsSeperator, "");
                if (!string.IsNullOrEmpty(decimalSeperator)) attribute = attribute.Replace(decimalSeperator, ",");
                //trys to parse value, depending on result adds it to attributes or invalid
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

        //returns arithmetic mean of attribute number "index" for all instances in Package
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

        //returns arithmetic mean of attribute number "index" for all instances in Package
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

        //Sorts out Outliers
        //Outliers get removed from all packages and are put into Outliers package
        private void DetectOutlier()
        {
            allInstances.Clear();
            for (int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                DetectOutlierPerPackage(InstancesSortedByResult[i]);
            }
            //sort data so outliers are removed from sorted packages too
            SortDataByResult();
        }

        //Sorts out outliers from Package that should contain only Instances with same results
        //Instances with attributes that differ from the attribute-mean of this package more than "strict"-times the standard deviation are removed
        private void DetectOutlierPerPackage(Package p)
        {
            Dictionary<int, double> avg = new Dictionary<int, double>();
            Dictionary<int, double> devi = new Dictionary<int, double>();

            //check for every instance in package
            for (int i = 0; i < p.Count; i++)
            {
                Instance instance = p.GetInstance(i);
                bool ok = true;
                //check for each attribute of this instance
                for (int iatt = 0; iatt < instance.Count; iatt++)
                {
                    //calculate avg and standard deviation only if not already calculated yet
                    if (!avg.ContainsKey(iatt) || !devi.ContainsKey(iatt))
                    {
                        double average = Average(iatt, p);
                        avg.Add(iatt, average);
                        double deviation = StandardDeviation(iatt, p);
                        devi.Add(iatt, deviation);
                    }

                    double attr = instance.GetAttribute(iatt);
                    //check if outlier
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
                    //add instance again to allInstances if no outlier
                    allInstances.AddInstance(instance);
                }
                else
                {
                    //add Instance to Outlier Package if it is an outlier
                    Outliers.AddInstance(instance);
                }
            }
        }

        void PadData()
        {
            //Pad Data seperated by sorted by result packages
            for(int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                PadPackage(InstancesSortedByResult[i]);
            }
        }

        //Pad every attribute in every instance of package
        void PadPackage(Package p)
        {
            //number of attributes retrieved from allInstances because all instances forced to have same number of attributes before
            for (int i = 0; i < allInstances.GetInstance(0).Count; i++)
            {
                PadAttribute(i, p);
            }
        }

        //Pad attribute number index in all instances of package p
        void PadAttribute(int index, Package p)
        {
            //calculate average of attribute over all instances of package
            double avg = Average(index, p);
            Console.WriteLine("Package {0} Attribute {1} avg: {2}", results.Result(p.GetInstance(0).Result), index, avg);
            for (int i = 0; i < p.Count; i++)
            {
                //try to correct attribute
                //class instance itself takes care of the case that attribute is already correct
                p.GetInstance(i).ReviseAttribute(index, avg);
            }
        }

        //serperates instances from allInstances into seperate packages depending on their result
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

        //divides all instances contained in InstancesSortedByResult into kfc disjoint Packages
        void DivideData(int kfc)
        {
            for(int i = 0; i < kfc; i++)
            {
                TestPackages.Add(new Package());
            }
            for(int i = 0; i < InstancesSortedByResult.Count; i++)
            {
                //randomize order of instances in package to remove any order present when read from file
                InstancesSortedByResult[i].Randomize();
                for(int j = 0; j < InstancesSortedByResult[i].Count; j++)
                {
                    //distribute instances with same result equally to testpackages
                    TestPackages[j % kfc].AddInstance(InstancesSortedByResult[i].GetInstance(j));
                }
            }
        }

        //calculates euclidean distance between two instances
        double EuclideanDistance(Instance i, Instance j)
        {
            double sum = 0;
            for(int k = 0; k < i.Count; k++)
            {
                sum += Math.Pow((i.GetAttribute(k) - j.GetAttribute(k)), 2);
            }

            return Math.Sqrt(sum);
        }

        //k-fold-cross test all packages in TestPackages
        //additionally test ptotential outliers that were sorted out before
        void KFCTestPackages(int knn)
        {
            for(int i = 0; i < TestPackages.Count; i++)
            {
                //put all packages except for the one currently being tested into a new list of packages
                List<Package> ToLearn = TestPackages.Except(new List<Package>() { TestPackages.ElementAt(i) }).ToList<Package>();
                Package p = Package.Concat(ToLearn);
                TestPackage(p, TestPackages[i], knn);
            }
            TestPackage(allInstances, Outliers, knn);
        }

        //test the package tt with the data from p being used to learn
        void TestPackage(Package p, Package tt, int knn)
        {
            for (int i = 0; i < tt.Count; i++)
            {
                int y = Classify(tt.GetInstance(i), p, knn);
                //add 1 to field in confusion matrix depending on the actual and the calculated result
                ConfusionMatrix[tt.GetInstance(i).Result, y]++;
            }
        }

        //Classify an instance that is represented by a string similar to the one that was read for learning data
        public string Classify(string s, int knn)
        {
            Instance i = ReadInstance(s);
            //normalize each attribute of the instance so it can be compared to learnig data
            for(int j = 0; j < i.Count; j++)
            {
                i.NormalizeAttribute(j, Avgs[j], StdDeviation[j]);
            }
            //return result string based on classification
            return results.Result(Classify(i, allInstances, knn));
        }

        //classify an instance using the data in package p for learning, considering the knn nearest neighbours
        int Classify(Instance i, Package p, int knn)
        {
            Dictionary<Instance, double> Distances = new Dictionary<Instance, double>();
            //save the euclidean distance between the instance i and every instance of p
            for(int j = 0; j < p.Count; j++)
            {
                Distances.Add(p.GetInstance(j), EuclideanDistance(i, p.GetInstance(j)));
            }

            //sort the distances list by distance ascendening
            List<Instance> Sorted = (from entry in Distances orderby entry.Value ascending select entry.Key).ToList<Instance>();
            int[] ResultCount = new int[results.Count];
            //evaluate the first knn Instances with smallest distance
            for(int j = 0; j < knn; j++)
            {
                try
                {
                    //count number of occurences of results
                    ResultCount[Sorted.ElementAt(j).Result]++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    //in case that there were less than knn packages in p
                    break;
                }
                
            }

            //make sure that if more than one result was the most common not, it does not depend on the order 
            //in which the results were read, which value is returned
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


        //display confusion matrix and accuracy
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
                    //add to correct count if actual and calculated result were the same
                    if (i == j) correct += ConfusionMatrix[i, j];
                    sum += ConfusionMatrix[i, j];
                }
                Console.WriteLine("");
            }

            Console.WriteLine("");
            //calculate accuracy
            double acc = correct / sum;
            Console.WriteLine("Accuracy: {0}", acc);
        }

        //normalize every instance so all attributes weigh equally
        private void NormalizeData()
        {
            //normalize evera attribute in allInstances
            for(int i = 0; i < allInstances.GetInstance(0).Count; i++)
            {
                allInstances.NormalizeAttribute(i, Avgs[i], StdDeviation[i]);
            }
        }

        //calcualte mean an standard deviation for all attributes and save them to list so they dont have to be calculated every time
        private void Statistics()
        {
            Avgs.Clear();
            StdDeviation.Clear();
            for(int i = 0; i<allInstances.GetInstance(0).Count; i++)
            {
                Avgs.Add(Average(i, allInstances));
                StdDeviation.Add(StandardDeviation(i, allInstances));
            }
}
    }
}
