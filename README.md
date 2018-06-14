# InfoGeek

## Atención, este manual está hecho para usuarios de windows.

## Introducción
InfoGeek es una de prueba que he creado para estudiar las bases no relacionales del tipo documental, en este caso, MongoDB.

## Descripción
InfoGeek pretende ser una aplicación que una empresas y profesionales de la informática.

La aplicación cuenta con 4 roles principales:

  -**Admin**: Es el administrador de la página web. Administra el catálogo de categorías y puede borrar banners inapropiados, además, cuenta con un panel de control con la información más relevante del sistema.
  -**User**: Uno de los dos usuarios principales del sistema. Un usuario puede suscribirse a una oferta de empleo, que significa que esa oferta guardará el currículum de dicho usuario para que la empresa pueda verlo. Por otra parte, tiene la posibilidad de comentar novedades de su sector mediante posts y comentar posts de otros usuarios.
  -**Enterprise**: Tienen la posibilidad de crear ofertas de trabajo para sus startups y elegir al mejor candidato de los que se suscriben a sus ofertas de empleo.
  -**Sponsor**: Los sponsors pueden poner banners en la página, los cuales se muestran aleatoriamente.

## Requisitos necesarios
Para que funcione el proyecto debes de tener instalados los componentes siguientes:
  -.Net Core
  -Editor de texto enriquecido o Visual Studio (se recomienda este último).
  -MongoDB

Además, se recomienda tener MongoDBCompass para poder ver la base de datos y modificarla a tu gusto.

## Iniciar MongoDB
Una vez instalado MongoDB debemos de iniciarlo, para ello debemos de acceder a C:\Program Files\MongoDB\Server\3.6\bin (a no ser que hayas elegido otro directorio durante la instalación), y ejecutar el siguiente comando:

```
mongod
```

## Importar la base de datos
La aplicación funciona perfectamente sin una base de datos importada, sin embargo, es necesario que para realizar algunas acciones como administrador del sistema.

Para ello, descarga la carpeta de la rama "database" y ejecuta el comando siguiente:

```
mongorestore -d infoGeek "directorio donde está la carpeta"
```

## Importar el proyecto
Para importar el proyecto, simplemente necesitas el archivo .sln en el caso de que tengas Visual Studio.

## Notas adicionales
En la rama "documentation" existe un documento donde se explica con más detalle como es la aplicación y su finalidad.
