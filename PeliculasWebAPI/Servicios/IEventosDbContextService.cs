using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PeliculasWebAPI.Servicios {
    public interface IEventosDbContextService {
        void ManejarTracked(object sender, EntityTrackedEventArgs args);
        void ManejarStateChange(object sender, EntityStateChangedEventArgs args);
    }
}
