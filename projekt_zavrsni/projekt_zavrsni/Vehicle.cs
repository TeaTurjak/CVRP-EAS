using System;
using System.Collections.Generic;
using System.Text;

namespace projekt_zavrsni
{
    class Vehicle
    {
        public int capacityOfVehicle;

        public Vehicle()
        {

        }

        public Vehicle(int capacity)
        {
            capacityOfVehicle = capacity;
        }

        public void setVehicleCapacity(int capacityToSet)
        {
            capacityOfVehicle = capacityToSet;
        }

        public int getVehicleCapacity()
        {
            return capacityOfVehicle;
        }

        public int removeForCustomerCapacity(int customerDemand)
        {
            capacityOfVehicle = capacityOfVehicle - customerDemand;
            return capacityOfVehicle;
        }
    }
}
