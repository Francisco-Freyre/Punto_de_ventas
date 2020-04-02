using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Punto_de_ventas.models
{
    public class Tempo_Ventas
    {
        [PrimaryKey, Identity]

        public int IdTempo { set; get; }

        public string Codigo { set; get; }

        public string Descripcion { set; get; }

        public string Precio { set; get; }

        public int Cantidad { set; get; }

        public string Importe { set; get; }

        public int Caja { set; get; }

        public int IdUsuario { set; get; }

        public string Costo { set; get; }

        public string Departamento { set; get; }

        public string Categoria { set; get; }
    }
}
