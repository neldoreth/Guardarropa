# Guardarropa - Sistema de Gestion de Guardarropa

## Descripcion

Aplicacion de escritorio para Windows para gestionar un servicio de guardarropa (coat check).
Permite emitir tickets de prendas (perchas), configurar empresas, precios e impresoras de
tickets, y llevar un historico de cierres diarios (Z).

## Stack Tecnologico

- **Framework**: WPF (.NET 8, `net8.0-windows`)
- **Lenguaje**: C#
- **MVVM**: CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`, `ObservableObject`)
- **Base de datos**: SQLite via Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`)
- **Impresion**: System.Drawing.Common / System.Drawing.Printing (impresoras de tickets/recibos)
- **Cultura**: `es-ES` (moneda en euros, fechas en espanol)
- **Temas**: claro / oscuro mediante `ResourceDictionary` intercambiable en tiempo de ejecucion

## Almacenamiento de datos

- La base de datos SQLite se crea en: `%LOCALAPPDATA%\Guardarropa\guardarropa.db`
- Se usa `EnsureCreated()` (NO hay migraciones). Si cambia el esquema de los modelos,
  hay que borrar el `.db` para que se vuelva a crear:
  `Remove-Item "$env:LOCALAPPDATA\Guardarropa\guardarropa.db"`
- Tablas: `Configuraciones`, `Empresas`, `Tickets`, `CierresZ`

## Estructura del Proyecto

```
guardarropa/
  installer/                 # Instalador Windows (Inno Setup)
    Guardarropa.iss          # Script del instalador (Setup.exe)
    build.ps1                # Publica la app y compila el instalador en un paso
    output/                  # Setup.exe generado (no versionado)
  src/
    Guardarropa/              # Proyecto principal WPF
      Models/                 # Modelos de datos (Empresa, Ticket, Configuracion, CierreZ)
      ViewModels/             # ViewModels (MVVM)
      Views/                  # Ventanas y controles XAML (+ code-behind)
      Services/               # Logica de negocio (BaseDatosServicio, ImpresionServicio,
                               #   DiarioServicio para el diario de tickets en .txt)
      Data/                   # GuardarropaDbContext (EF Core)
      Assets/                 # TemaClaro.xaml / TemaOscuro.xaml (recursos de tema) e icono.ico
      AppInfo.cs              # Nombre, version y changelog del programa (ventana Acerca de)
      App.xaml                # Punto de entrada, cultura y tema
      MainWindow.xaml         # Ventana host; intercambia UserControls
```

## Conceptos clave: Percha vs. Ticket

Son dos contadores DISTINTOS:

- **Numero de percha**: numero visible y editable en la pantalla principal. Se reinicia a 1
  con cada cierre Z (por empresa). Es el numero que identifica la prenda entregada.
- **Numero de ticket (contador total)**: contador global acumulado que NO se reinicia con la Z.
  Solo se puede poner a cero manualmente desde Ajustes, con confirmacion (accion irreversible).
  Se guarda en `Configuracion.ContadorTickets`.

## Flujo Principal

1. **Primer inicio**: se establece la contrasena maestra (no hay datos previos).
2. **Inicio normal**: seleccionar empresa -> pantalla principal.
3. **Pantalla principal**: numero de percha (editable) + numero de prendas (+/-) -> total
   calculado -> boton grande "IMPRIMIR PERCHA". La tecla Intro tambien imprime (el foco
   siempre esta en el TextBox de percha para garantizarlo). Se muestra la fecha actual.
4. **Cierre Z**: requiere la contrasena Z, reinicia el contador de perchas y genera un
   informe imprimible (perchas emitidas, prendas, dinero recaudado).

## Ticket impreso (de arriba a abajo)

1. Nombre de la empresa
2. Calle + numero
3. Ciudad + provincia
4. Fecha / hora
5. "Nº. Percha" + el numero de percha (en grande)
6. Nº de prendas y precio por prenda
7. TOTAL en euros
8. Numero de ticket (contador total)

