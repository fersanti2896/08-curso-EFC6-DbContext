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
7. __Eventos SaveChanges.__
8. __Queries Arbitrarios.__
9. __Sentencias Arbitrarias.__
10. __ToSqlQuery - Centralizando Queries Arbitrarios.__
11. __Procedimientos Almacenados.__
12. __Introducción a Transacciones.__
13. __BeginTransaction - Una transacción para varios SaveChanges.__

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

_Tracked_. Se ejecuta cuando Entity Framework le empieza dar seguimiento a una entidad. 

_StateChanged_. Se ejecuta cuando el status de la entidad que ya tiene seguimiento ha cambiado. 

Para ello creamos nuestra interfaz `IEventosDbContextService.cs`.

![InterfazEventos](/PeliculasWebAPI/images/IEventosDbContextService.png)

También creamos nuestro servicio `EventosDbContextService.cs` que va heredar de nuestra interfaz que hemos creado. 

![ServicioEventos](/PeliculasWebAPI/images/EventosDbContextService.png)

Registramos tanto la interfaz como nuestro servicio en nuestro `Program.cs`.

![registro](/PeliculasWebAPI/images/Registrando%20Interfaz%20y%20Servicio%20Tracked.png)

Y en nuestro constructor de nuestro `ApplicationDBContext.cs` inyectamos nuestra dependencia de nuestro evento. 

![inyeccionEvento](/PeliculasWebAPI/images/Constructor%20DbContext%20Tracked.png)

Esta configuración no se disparan cuando no se usa el `AsNotTracking`, por ello siempre debe tener `Tracking`, en nuestro ejemplo de `Generos` que si lo tiene, al hacer la consulta de un genero nos devuelve el siguiente resultado: 

![consultaGenero](/PeliculasWebAPI/images/Consulta%20generos%20GET.PNG)

Vista en consola nos devuelve las configuraciones que hicimos en nuestro servicio `EventosDbContextsService.cs`.

![consultaGeneroCmd](/PeliculasWebAPI/images/Consulta%20generos%20GET%202%20Consola.PNG)

Al agregar un genero dispara el segundo método que habíamos configurado: 

![insercionGenero](/PeliculasWebAPI/images/Insercion%20Genero%20Tracked.PNG)

Vista en consola: 

![insercionGeneroCmd](/PeliculasWebAPI/images/Insercion%20Genero%20Tracked%20Consola.PNG)

#### Eventos SaveChanges

Tenemos otros métodos para guardar cambios:

 - Uno se dispara antes de guardar los cambios.
 - Otro que se dispara después de guardar los cambios.
 - Otro que se dispara si durante el guardado de los cambios da un error. 

Por lo cual, en nuestro `EventosDbContextService.cs` configuramos 3 métodos para los eventos mencionados, estos métodos están declarados también en nuestro `IEventosDbContextService.cs`.

![metodosSaveChnages](/PeliculasWebAPI/images/SaveChanges%20Mas%20Metodos.png)

Y en nuestro `ApplicationDBContext.cs` inyectamos estos métodos en nuestro constructor. 

![SaveChangesConstructor](/PeliculasWebAPI/images/SaveChanges%20en%20DbContext.png)

Al probar registrando 3 nuevos géneros, nos devuelve un status `200`. 

![tresgeneros](/PeliculasWebAPI/images/SavedChanges%20Resultado.PNG)

En nuestra consola nos devuelve lo que implementandos en nuestros métodos. 

![SaveChangesCmd](/PeliculasWebAPI/images/SavedChanges.PNG)

#### Queries Arbitrarios

Cuando no podemos hacer consultas que queremos, podemos definir queries arbitrarios en Entity Framework. 

Por ejemplo en `GenerosController.cs` podemos implementar un querie arbitrario ya sea con interporlación o no. 

![QuerieArbiGeneros](/PeliculasWebAPI/images/QuerieArbitrarioGenerosController.png)

Como resultado tenemos un status `200` 

![QuerieResult](/PeliculasWebAPI/images/QuerieArbitrarioResultado.PNG)

Si lo vemos desde la consola, vemos que se implementó la sentencia SQL. 

![QuerieResultCmd](/PeliculasWebAPI/images/QuerieArbitrarioResultado%20Consola.PNG)

En `CinesController.cs` de igual forma aplicamos un querie arbitrario a una endpoint. 

![QuerieArbiCines](/PeliculasWebAPI/images/QuerieArbitrarioCinesController.png)

Como resutado obtenemos un status `200` de la consulta.

![QuerieResult2](/PeliculasWebAPI/images/QuerieArbitrarioResultado%202.PNG)

Si lo vemos desde la consola, notamos la implementación de SQL. 

![QueriResultCmd2](/PeliculasWebAPI/images/QuerieArbitrarioResultado%20Consola%202.PNG)

#### Sentencias Arbitrarias

Si queremos insertar, actualizar o eliminar, podemos hacer uso de una sentencia arbitraria, por ejemplo, en nuestro `GenerosController.cs` se hace una sentencia SQL para crear un nuevo genero. 

![nuevoGenero](/PeliculasWebAPI/images/NuevoGenero.png)

Probamos desde el `endpoint` para crear un nuevo genero, el cual nos devuelve un status `200`. 

