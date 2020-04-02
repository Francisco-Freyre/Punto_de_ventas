using LinqToDB.Mapping;
using Punto_de_ventas.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Punto_de_ventas.models
{
    public class Productos 
    {
        [PrimaryKey, Identity]

        public int IdProducto { set; get; }

        public string Codigo { set; get; }

        public string Descripcion { set; get; }

        public decimal Costo { set; get; }

        public decimal PrecioVenta { set; get; }

        public int Existencia { set; get; }

        public int Minimo { set; get; }

        public string Departamento { set; get; }

        public string Categoria { set; get; }

        public string Fecha { set; get; }

        public string Hora { set; get; }
    }
}
