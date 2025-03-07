Aquí tienes el archivo modificado para un proyecto en .NET:


# Biblioteca-de-arte
Repositorio del proyecto de seguridad 

## Problematica
En la actualidad, los artistas emergentes enfrentan dificultades para exhibir y comercializar sus obras debido a la falta de espacios accesibles y la alta competencia en plataformas tradicionales. Muchas soluciones existentes están orientadas a artistas consolidados, lo que limita la visibilidad y las oportunidades de los creadores en crecimiento.
Nuestro proyecto busca abordar esta problemática mediante el desarrollo de una plataforma web que permita a los artistas exponer sus obras y conectarse directamente con el público sin intermediarios. La plataforma ofrecerá herramientas avanzadas para la gestión, visualización y comercialización de arte digital, optimizando la experiencia tanto para creadores como para consumidores.

## Integrantes
### Sánchez Roano Carlos Alonso
- Correo: carlosroano201@gmail.com
- Usuario de git: Alonso-Roano

### Dolores Ramirez Alexis Daniel
- Correo: alexisdanieldr@gmail.com
- Usuario de git: Maxxired

### Gómez Segura Andrea
- Correo: angomezs.123@gmail.com
- Usuario de git: Goraz23


## Dependencias

### Dependencias principales

- `Microsoft.AspNetCore.Authentication.JwtBearer`: 8.0.2
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`: 8.0.2
- `Microsoft.EntityFrameworkCore.Design`: 8.0.2
- `Microsoft.EntityFrameworkCore.SqlServer`: 8.0.2
- `Serilog.AspNetCore`: 9.0.0
- `Swashbuckle.AspNetCore`: 6.6.2
- `Swashbuckle.AspNetCore.Filters`: 8.0.2
- `System.IdentityModel.Tokens.Jwt`: 8.0.2

### Dependencias de desarrollo

- `Microsoft.NET.Sdk.Web`: Dependencia del SDK de .NET para aplicaciones web.
- `Microsoft.Extensions.Logging`: Para el manejo de logs y trazas en la aplicación.
- `xUnit`: Para realizar pruebas unitarias.

## Pasos para hacer un commit correctamente

### 1. Clonar el repositorio
Si aún no tienes el repositorio en tu máquina, clónalo con:
```sh
 git clone https://github.com/Alonso-Roano/Biblioteca-de-arte-back
```

### 2. Crear una nueva rama
Antes de hacer cambios, crea una nueva rama basada en `main`:
```sh
 git checkout -b nombre-de-la-rama
```
Ejemplo:
```sh
 git checkout -b feature/agregar-funcionalidad
```

### 3. Hacer cambios y agregarlos al staging
Edita los archivos necesarios y luego agrégalos al área de staging:
```sh
 git add archivo1 archivo2
```
O para agregar todos los cambios:
```sh
 git add .
```

### 4. Crear un commit
Una vez agregados los cambios, haz un commit con un mensaje claro y descriptivo:
```sh
 git commit -m "Descripción breve del cambio"
```
Ejemplo:
```sh
 git commit -m "Corrige el bug en la autenticación de usuarios"
```

### 5. Subir la rama al repositorio remoto
Sube tu nueva rama al repositorio en GitHub:
```sh
 git push origin nombre-de-la-rama
```
Ejemplo:
```sh
 git push origin feature/agregar-funcionalidad
```

### 6. Crear un Pull Request
- Ve a tu repositorio en GitHub.
- Dirígete a la pestaña "Pull Requests".
- Haz clic en "New Pull Request".
- Selecciona la rama que acabas de subir.
- Agrega una descripción y solicita revisión.

### 7. Esperar Revisión
Los administradores revisarán tu código y podrán solicitar cambios antes de fusionarlo con `main`.

---

### Notas Importantes
- Sigue el formato de nombres de ramas (`feature/`, `bugfix/`, `hotfix/`).
- Usa commits descriptivos y concisos.
- Asegúrate de actualizar tu `main` antes de crear nuevas ramas:
  ```sh
  git pull origin main
  ```

---

## Cómo correr el proyecto

### 1. Instalar las dependencias
Asegúrate de tener el SDK de .NET instalado en tu máquina. Luego, en la raíz del proyecto, restaura las dependencias con el siguiente comando:
```sh
dotnet restore
```

### 2. Borrar la carpeta de migraciones (si es necesario)
Si has realizado cambios en el modelo de datos o si necesitas empezar de nuevo con las migraciones, puedes borrar la carpeta de migraciones existente. Esta carpeta se encuentra normalmente en la ruta `Migrations/` dentro del proyecto.
```sh
rm -rf Migrations
```

### 3. Crear las migraciones
Genera las migraciones desde el modelo de datos actual ejecutando el siguiente comando:
```sh
dotnet ef migrations add NombreDeLaMigracion
```
**Nota**: Sustituye `NombreDeLaMigracion` por un nombre descriptivo que haga referencia a la actualización que estás realizando.

### 4. Actualizar la base de datos
Después de generar la migración, aplica las migraciones a la base de datos ejecutando:
```sh
dotnet ef database update
```
Esto actualizará la base de datos según las migraciones generadas.

### 5. Ejecutar el servidor de desarrollo
Para correr el proyecto en modo de desarrollo, usa el siguiente comando:
```sh
dotnet run
```
Esto abrirá la aplicación en el navegador de manera local, generalmente en `http://localhost:5000`.

### 6. Abrir la aplicación en el navegador
Una vez que el servidor esté corriendo, puedes ver la aplicación en tu navegador en la dirección indicada anteriormente. Si quieres cambiar el puerto, puedes modificar la configuración en el archivo `launchSettings.json`.
