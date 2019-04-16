using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xpidea.neuro.net;
using xpidea.neuro.net.backprop;
using xpidea.neuro.net.patterns;

namespace scanner
{
    class OCR
    {
        public List<string> initializare(string[] s)
        {
            OCRNetwork.OCR();
            List<string> total = new List<string>();
            foreach (string item in s)
            {
                string aux = item;
                aux = aux.Contains("Ă") ? aux.Replace("Ă", "A") : aux;
                aux = aux.Contains("Â") ? aux.Replace("Â", "A") : aux;
                aux = aux.Contains("Ș") ? aux.Replace("Ș", "S") : aux;
                aux = aux.Contains("Ț") ? aux.Replace("Ț", "T") : aux;
                aux = aux.Contains(" ") ? aux.Replace(" ", ".") : aux;
                OCRNetwork.test = aux;
                string res = OCRNetwork.result();
                total.Add(res);
            }
            return total;
        }
        public class OCRNetwork : BackPropagationRPROPNetwork
        {
            public static int aMatrixDim = 10;
            public static byte aFirstChar = (byte)'-';
            public static byte aLastChar = (byte)'z';
            public static int aCharsCount = aLastChar - aFirstChar + 1;
            public static PatternsCollection trainingPatterns;
            public static OCRNetwork backpropNetwork;
            public static System.Windows.Forms.Label label5=new System.Windows.Forms.Label();
            public static string test;

            public static void OCR()
            {
                
                label5.Text = "";
                for (int i = 0; i < aCharsCount; i++)
                    label5.Text += Convert.ToChar(aFirstChar + i) + " ";
                generare_training_patterns();
                backpropNetwork = new OCRNetwork(new int[3] { aMatrixDim * aMatrixDim, (aMatrixDim * aMatrixDim + aCharsCount) / 2, aCharsCount });
                if (trainingPatterns != null && backpropNetwork != null)
                {
                    backpropNetwork.Train(trainingPatterns);
                }
            }
            public OCRNetwork(int[] nodesInEachLayer) : base(nodesInEachLayer)
            {

            }
            private int OutputPatternIndex(Pattern pattern)
            {
                for (int i = 0; i < pattern.OutputsCount; i++)
                    if (pattern.Output[i] == 1)
                        return i;
                return -1;
            }
            public int BestNodeIndex
            {
                get
                {
                    int result = -1;
                    double aMaxNodeValue = 0;
                    double aMinError = double.PositiveInfinity;
                    for (int i = 0; i < this.OutputNodesCount; i++)
                    {
                        NeuroNode node = OutputNode(i);
                        if ((node.Value > aMaxNodeValue) || ((node.Value >= aMaxNodeValue) && (node.Error < aMinError)))
                        {
                            aMaxNodeValue = node.Value;
                            aMinError = node.Error;
                            result = i;
                        }

                    }
                    return result;
                }
            }
            public override void Train(PatternsCollection patterns)
            {

                int iteration = 0;
                if (patterns != null)
                {
                    double error = 0;
                    int good = 0;
                    while (good < patterns.Count) // Train until all patterns are correct
                    {

                        error = 0;
                        good = 0;
                        for (int i = 0; i < patterns.Count; i++)
                        {
                            for (int k = 0; k < NodesInLayer(0); k++)
                                nodes[k].Value = patterns[i].Input[k];
                            this.Run();
                            for (int k = 0; k < this.OutputNodesCount; k++)
                            {
                                error += Math.Abs(this.OutputNode(k).Error);
                                this.OutputNode(k).Error = patterns[i].Output[k];
                            }
                            this.Learn();
                            if (BestNodeIndex == OutputPatternIndex(patterns[i]))
                                good++;

                            iteration++;
                        }

                        foreach (NeuroLink link in links) ((EpochBackPropagationLink)link).Epoch(patterns.Count);
                    }
                }
            }

            public static PatternsCollection CreateTrainingPatterns(Font font)
            {
                PatternsCollection result = new PatternsCollection(aCharsCount, aMatrixDim * aMatrixDim, aCharsCount);
                for (int i = 0; i < aCharsCount; i++)
                {
                    double[] aBitMatrix = CharToDoubleArray(Convert.ToChar(aFirstChar + i), font, aMatrixDim, 0);
                    for (int j = 0; j < aMatrixDim * aMatrixDim; j++)
                        result[i].Input[j] = aBitMatrix[j];
                    result[i].Output[i] = 1;
                }
                return result;
            }
            public static double[] CharToDoubleArray(char aChar, Font aFont, int aArrayDim, int aAddNoisePercent)
            {
                double[] result = new double[aArrayDim * aArrayDim];
                Graphics gr = label5.CreateGraphics();
                Size size = Size.Round(gr.MeasureString(aChar.ToString(), aFont));
                Bitmap aSrc = new Bitmap(size.Width, size.Height);
                Graphics bmp = Graphics.FromImage(aSrc);
                bmp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                bmp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                bmp.Clear(Color.White);
                bmp.DrawString(aChar.ToString(), aFont, new SolidBrush(Color.Black), new Point(0, 0), new StringFormat());
                double xStep = (double)aSrc.Width / (double)aArrayDim;
                double yStep = (double)aSrc.Height / (double)aArrayDim;
                for (int i = 0; i < aSrc.Width; i++)
                    for (int j = 0; j < aSrc.Height; j++)
                    {
                        int x = (int)((i / xStep));
                        int y = (int)(j / yStep);
                        Color c = aSrc.GetPixel(i, j);
                        result[y * x + y] += Math.Sqrt(c.R * c.R + c.B * c.B + c.G * c.G); //Convert to BW
                    }
                return Scale(result);
            }
            private static double MaxOf(double[] src)
            {
                double res = double.NegativeInfinity;
                foreach (double d in src)
                    if (d > res) res = d;
                return res;
            }

            private static double[] Scale(double[] src)
            {
                double max = MaxOf(src);
                if (max != 0)
                {
                    for (int i = 0; i < src.Length; i++)
                        src[i] = src[i] / max;
                }
                return src;
            }
            private static void generare_training_patterns()
            {
                Font arialFont = new Font("Britannic Bold", 19, System.Drawing.FontStyle.Regular);
                trainingPatterns = CreateTrainingPatterns(arialFont);
            }
            public static string result()
            {
                char[] res = test.ToCharArray();
                string rezultat = "";
                Font arialFont = new Font("Britannic Bold", 19, System.Drawing.FontStyle.Regular);
                if (backpropNetwork != null)
                {
                    foreach (var item in res)
                    {
                        double[] aInput = CharToDoubleArray(item, arialFont, aMatrixDim, 0);
                        for (int i = 0; i < backpropNetwork.InputNodesCount; i++)
                            backpropNetwork.InputNode(i).Value = aInput[i];
                        backpropNetwork.Run();
                        rezultat += (string)Convert.ToChar(aFirstChar + backpropNetwork.BestNodeIndex).ToString();
                    }
                }
                return rezultat;
            }

        }
    }
}
