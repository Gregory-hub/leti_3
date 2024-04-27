using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    internal class Shape
    {
        public List<Polygon> Polygons { get; }

        public Shape()
        {
            Polygons = new List<Polygon>();
        }
    }
}

