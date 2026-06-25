# Guardarropa - Sistema de Gestion de Guardarropa

## Descripcion

Aplicacion de escritorio para Windows para gestionar un servicio de guardarropa (coat check).
Permite gestionar tickets de prendas, configurar empresas, precios, impresoras de tickets,
y llevar un historico de cierres diarios (Z).

## Stack Tecnologico

- **Framework**: WPF (.NET 8)
- **Lenguaje**: C#
- **Base de datos**: SQLite (via Entity Framework Core)
- **UI**: Material Design con MaterialDesignInXAML
- **Impresion**: System.Drawing.Printing (impresoras de tickets/recibos)
- **PDF**: QuestPDF para exportacion de informes

## Estructura del Proyecto

```
guardarropa/
  src/
    Guardarropa/              # Proyecto principal WPF
      Models/                 # Modelos de datos
      ViewModels/             # ViewModels (MVVM)
      Views/                  # Ventanas y controles XAML
      Services/               # Logica de negocio (impresion, BD, etc.)
      Data/                   # DbContext y migraciones EF Core
      Assets/                 # Recursos (iconos, fuentes)
      App.xaml                # Punto de entrada
  tests/
    Guardarropa.Tests/        # Tests unitarios
```

## Flujo Principal

1. **Primer inicio**: Pedir contrasena maestra (no hay datos previos)
2. **Inicio normal**: Seleccionar empresa -> Pantalla principal
3. **Pantalla principal**: Introducir numero de prendas -> Imprimir ticket
4. **Ticket**: Nombre empresa, fecha/hora, numero de ticket (centrado, grande), precio total
5. **Cierre Z**: Requiere contrasena especifica, resetea contador de tickets, genera informe

## Funcionalidades

### Empresas
- CRUD de empresas desde ajustes
- Seleccion de empresa al iniciar

### Tickets
- Numero de prendas por ticket
- Precio = precio_por_prenda * numero_prendas
- Numero de ticket autoincremental (se resetea con la Z)
- Impresion en impresora de tickets

### Configuracion (protegida por contrasena)
- Contrasena maestra (se establece en primer inicio, modificable despues)
- Precio por prenda
- Empresas
- Impresora: seleccion de impresora, anchura, longitud, margenes, tamano de textos
- Generar contrasena para sacar la Z
- Historico de cierres Z (fecha, tickets emitidos, dinero recaudado)

### Cierre Z
- Requiere contrasena generada desde ajustes
- Pone contadores a 0
- Genera listado (ticket o PDF): tickets del dia, dinero recaudado

## Comandos

```bash
# Compilar
dotnet build src/Guardarropa/Guardarropa.csproj

# Ejecutar
dotnet run --project src/Guardarropa/Guardarropa.csproj

# Tests
dotnet test tests/Guardarropa.Tests/Guardarropa.Tests.csproj

# Publicar (release)
dotnet publish src/Guardarropa/Guardarropa.csproj -c Release -r win-x64 --self-contained
```

## Convenciones

- Patron MVVM estricto
- Nombres de clases y metodos en PascalCase
- Nombres de variables locales en camelCase
- Nombres de campos privados con prefijo _
- Idioma del codigo (variables, clases, metodos): espanol
- Idioma de la UI: espanol
- Commits en espanol
