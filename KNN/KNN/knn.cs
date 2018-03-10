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
        int k;
        bool hasheading;
        Results results = new Results();
        Package allInstances = new Package();

        public knn(string Filename, string Seperator, int Resultposition, int K, bool hasHeading = false)
        {
            filename = Filename;
            seperator = Seperator;
            resultposition = Resultposition;
            k = K;
            hasheading = hasHeading;

            ReadData();
        }

        private void ReadData()
        {
            string deploypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(deploypath, filename);
            FileStream file = new FileStream(filepath, FileMode.Open);
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
                    if(double.TryParse(attributes[j], out value))
                    {
                        i.AddAttribute(value);
                        attcount++;
                    }
                    if (attributecount == -1) attributecount = attcount;
                    else
                    {
                        if(attcount == attributecount)
                        {
                            allInstances.AddInstance(i);
                        }
                    }
                }

            }

            sr.Close();
        }
    }
}
