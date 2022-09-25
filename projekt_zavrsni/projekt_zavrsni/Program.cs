using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace projekt_zavrsni
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CVRP using EAS");
            Console.WriteLine();

            string chosenFile;
            int chosenVehicleCapacity;
            double alpha;
            double beta;
            double ro;
            int numberOfProgramRuns;
            int maxNumberOfIteration;
            string resultFile;

            Console.WriteLine("valuesE-n22-k4.txt, valuesE-n23-k3.txt, valuesP-n16-k8.txt, valuesP-n19-k2, valuesP-n20-k2");
            Console.Write("Enter just the extention(_) of targeted value file(values_.txt): ");
            chosenFile = Console.ReadLine();
            Console.WriteLine();

            Console.Write("Alpha parameter(1): ");
            alpha = Convert.ToInt32(Console.ReadLine());
            Console.Write("Beta parameter(2-5): ");
            beta = Convert.ToInt32(Console.ReadLine());
            Console.Write("Ro parameter (0.0<ro<1.0): ");
            ro = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine();

            Console.Write("Number of iterations in a program run: ");
            maxNumberOfIteration = Convert.ToInt32(Console.ReadLine());
            Console.Write("Number of program runs: ");
            numberOfProgramRuns = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            Console.Write("Result file name: ");
            resultFile = Console.ReadLine();

            double optimumSolution=0;
            int runCounter = 1;
            
            while (runCounter <= numberOfProgramRuns)
            {
                int numberOfLocations;
                int fullDemand;
                int tempIndex = 0;
                int tempXCoordinate = 0;
                int tempYCoordinate = 0;
                int tempDemand = 0;
                string tempLine;   

                string line;

                var chosenFilePath = Path.Combine(Directory.GetCurrentDirectory(), "values" + chosenFile + ".txt");

                System.IO.StreamReader valuesFile = new System.IO.StreamReader(chosenFilePath);

                numberOfLocations = Convert.ToInt32(valuesFile.ReadLine());
                optimumSolution = Convert.ToDouble(valuesFile.ReadLine());
                double e = Convert.ToDouble(numberOfLocations - 1);
                

                fullDemand = Convert.ToInt32(valuesFile.ReadLine());
                chosenVehicleCapacity = Convert.ToInt32(valuesFile.ReadLine());

                int numberOfNeeededVehicles = (int)Math.Ceiling((double)fullDemand / (double)chosenVehicleCapacity);
                
                double m = Convert.ToDouble(numberOfNeeededVehicles);

                Vehicle[] vehicles = new Vehicle[numberOfNeeededVehicles];

                for (int z = 0; z < numberOfNeeededVehicles; z++)
                {
                    vehicles[z] = new Vehicle(chosenVehicleCapacity);
                }

                Location[] locations = new Location[numberOfLocations];
                int i = 0;

                while ((line = valuesFile.ReadLine()) != null)
                {
                    int count = 0;
                    tempLine = line;
                    string[] tempParts = tempLine.Split(' ');

                    foreach (var word in tempParts)
                    {
                        if (count == 0)
                        {
                            tempIndex = Convert.ToInt32(word);
                        }
                        if (count == 1)
                        {
                            tempXCoordinate = Convert.ToInt32(word);
                        }
                        if (count == 2)
                        {
                            tempYCoordinate = Convert.ToInt32(word);
                        }
                        if (count == 3)
                        {
                            tempDemand = Convert.ToInt32(word);
                        }
                        count++;
                    }

                    locations[i] = new Location(tempIndex, tempXCoordinate, tempYCoordinate, tempDemand);
                    i++;
                }

                valuesFile.Close();

                double[,] matrixOfDistanceValues = new double[numberOfLocations, numberOfLocations];
                double[,] matrixOfHeuristicValues = new double[numberOfLocations, numberOfLocations];

                for (int row = 0; row < numberOfLocations; row++)
                {
                    for (int column = 0; column < numberOfLocations; column++)
                    {
                        if (row == column)
                        {
                            matrixOfDistanceValues[row, column] = 0;
                        }
                        else
                        {
                            double xRow = locations[row].getXCoordinate();
                            double yRow = locations[row].getYCoordinate();

                            double xColumn = locations[column].getXCoordinate();
                            double yColumn = locations[column].getYCoordinate();

                            matrixOfDistanceValues[row, column] = Math.Sqrt(Math.Pow(xColumn - xRow, 2) + Math.Pow(yColumn - yRow, 2));
                        }
                    }
                }

                for (int row = 0; row < numberOfLocations; row++)
                {
                    for (int column = 0; column < numberOfLocations; column++)
                    {
                        if (row == column)
                        {
                            matrixOfHeuristicValues[row, column] = 0;
                        }
                        else
                        {
                            matrixOfHeuristicValues[row, column] = 1 / matrixOfDistanceValues[row, column];
                        }
                    }
                }


                List<Location> locationsOfCnn = new List<Location>();
                List<Location> notVisitedCustomers = new List<Location>();

                for (int k = 1; k < numberOfLocations; k++)
                {
                    notVisitedCustomers.Add(locations[k]);
                }

                locationsOfCnn.Add(locations[0]);

                int holderIndex = locationsOfCnn[0].getindexOfLocation();

                while (notVisitedCustomers.Count > 0)
                {
                    int possibleFirstIndex = notVisitedCustomers.First().getindexOfLocation();
                    double minDistance = matrixOfDistanceValues[holderIndex - 1, possibleFirstIndex - 1];
                    int indexTracker = possibleFirstIndex;

                    foreach (Location notVisitedLocation in notVisitedCustomers)
                    {
                        int possibleIndex = notVisitedLocation.getindexOfLocation();

                        if ((holderIndex - 1) != (possibleIndex - 1))
                        {
                            double tempDistance = matrixOfDistanceValues[holderIndex - 1, possibleIndex - 1];

                            if (tempDistance <= minDistance)
                            {
                                minDistance = tempDistance;
                                indexTracker = possibleIndex;
                            }

                        }
                    }
                    for (int a = 0; a < notVisitedCustomers.Count; a++)
                    {
                        if (indexTracker == notVisitedCustomers[a].getindexOfLocation())
                        {
                            locationsOfCnn.Add(notVisitedCustomers[a]);
                            holderIndex = indexTracker;
                            notVisitedCustomers.Remove(notVisitedCustomers[a]);

                            break;
                        }
                    }
                }

                locationsOfCnn.Add(locations[0]);

                Location[] locationsOfCnnRoute = new Location[numberOfLocations + 1];
                int y = 0;

                foreach (Location locationCnnToConvert in locationsOfCnn)
                {
                    locationsOfCnnRoute[y] = locationCnnToConvert;
                    y++;
                }

                Route Cnn = new Route(locationsOfCnnRoute);
                double CnnValue = Cnn.getQualityOfRoute();

                double[,] matrixOfPheromoneValues = new double[numberOfLocations, numberOfLocations];
                double[,] matrixOfProbabilityValues = new double[numberOfLocations, numberOfLocations];

                for (int row = 0; row < numberOfLocations; row++)
                {
                    for (int column = 0; column < numberOfLocations; column++)
                    {
                        if (row == column)
                        {
                            matrixOfPheromoneValues[row, column] = 0;
                        }
                        else
                        {
                            matrixOfPheromoneValues[row, column] = (e + m) / (ro * CnnValue);
                        }
                    }
                }

                for (int row = 0; row < numberOfLocations; row++)
                {
                    for (int column = 0; column < numberOfLocations; column++)
                    {
                        if (row == column)
                        {
                            matrixOfProbabilityValues[row, column] = 0;
                        }
                        else
                        {
                            matrixOfProbabilityValues[row, column] = (Math.Pow(matrixOfPheromoneValues[row, column], alpha)) * (Math.Pow(matrixOfHeuristicValues[row, column], beta));
                        }
                    }
                }

                int iteration = 1;

                Location[] locationsOfBestRoute = new Location[(numberOfLocations + numberOfNeeededVehicles)];
                Route bestSoFarSolution = new Route();
                double bestSoFarSolutionQuality = 9999999999999999999;
                double bestSoFarSolutionQualityCheck = bestSoFarSolutionQuality;


                while (iteration <= maxNumberOfIteration)
                {
                    int[] vehicleCapacityCheck = new int[numberOfNeeededVehicles];

                    for (int z = 0; z < numberOfNeeededVehicles; z++)
                    {
                        vehicles[z].setVehicleCapacity(chosenVehicleCapacity);
                    }

                    for (int k = 0; k < numberOfNeeededVehicles; k++)
                    {
                        vehicleCapacityCheck[k] = vehicles[k].getVehicleCapacity();
                    }

                    List<Location> locationsOfRouteTabu = new List<Location>();
                    List<Location> notVisitedCustomersForRoute = new List<Location>();

                    for (int c = 1; c < numberOfLocations; c++)
                    {
                        notVisitedCustomersForRoute.Add(locations[c]);
                    }

                    int vehicleCounter = 0;

                    locationsOfRouteTabu.Add(locations[0]);
                    int holderIndexRoute = locationsOfRouteTabu.Last().getindexOfLocation();

                    while (vehicleCounter < numberOfNeeededVehicles)
                    {

                        if (notVisitedCustomersForRoute.Count > 0)
                        {
                            while ((vehicleCapacityCheck[vehicleCounter] > 0) && (notVisitedCustomersForRoute.Count > 0))
                            {
                                int indexTrackerRoute = locationsOfRouteTabu.Last().getindexOfLocation();

                                double totalProbabilitySum = 0;
                                foreach (Location notVisitedLocationRoute in notVisitedCustomersForRoute)
                                {
                                    if ((holderIndexRoute - 1) != (notVisitedLocationRoute.getindexOfLocation() - 1))
                                    {
                                        totalProbabilitySum = totalProbabilitySum + matrixOfProbabilityValues[(holderIndexRoute - 1), (notVisitedLocationRoute.getindexOfLocation() - 1)];
                                    }
                                }

                                Random random = new Random();
                                double randomProbability = random.NextDouble() * totalProbabilitySum;

                                double previousCumulativeSum = 0;
                                double cumulativeSum = 0;

                                foreach (Location notVisitedLocationRoute in notVisitedCustomersForRoute)
                                {
                                    if ((holderIndexRoute - 1) != (notVisitedLocationRoute.getindexOfLocation() - 1))
                                    {
                                        cumulativeSum = cumulativeSum + matrixOfProbabilityValues[holderIndexRoute - 1, (notVisitedLocationRoute.getindexOfLocation() - 1)];

                                        if ((randomProbability > previousCumulativeSum) && (randomProbability <= cumulativeSum))
                                        {
                                            indexTrackerRoute = notVisitedLocationRoute.getindexOfLocation();

                                            break;
                                        }
                                        else if (randomProbability == 0)
                                        {
                                            indexTrackerRoute = notVisitedLocationRoute.getindexOfLocation();
                                            break;
                                        }

                                        previousCumulativeSum = cumulativeSum;
                                    }
                                }

                                for (int a = 0; a < notVisitedCustomersForRoute.Count; a++)
                                {
                                    if (indexTrackerRoute == notVisitedCustomersForRoute[a].getindexOfLocation())
                                    {

                                        int tempVehicleCapacity = vehicles[vehicleCounter].getVehicleCapacity() - notVisitedCustomersForRoute[a].getDemandOfLocation();
                                        if (tempVehicleCapacity < 0)
                                        {
                                            vehicleCapacityCheck[vehicleCounter] = vehicleCapacityCheck[vehicleCounter] - notVisitedCustomersForRoute[a].getDemandOfLocation();
                                            break;
                                        }
                                        else
                                        {
                                            vehicles[vehicleCounter].removeForCustomerCapacity(notVisitedCustomersForRoute[a].getDemandOfLocation());
                                            vehicleCapacityCheck[vehicleCounter] = vehicleCapacityCheck[vehicleCounter] - notVisitedCustomersForRoute[a].getDemandOfLocation();

                                            locationsOfRouteTabu.Add(notVisitedCustomersForRoute[a]);

                                            holderIndexRoute = indexTrackerRoute;

                                            notVisitedCustomersForRoute.Remove(notVisitedCustomersForRoute[a]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        locationsOfRouteTabu.Add(locations[0]);
                        vehicleCounter++;
                    }

                    Location[] locationsOfRouteTabuArr = new Location[numberOfLocations - 1 + numberOfNeeededVehicles + 1];
                    int w = 0;

                    foreach (Location locationOfTabuToConvert in locationsOfRouteTabu)
                    {
                        locationsOfRouteTabuArr[w] = locationOfTabuToConvert;
                        w++;
                    }

                    Route possibleBestSoFarSolution = new Route(locationsOfRouteTabuArr);

                    double tempSolutionQualityBest;
                    double tempSolutionQuality;

                    double Ck;
                    double Cbs;

                    if (notVisitedCustomersForRoute.Count == 0)
                    {
                        tempSolutionQuality = possibleBestSoFarSolution.getQualityOfRoute();
                        tempSolutionQualityBest = possibleBestSoFarSolution.getQualityOfRoute();

                        if ((tempSolutionQuality < bestSoFarSolutionQuality))
                        {
                            for (int b = 0; b < numberOfLocations; b++)
                            {
                                for (int f = 0; f < numberOfLocations; f++)
                                {
                                    if (b == f)
                                    {
                                        matrixOfPheromoneValues[b, f] = 0;
                                    }
                                    else
                                    {
                                        matrixOfPheromoneValues[b, f] = (1 - ro) * matrixOfPheromoneValues[b, f];
                                    }
                                }
                            }

                            Ck = tempSolutionQuality;
                            Cbs = tempSolutionQualityBest;

                            for (int q = 0; q < ((locationsOfRouteTabuArr.Length) - 1); q++)
                            {
                                int j = i + 1;

                                int currentIndex = locationsOfRouteTabuArr[i].getindexOfLocation();
                                int nextIndex = locationsOfRouteTabuArr[j].getindexOfLocation();

                                matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] = matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] + (1 / Ck) + (e * (1 / Cbs));

                            }

                            for (int row = 0; row < numberOfLocations; row++)
                            {
                                for (int column = 0; column < numberOfLocations; column++)
                                {
                                    if (row == column)
                                    {
                                        matrixOfProbabilityValues[row, column] = 0;
                                    }
                                    else
                                    {
                                        matrixOfProbabilityValues[row, column] = (Math.Pow(matrixOfPheromoneValues[row, column], alpha)) * (Math.Pow(matrixOfHeuristicValues[row, column], beta));
                                    }
                                }
                            }

                            bestSoFarSolutionQuality = tempSolutionQuality;
                            bestSoFarSolution = possibleBestSoFarSolution;
                            locationsOfBestRoute = locationsOfRouteTabuArr;

                            iteration++;
                        }
                        else if ((tempSolutionQuality >= bestSoFarSolutionQuality))
                        {
                            for (int b = 0; b < numberOfLocations; b++)
                            {
                                for (int f = 0; f < numberOfLocations; f++)
                                {
                                    if (b == f)
                                    {
                                        matrixOfPheromoneValues[b, f] = 0;
                                    }
                                    else
                                    {
                                        matrixOfPheromoneValues[b, f] = (1 - ro) * matrixOfPheromoneValues[b, f];
                                    }

                                }
                            }

                            Ck = tempSolutionQuality;
                            Cbs = tempSolutionQualityBest;

                            for (int q = 0; q < ((locationsOfRouteTabuArr.Length) - 1); q++)
                            {
                                int j = i + 1;

                                int currentIndex = locationsOfRouteTabuArr[i].getindexOfLocation();
                                int nextIndex = locationsOfRouteTabuArr[j].getindexOfLocation();

                                matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] = matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] + (1 / Ck);

                            }

                            for (int q = 0; q < ((locationsOfBestRoute.Length) - 1); q++)
                            {
                                int j = i + 1;

                                int currentIndex = locationsOfBestRoute[i].getindexOfLocation();
                                int nextIndex = locationsOfBestRoute[j].getindexOfLocation();

                                matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] = matrixOfPheromoneValues[(currentIndex - 1), (nextIndex - 1)] + (e * (1 / Cbs));

                            }

                            for (int row = 0; row < numberOfLocations; row++)
                            {
                                for (int column = 0; column < numberOfLocations; column++)
                                {
                                    if (row == column)
                                    {
                                        matrixOfProbabilityValues[row, column] = 0;
                                    }
                                    else
                                    {
                                        matrixOfProbabilityValues[row, column] = (Math.Pow(matrixOfPheromoneValues[row, column], alpha)) * (Math.Pow(matrixOfHeuristicValues[row, column], beta));
                                    }
                                }
                            }

                            iteration++;
                        }
                        else
                        {
                            iteration++;
                        }

                    }
                    else
                    {
                        iteration++;
                    }

                }

                if (!File.Exists(resultFile + ".txt"))
                {
                    using (StreamWriter resultWriterNew = File.CreateText(resultFile + ".txt"))
                    {
                        if (bestSoFarSolutionQualityCheck == bestSoFarSolutionQuality)
                        {
                            resultWriterNew.WriteLine("no routes were made");
                        }
                        else
                        {
                            double BestSolutionValue = bestSoFarSolution.getQualityOfRoute();
                            string resultLine = BestSolutionValue.ToString("#.####");
                            resultWriterNew.WriteLine(resultLine);
                        }
                    }
                }
                else
                {
                    using (StreamWriter resultWriterExist = File.AppendText(resultFile + ".txt"))
                    {
                        if (bestSoFarSolutionQualityCheck == bestSoFarSolutionQuality)
                        {
                            resultWriterExist.WriteLine("no routes were made");
                        }
                        else
                        {
                            double BestSolutionValue = bestSoFarSolution.getQualityOfRoute();
                            string resultLine = BestSolutionValue.ToString("#.####");
                            resultWriterExist.WriteLine(resultLine);
                        }
                    }

                }

                runCounter++;
            }


            string resultFileLine;
            var resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), resultFile + ".txt");
            System.IO.StreamReader resultFileAnalysis = new System.IO.StreamReader(resultFilePath);

            Console.WriteLine("\n");

            int analysisCount = 0;
            List<double> routeQualityData = new List<double>();

            while ((resultFileLine = resultFileAnalysis.ReadLine()) != null)
            {
                if(resultFileLine != "no routes were made")
                {
                    analysisCount++;
                    routeQualityData.Add(Convert.ToDouble(resultFileLine));
                }
            }

            double sumOfQualityOfRoutes = 0;
            foreach(double quality in routeQualityData)
            {
                sumOfQualityOfRoutes += quality;
            }
            double averageQualityOfRoutes = sumOfQualityOfRoutes / Convert.ToDouble(analysisCount);
            Console.WriteLine("Average quality is: " + string.Format("{0:0.0000}", averageQualityOfRoutes));

            double standardDeviation = 0;
            double sumOfSquaresForStandardDeviation = 0;
            foreach (double quality in routeQualityData)
            {
                sumOfSquaresForStandardDeviation += Math.Pow((quality-averageQualityOfRoutes),2);
            }
            standardDeviation = Math.Sqrt(sumOfSquaresForStandardDeviation / (Convert.ToDouble(analysisCount)));
            Console.WriteLine( "Standard deviation is: " + string.Format("{0:0.0000}", standardDeviation));

            double bestRouteQualityResult=9999999999999999999;
            foreach (double quality in routeQualityData)
            {
                if (quality <= bestRouteQualityResult)
                {
                    bestRouteQualityResult = quality;
                }
            }
            Console.WriteLine("Best route quality solution is: " + bestRouteQualityResult);

            double worstRouteQualityResult = 0;
            foreach (double quality in routeQualityData)
            {
                if (quality >= worstRouteQualityResult)
                {
                    worstRouteQualityResult = quality;
                }
            }
            Console.WriteLine("Worst route quality solution is: " + worstRouteQualityResult);

            double deviationFromOptimumSolution = Math.Abs((1 - (averageQualityOfRoutes / optimumSolution))) * 100;
            Console.WriteLine("Deviation from optimum solution is: " + string.Format("{0:0.0000}", deviationFromOptimumSolution) + "%");
            
            return;
        }
    }
}
