/* @author Alex Phelps
 * 
 * Simple perceptron (single node neural network)
 *  that is trained from a csv file included in the project.
 *  
 * The default one I have set up is for an AND gate, but
 *  anything a perceptron can do should work, as long
 *  as you also change the 'dim' constant to match the
 *  number of inputs (+ bias).
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimplePerceptron
{
    class Program
    {
        const int dim = 2 + 1;                                  // 2 inputs + bias
        const double learnRate = 0.5;                       // rate of weight changes during epochs

        private static Random rand = new Random();
        private static double[] weights = new double[dim];  // perceptron weights

        static void Main(string[] args)
        {

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = rand.NextDouble();
            }

            //weights = new double[] { 0, 0, 0 };


            // Get and format training data from project csv
            List<double[]> training = new List<double[]>(); // training data value arrays
            try
            {
                using (var reader = new StreamReader(@".\training.csv")) // The premade one I created is for an  [AND Gate]
                {
                    while (!reader.EndOfStream)
                    {
                        double[] train = new double[dim];           // train array 
                        string[] split = reader.ReadLine().Split(',');  // array of split csv strings (this row)

                        if (split.Length != dim) // value for each dimension + 1 for output and -1 for bias
                        {
                            throw new FormatException("training.csv does not have the correct number of columns.");
                        }

                        for (int i = 0; i < split.Length; i++)
                        {
                            train[i] = double.Parse(split[i]);  // will throw exception if not formatted right, ie check below catch
                        }

                        training.Add(train);
                    }
                }
            }
            catch (Exception e) // csv file could not be read correctly
            {
                Console.WriteLine("Training Data not formatted correctly!");

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine("---------Starting weights---------");
            for (int i = 0; i < dim; i++)
            {
                Console.WriteLine("W_{0}: {1:0.000}", i + 1, weights[i]);
            }

            // Training
            int epochs = 0;     // just for output
            bool weightChange;  // until no weight changes
            do
            {
                epochs++;
                weightChange = false; // reset loop var

                foreach (double[] train in training) // loop training data
                {
                    double output = Predict(train);
                    double expectedOutput = train[dim - 1]; // Output from csv

                    double error = expectedOutput - output;

                    if (Math.Abs(error) > 0.001) // not correct yet (with sigmoid as opposed to step, you cannot reasonably get no error)
                    {
                        weightChange = true;

                        for (int i = 0; i < dim; i++)
                        {
                            double[] values = train.Prepend(1).ToArray(); // Add bias
                            weights[i] += learnRate * error * values[i]; // alter weights based in error, learn rate, and training input
                        }
                    }
                }
            }
            while (weightChange);
            //

            //Conclusion
            Console.WriteLine();
            Console.WriteLine("---Final Weights after {0} epochs---", epochs);
            for (int i = 0; i < dim; i++)
            {
                Console.WriteLine("W_{0}: {1:0.0}", i + 1, weights[i]);
            }
            Console.WriteLine("----------------------------------");

            // User Input
            double[] userInput = new double[dim - 1]; // - bias
            Console.WriteLine();
            for (int i = 0; i < dim - 1; i++)
            {
                Console.Write("Input value {0}: ", i + 1);
                
                if (!double.TryParse(Console.ReadLine(), out userInput[i])) // Parse as double
                {
                    // If failed parse
                    i--; // redo this loop
                    Console.WriteLine("Please enter a valid number.");
                }
            }

            // In a realistic scenario, you would have to round Predict() in order to get a proper 1 or 0 answer (for sigmoid)
            //  Here I show the output of the raw function value so that it is easy to see in the front-end
            Console.WriteLine("Predict({0}) = {1} (Raw Output: {2})", string.Join(", ", userInput), Math.Round(Predict(userInput)), Predict(userInput));


            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static double Predict(double[] input)
        {
            input = input.Prepend(1).ToArray(); // Add bias
            double sum = weights.Select((weight, index) => weight * input[index]).Sum(); // multiplies each input by corresponding weight, then sums
            double output = Activation(sum);    // Node output (ie activation function)

            return output;
        }

        private static double Activation(double value)
        {
            return Sigmoid(value);
            // return (value >= 0) ? 1 : 0; // step function
        }

        private static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1 + k);
        }
    }
}
