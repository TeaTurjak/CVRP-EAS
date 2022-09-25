using System;
using System.Collections.Generic;
using System.Text;

namespace projekt_zavrsni
{
    class Location
    {
        public int indexOfLocation;
        public int xCoordinate;
        public int yCoordinate;
        public int demandOfLocation;
        public Location()
        {

        }

        public Location(int index, int x, int y, int demand)
        {
            indexOfLocation = index;
            xCoordinate = x;
            yCoordinate = y;
            demandOfLocation = demand;
        }

        public int getindexOfLocation()
        {
            return indexOfLocation;
        }

        public int getXCoordinate()
        {
            return xCoordinate;
        }

        public int getYCoordinate()
        {
            return yCoordinate;
        }

        public int getDemandOfLocation()
        {
            return demandOfLocation;
        }

    }
}
