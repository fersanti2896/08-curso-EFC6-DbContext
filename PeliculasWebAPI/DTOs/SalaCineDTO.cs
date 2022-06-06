using PeliculasWebAPI.Entidades;

namespace PeliculasWebAPI.DTOs {
    public class SalaCineDTO {
        public decimal Precio { get; set; }
        public TipoSalaCine TipoSalaCine { get; set; }
    }
}
