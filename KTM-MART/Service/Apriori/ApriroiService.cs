using KTM_MART.Models;

namespace KTM_MART.Service.Apriori
{
    public class ApriroiService : IAprioriService
    {

        private readonly TrainingSet m_set;
        private readonly int m_support;
        private readonly int m_confidence;

        //private readonly int m_thresholdSupport;
        public ApriroiService(TrainingSet set, int support, int confidence)
        {
            m_set = set;
            m_support = support > 100 ? 100 : support < 0 ? 0 : support;
            m_confidence = confidence > 100 ? 100 : confidence < 0 ? 0 : confidence;

            //m_thresholdSupport = m_set.Samples.Count * m_support / 100;
        }
        public string PrintFinalValues(Dictionary<string[], int> group)
        {
            string output = "RESULTS\n";
            output += "--------------------\n";
            int index = 1;
            foreach (KeyValuePair<string[], int> product in group)
            {
                string[] keys = product.Key;

                for (int i = -1; i < keys.Length; i++)
                {
                    output += $"\nRESULT {index} : \n";

                    string[] ins;
                    string[] outs;

                    if (i == -1)
                    {
                        ins = new string[] { keys[0], keys[1] };
                        outs = keys.Except(ins).ToArray();
                        output += PrintThresholdRule(keys, ins, outs);
                        index++;

                        continue;
                    }

                    ins = new string[] { keys[i] };
                    outs = keys.Except(ins).ToArray();

                    output += PrintThresholdRule(keys, ins, outs);

                    index++;

                }

            }
            return output;
        }
        private string PrintThresholdRule(string[] keys, string[] ins, string[] outs)
        {
            int XYZ = GetGroupCountInSamples(keys);
            int N = GetGroupCountInSamples(ins);
            double result = (double)XYZ / (double)N * 100;
            string resultString = $"trust({string.Join(",", ins)} -> {string.Join(",", outs)})\n";
            resultString += $"The probability of being [{string.Join(",", outs)}] on the product set [{string.Join(",", ins)}]\t%{result}";
            return resultString;
        }

        public Dictionary<string[], int> MergeGroupProducts(Dictionary<string[], int> grouped)
        {
            Dictionary<string[], int> temp = new Dictionary<string[], int>(new ArrayComparer());
            List<string> datas = new List<string>();
            foreach (KeyValuePair<string[], int> main in grouped)
            {
                string[] keys = main.Key;
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!datas.Contains(keys[i]))
                    {
                        datas.Add(keys[i]);
                    }
                }
            }
            temp.Add(datas.ToArray(), GetGroupCountInSamples(datas.ToArray()));
            return temp;
        }

        public Dictionary<string[], int> GroupProducts(Dictionary<string, int> productCounts)
        {
            Dictionary<string[], int> temp = new Dictionary<string[], int>(new ArrayComparer());
            foreach (KeyValuePair<string, int> main in productCounts)
            {
                string mainKey = main.Key;
                foreach (KeyValuePair<string, int> sub in productCounts)
                {
                    string subKey = sub.Key;
                    if (!mainKey.Equals(subKey))
                    {
                        string[] head1 = new string[] { mainKey, subKey };
                        string[] head2 = head1.Reverse().ToArray();
                        if (!temp.ContainsKey(head1) && !temp.ContainsKey(head2))
                        {
                            temp.Add(head1, GetGroupCountInSamples(head1));
                        }
                    }
                }
            }
            return temp;
        }
        public int GetGroupCountInSamples(string[] head)
        {
            int temp = 0;
            for (int i = 0; i < this.m_set.Samples.Count; i++)
            {
                if (head.Except(this.m_set.Samples[i].Products).Count() == 0)
                {
                    temp++;
                }
            }
            return temp;
        }
        //support(A -> B) = count{X, Y, Z} / N
        public double GetGroupSupportThreshold(string[] A, string[] B)
        {
            double temp = 0.0d;

            return temp;
        }
        public string PrintCounts(Dictionary<string, int> productCounts)
        {
            string result = "";
            foreach (KeyValuePair<string, int> product in productCounts)
            {
                result += $"count({product.Key})\t{product.Value}\n";
            }
            return result;
        }
        public string PrintMergedGrouped(Dictionary<string[], int> productCounts)
        {
            string result1 = "";
            foreach (KeyValuePair<string[], int> product in productCounts)
            {
                result1 += ($"{string.Join(",", product.Key)}\t{product.Value}");
            }
            return result1;
        }

        public string PrintGrouped(Dictionary<string[], int> productCounts)
        {
            string result1 = "";
            foreach (KeyValuePair<string[], int> product in productCounts)
            {
                result1 += ($"{string.Join(",", product.Key)}\t{product.Value}\n");
            }
            return result1;
        }

        public string PrintGroups(Dictionary<string[], int> productCounts)
        {
            string result1 = "";
            foreach (KeyValuePair<string[], int> product in productCounts)
            {
                result1 += ($"{string.Join(",", product.Key)}\t{product.Value}\n");
            }
            return result1;
        }

        public Dictionary<string, int> RemoveThreshold(Dictionary<string, int> productCounts)
        {
            return productCounts.Where(pair => pair.Value >= 3).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        public Dictionary<string[], int> RemoveThresholds(Dictionary<string[], int> productCounts)
        {
            return productCounts.Where(pair => pair.Value >= 3).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public Dictionary<string, int> CalulateProductCounts()
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            for (int i = 0; i < m_set.Samples.Count; i++)
            {
                for (int j = 0; j < m_set.Samples[i].Products.Count; j++)
                {
                    string product = m_set.Samples[i].Products[j];
                    if (temp.ContainsKey(product))
                    {
                        temp[product]++;
                    }
                    else
                    {
                        temp.Add(product, 1);
                    }
                }
            }
            return temp;
        }
    }
}