## Funcionalidades

### Empresas
- CRUD desde Ajustes (alta, edicion y baja logica via `Activa`).
- Campos: nombre, calle, numero, ciudad, provincia.
- Seleccion de empresa al iniciar la aplicacion.

### Configuracion (protegida por contrasena maestra)
Organizada en 4 pestanas. "GUARDAR CAMBIOS" guarda y cierra la vista.

- **General**: tema claro/oscuro (tiempo real), precio por prenda, CRUD de empresas (con direccion).
- **Contrasena**: contrasena Z (editable), cambiar contrasena maestra.
- **Cierre Z**: contador total de tickets (con boton de reinicio irreversible), acceso al historico.
- **Impresora**: seleccion de impresora, ancho/alto (mm), margenes (mm), tamanos de texto.
- Ademas hay un boton "Abrir carpeta del diario" (ver Diario de tickets, abajo).

### Cierre Z
- Requiere la contrasena Z.
- Reinicia el contador de perchas (no el contador total de tickets).
- Genera informe imprimible: perchas, prendas y dinero recaudado.

### Diario de tickets
- Cada percha impresa se registra automaticamente (`DiarioServicio`) en un archivo `.txt`
  diario por empresa, guardado en `%LOCALAPPDATA%\Guardarropa\diario\`.
- Cada linea tiene hora, numero de percha, prendas y total; la ultima linea del dia
  acumula el total de perchas, prendas y dinero.

### Restablecimiento de contrasena maestra
- Si en la carpeta del ejecutable existe un `reset.txt` con la linea `password=reset`,
  al iniciar la app se muestra la pantalla de restablecimiento en vez del login normal.
- Al guardar la nueva contrasena se borra `reset.txt` automaticamente.

### Acerca de
- Abajo a la derecha de la pantalla principal se muestra el numero de version (`AppInfo.Version`).
- Al hacer clic se abre una ventana "Acerca de" con el nombre del programa, el desarrollador
  (ClickEZ Solutions) y el historial de cambios (`AppInfo.Historial`).
- Al publicar una nueva version hay que actualizar `AppInfo.Version`/`AppInfo.Historial`
  Y `MyAppVersion` en `installer/Guardarropa.iss` (no estan sincronizados automaticamente).

## Comandos

```powershell
# IMPORTANTE: las herramientas no estan en PATH por defecto en este equipo
$env:PATH = "C:\Program Files\dotnet;C:\Program Files\Git\cmd;C:\Program Files\GitHub CLI;$env:PATH"

# Compilar
dotnet build src/Guardarropa/Guardarropa.csproj

# Ejecutar
dotnet run --project src/Guardarropa/Guardarropa.csproj

# Generar el instalador Windows (Setup.exe autocontenido, requiere Inno Setup 6)
powershell -File installer/build.ps1
```

## Instalador (Windows)

- `installer/build.ps1` publica la app como `.exe` self-contained (win-x64, single-file,
  no requiere .NET instalado en el equipo destino) y compila `installer/Guardarropa.iss`
  con Inno Setup 6 (`%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe`).
- El resultado es `installer/output/Guardarropa-Setup-<version>.exe`: instala en
  `%LOCALAPPDATA%\Programs\Guardarropa` (sin necesitar permisos de administrador),
  crea accesos directos en el menu inicio y (opcional) en el escritorio, e incluye
  desinstalador.
- Recordar mantener sincronizada la version entre `AppInfo.Version` (C#) y
  `MyAppVersion` (`installer/Guardarropa.iss`).

## Convenciones

- Patron MVVM.
- Clases y metodos en PascalCase; variables locales en camelCase; campos privados con prefijo `_`.
- Idioma del codigo (variables, clases, metodos): espanol (sin acentos en identificadores).
- Idioma de la UI: espanol.
- Commits en espanol.
- Los archivos `.cs` con caracteres no ASCII (p. ej. `º` en `ImpresionServicio.cs`) se guardan
  en UTF-8 con BOM para que el compilador los lea correctamente.