![nuevoGeneroSwagger](/PeliculasWebAPI/images/NuevoGeneroSwagger.PNG)

Verificando desde nuestra Base de Datos.

![GeneroBD](/PeliculasWebAPI/images/NuevoGeneroBD.PNG)

#### ToSqlQuery - Centralizando Queries Arbitrarios

Podemos mapear un querie arbitrario a una clase específica. 

Desde nuestro `ApplicationDBContext.cs` podemos definir un querie arbitrario. 

![querieArbitrario](/PeliculasWebAPI/images/ToSqlQuery%20DbContext.png)

Probando desde nuestro `endpoint` de Películas. 

![resultadoQuerie](/PeliculasWebAPI/images/ToSqlQuery%20Resultado.PNG)

#### Procedimientos Almacenados

Para poder ejecutar Procedimiento Almacenado desde Entity Framework, debemos tener un PA en una migración. 

Definimos nuestro procedimientos almacenados através de una migración, en donde vamos a consultar un genero por su _id_ e insertar un genero. 

![ProcAlmMigracion](/PeliculasWebAPI/images/ProcAlm%20Migracion.png)

En nuestro `GenerosController.cs` crearemos dos endpoints para nuestro procedimientos almacenados. 

Para el `endpoint: api/generos/ProcAlm/{id}` de tipo `GET` definimos el método en nuestro controlador. 

![procAlmGet](/PeliculasWebAPI/images/ProcAlm%20Get.png)

Probando el `endpoint` nos devuelve un status `200`: 

![procAlmResult](/PeliculasWebAPI/images/ProcAlm%20Result%201.PNG)

Para el `endpoint: api/generos/ProcAlm` de tipo `POST` definimos el método en nuestro controlador. 

![procAlmGet](/PeliculasWebAPI/images/ProcAlm%20Post.png)

Probando el `endpoint` nos devuelve un status `200`: 

![procAlmResult](/PeliculasWebAPI/images/ProcAlm%20Result%202.PNG)

Visto en nuestra Base de Datos, notamos que se registró el Género con éxito. 

![GenerosBDProcAlm](/PeliculasWebAPI/images/generos%20BD%20Proc%20Alm.PNG)

#### Introducción a Transacciones

Las transacciones se usan para que todas las tareas se ejecuten existosamente, de lo contrario ninguna se ejecutará aunque alguna tarea sea existosa. 

En nuestro caso `Cine.cs` y `SalaCine.cs` si quieremos insertar un Cine con su Sala de Cine, debemos hacer dos operaciones, es decir, la de registrar el cine y sus salas de cines.

En nuestro `GenerosController.cs` ya tenemos un `endpoint` que usa una transacción con el método `SaveChangesAsync()`.

![variosGenerosTrans](/PeliculasWebAPI/images/VariosGenerosTransc.png)

Lo que nos indica que si agregramos una colección de Generos y mientras uno sea incorrecto, los demás aunque sea existosos, no se ejecutará correctamente la petición y por lo tanto tampoco se replicará en la Base de Datos. 

Por lo que en el `endpoint` nos devuelve un error `500`.

![GenerosTransc](/PeliculasWebAPI/images/GenerosTransc.PNG)

#### BeginTransaction - Una transacción para varios SaveChanges

En ocasiones cuando se ejecuta una transacción y la primera es existosa, pero la segunda no, se debe revertir todo el proceso, para ello se usa el `BeginTransaction`.

Con `BeginTransaction` se puede cubrir varios `SaveChanges()`.

Si queremos tener dos entidades que no queremos que se relaciones, podemos usar `BeginTransaction`, por lo cual creamos dos entidades `Factura.cs` y `FacturaDetalle.cs`

`Factura.cs`

![factura](/PeliculasWebAPI/images/Factura.png)

`FacturaDetalle.cs`

![facturaDetalle](/PeliculasWebAPI/images/FacturaDetalle.png)

Al hacer la relación normal entre las dos entidades en `FacturaConfig.cs`.

![facturaConfig](/PeliculasWebAPI/images/FacturaConfig.png)

Posteriormente hacemos la migración y empujamos los cambios hacia la Base de Datos. 

![facturaMigracion](/PeliculasWebAPI/images/FacturaMigracion.png)

Creamos nuestro `FacturaController.cs` y un `endpoint` que creará una factura. 

![facturapost](/PeliculasWebAPI/images/PostFactura.png)

En primera instancia sino se tiene la línea 

    /* Simula Error */
    throw new ApplicationException("Esto es una prueba");

Y al hacer la petición `POST` nos devuelve un status `200` donde se registraron tanto la factura como su detalle. 

![facturaResult](/PeliculasWebAPI/images/FacturaResult.PNG)

En nuestra Base de Datos persisten los registros que creamos. 

![facturasDB](/PeliculasWebAPI/images/Select%20Facturas.PNG)

Pero simulamos el error incluyendo el código en nuestro método `POST`.

    /* Simula Error */
    throw new ApplicationException("Esto es una prueba");

Al hacer la posteo de los registros, nos devuelve un status `400` donde revierte los cambios realizados, es decir, no se ejecuta ninguna acción. 

![facturaResultError](/PeliculasWebAPI/images/FacturaResult%20Error.PNG)