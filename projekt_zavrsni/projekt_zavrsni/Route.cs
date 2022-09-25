using System;
using System.Collections.Generic;
using System.Text;

namespace projekt_zavrsni
{
    class Route
    {
        public Location[] customersOnRoute;

        public Route()
        {

        }

        public Route(Location[] customers)
        {
            customersOnRoute = customers;
        }


        public double getQualityOfRoute()
        {
            double qualityOfRoute = 0;

            for (int i = 0; i < ((customersOnRoute.Length) - 1); i++)
            {
                int j = i + 1;

                double xCurrentCustomer = customersOnRoute[i].getXCoordinate();
                double yCurrentCustomer = customersOnRoute[i].getYCoordinate();

                double xNextCustomer = customersOnRoute[j].getXCoordinate();
                double yNextCustomer = customersOnRoute[j].getYCoordinate();

                if (xCurrentCustomer == xNextCustomer && yCurrentCustomer == yNextCustomer)
                {
                    qualityOfRoute = 9999999999999999999;
                    break;
                }
                else
                {
                    qualityOfRoute = Convert.ToDouble(qualityOfRoute + Convert.ToInt32(Math.Sqrt(Math.Pow(xNextCustomer - xCurrentCustomer, 2) + Math.Pow(yNextCustomer - yCurrentCustomer, 2))));
                }
            }

            return qualityOfRoute;
        }
    }
}
