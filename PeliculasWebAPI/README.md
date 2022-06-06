# Resumen de la sección 8: DbContext
___

El DbContext es una parte principal de Entity Framework, el cual tienen algunas propiedades importantes: 

- _Database_. Nos permite realizar funcionalidades como transacciones, creación y migraciones de bases de datos así como querys arbitrarios. 

- _Change Tracker_. Se encarga de dar el seguimiento de cambios de las instancia de entidades en nuestra aplicación. 

- _Model_. Nos permite tener acceso a la base de datos. 

- _ContextId_. Es un identificador único de cada instancia del DbContext, sirve para login o logueo. 

1. __OnConfiguring.__
2. __Cambiando status de una entidad.__
3. __Actualizando algunas propiedades.__
4. __Sobreescribiendo SaveChanges.__
5. __Inyección de Dependencias en DbContext.__
6. __Eventos Tracked y StateChanged.__

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

#### Actualizando algunas propiedades

Si queremos actualizar todas las columnas de una tabla, podemos hacer que solo se actualizan las columnas que queremos como modificadas. 

En `ActoresController.cs` si solo queremos actualizar el campo nombre pero los demás campos dejarlos como están, podemos hacer el código: 

    context.Entry(actor).Property(a => a.Nombre)
                                .IsModified = true;

![actoresController](/PeliculasWebAPI/images/actoresController.png)

Si modificamos el actor con _id: 6_, dandole el nombre _Tom Hanks_

![actorput](/PeliculasWebAPI/images/actoresput.PNG)

Inicialmente nuestra base de datos se encontraba con el registro que queriamos cambiar. 

![actores](/PeliculasWebAPI/images/actores1.PNG)

Al actualizar solo su campo _Nombre_, tenemos el resultado siguiente, donde solo se actualiza la propiedad mencionada, mientras que los demás campos del _id: 6_ se quedan como estaban.

![actores2](/PeliculasWebAPI/images/actores2.PNG)

#### Sobreescribiendo SaveChanges

Podemos sobreescribir el método `SaveChanges` para poder realizar cambios de una manera centralizada. 

Creamos una clase abstracta `EntidadAuditable.cs` que va herederar nuestras entidades.

![entidadAuditable](/PeliculasWebAPI/images/EntidadAuditable.png)

El cual nuestra entidad `Genero.cs` va heredar

![EntidadGenero](/PeliculasWebAPI/images/GeneroHeredaEntidadAuditable.png)

En nuestro `ApplicactionDBContext.cs` creamos un método que va hacer el proceso de agrega y modificar las propiedades que tienen nuestro `EntidadAuditable.cs`.

![DbContext](/PeliculasWebAPI/images/SavaChangesDbContext.png)

Al probar creando un nuevo genero, tenemos un status `200`.

![nuevoGenero](/PeliculasWebAPI/images/generosavachenges.PNG)

Al verificar nuestra Base de Datos, vemos que se agregó el registro pero con las columnas de nuestra clase `EntidadAuditable.cs`

![generos1](/PeliculasWebAPI/images/genero1.PNG)

Al actualiza el mismo registro, tenemos un status `200`.

![actualizarGenero](/PeliculasWebAPI/images/genero%20put.PNG)

Al verificar el registro en nuestra Base de Datos, vemos que la columna `UsuarioModificacion` fue lo que se actualizó además de la columna `Nombre`.

![generos2](/PeliculasWebAPI/images/genero2.PNG)

#### Inyección de Dependencias en DbContext

Si queremos modificar nuestro código anterior envés de código duro al querer registrar el id de un usuario podemos crear un servicio que se inyecte en nuestro `ApplicationDBContext.cs`.

Creamos nuestra interfaz `IUsuarioService.cs`.

![interfazUsuario](/PeliculasWebAPI/images/IUsuarioService.png)

Creamos nuestro servicio `UsuarioService.cs`.

![usuarioService](/PeliculasWebAPI/images/UsuarioService.png)

En nuestro `Program.cs` registramos la interfaz y el servicio. 

![registroIS](/PeliculasWebAPI/images/Registro%20de%20interfaz%20y%20servicio.png)

Y en nuestro `ApplicationDBContext.cs` inyectamos la dependencia del servicio. 

![inyeccionService](/PeliculasWebAPI/images/inyectandoDependencia.png)

Y al modificar nuestro método con la inyección de la dependecia. 

![inyeccionService2](/PeliculasWebAPI/images/inyectandoDependencia%202.png)

Y al probar al registrar un nuevo genero, tenemos un status `200`. 

![nuevoGenero](/PeliculasWebAPI/images/generoPostIP.PNG)

Al verificar en nuestra Base de Datos, vemos que tenemos el mismo resultado.

![generosBD3](/PeliculasWebAPI/images/generos%20BD.PNG)

#### Eventos Tracked y StateChanged

