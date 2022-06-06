# Resumen de la sección 8: DbContext
___

El DbContext es una parte principal de Entity Framework, el cual tienen algunas propiedades importantes: 

- _Database_. Nos permite realizar funcionalidades como transacciones, creación y migraciones de bases de datos así como querys arbitrarios. 

- _Change Tracker_. Se encarga de dar el seguimiento de cambios de las instancia de entidades en nuestra aplicación. 

- _Model_. Nos permite tener acceso a la base de datos. 

- _ContextId_. Es un identificador único de cada instancia del DbContext, sirve para login o logueo. 

1. __OnConfiguring.__
2. __Cambiando status de una entidad.__

#### OnConfiguring

Cuando configuramos nuestro DbContext, lo hacemos desde `Program.cs`, pero existe otra forma de hacerlo y es a través desde `ApplicationDBContext.cs`.

De esta forma si ya existe una configuracion del DbContext en nuestro `Program.cs` condicionamos esta configuraciones para que no vuelva a configurar el DbContext en `ApplicationDBContext.cs`.

![onconfiguring](/PeliculasWebAPI/images/onconfiguring.png)

#### Cambiando el status de una entidad

Si bien podemos hacer cambios con _Create_, _Update_ y _Delete_ a una entidad, también podemos darle otra configuración de _Entry_ para cambiar su status. 

Por ejemplo, desde `GenerosController.cs` en el método `POST` cuando creamos un nuevo género, envés de usar:

    context.Add(genero);

El cual actualiza su status, podemos implementar:

    context.Entry(genero).State = EntityState.Added;

![entry-state](/PeliculasWebAPI/images/EntryState.png)

Al hacer el agregado, tenemos un status `200` el cual si agrego el nuevo género. 

![entry-state-ejecucion](/PeliculasWebAPI/images/EntryState%20desde%20Swagger.PNG)