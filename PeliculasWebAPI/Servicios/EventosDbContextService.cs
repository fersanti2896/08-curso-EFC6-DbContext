using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PeliculasWebAPI.Servicios {
    public class EventosDbContextService : IEventosDbContextService {
        private readonly ILogger<EventosDbContextService> logger;

        public EventosDbContextService(ILogger<EventosDbContextService> logger){
            this.logger = logger;
        }

        public void ManejarTracked(object sender, EntityTrackedEventArgs args) {
            var msg = $"Entidad: { args.Entry.Entity } | Estado: { args.Entry.State }";
            logger.LogInformation(msg);
        }

        public void ManejarStateChange(object sender, EntityStateChangedEventArgs args) {
            var msg = $@"Entidad: { args.Entry.Entity } | Estado Anterior: { args.OldState } |
                         Estado Nuevo: { args.NewState }";
            logger.LogInformation(msg);
        } 
    }
}
