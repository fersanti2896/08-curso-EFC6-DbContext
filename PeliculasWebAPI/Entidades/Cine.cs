using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace PeliculasWebAPI.Entidades {
    public class Cine {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }

        /* Propiedades de navegación, entidades de relación */
        public CineOferta CineOferta { get; set; }

        /* HashSet es una colección */
        public HashSet<SalaCine> SalaCine { get; set; }

        /* Propieda de navegación */
        public CineDetalle CineDetalle { get; set; }

        /* Entidad de propiedad */
        public Direccion Direccion { get; set; }
    }
}
