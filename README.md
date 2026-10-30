# SGCAE Minería

Backend REST para gestionar empleados, credenciales y accesos físicos de una empresa minera con tres sedes:

- Mina Guayabito
- Mina Guaico
- Relavera

El proyecto aplica **Clean Architecture**, modelado de dominio (**DDD**) y separación de escritura/lectura (**CQRS**), con ASP.NET Core 8, Entity Framework Core y SQL Server 2022.

## Alcance funcional

| Requerimiento | Implementación |
|---|---|
| RF-01 Empleados | Registro, consulta paginada, actualización y desactivación lógica |
| RF-02 Credenciales | Asignación, consulta, bloqueo y revocación irreversible |
| RF-03 Entradas y salidas | Validación y registro de accesos autorizados **y rechazados** |
| RF-04 Historial | Filtros por empleado, fechas, punto, resultado y paginación |
| RF-06 Puntos de acceso | Registro, consulta y habilitación/deshabilitación por sede |

La numeración conserva el enunciado original, que no incluye un RF-05.

## Arquitectura

```text
HTTP / Swagger
      │
      ▼
 Sgcae.Api ───────────────┐
      │                   │ composición y DI
      ▼                   ▼
 Sgcae.Application ◄── Sgcae.Infrastructure
      │                   │
      ▼                   ▼
 Sgcae.Domain          SQL Server
```

- **Domain:** entidades, objeto de valor `CorreoElectronico`, enumeraciones, invariantes y transiciones de estado.
- **Application:** comandos, consultas, DTO, casos de uso y puertos de persistencia. No conoce Entity Framework.
- **Infrastructure:** `DbContext`, configuraciones SQL Server y repositorios.
- **Api:** controladores, contratos HTTP, Swagger y manejo uniforme de errores con `ProblemDetails`.

Las dependencias apuntan hacia el dominio. Los comandos modifican estado y las consultas usan rutas independientes y `AsNoTracking()`.

## Reglas de negocio principales

- La identificación y el correo de cada empleado son únicos.
- Un empleado inactivo no puede recibir una credencial ni ingresar a una sede.
- El código de credencial es único y un empleado solo puede tener una credencial vigente.
- Una credencial revocada no puede regresar a otro estado.
- Solo autorizan el acceso una credencial vigente, un empleado activo y un punto habilitado.
- Todo intento queda registrado, incluso con credencial o punto inexistente.
- La paginación se limita a 100 registros por solicitud.

## Ejecución rápida con Docker

Requisito: Docker Desktop o Docker Engine con Compose.

```bash
docker compose up --build
```

Servicios disponibles:

- Swagger: <http://localhost:5080/swagger>
- Estado de la API: <http://localhost:5080/health>
- SQL Server: `localhost:1433`

Al iniciar por primera vez, EF Core crea el esquema y carga las tres sedes y estos puntos:

| Código | Sede |
|---|---|
| `GYA-PORTERIA` | Mina Guayabito |
| `GUA-PORTERIA` | Mina Guaico |
| `REL-INGRESO` | Relavera |

> La contraseña incluida en `docker-compose.yml` es solo para desarrollo académico. En un despliegue real debe almacenarse en secretos y rotarse.

Para detener los contenedores:

```bash
docker compose down
```

Use `docker compose down -v` únicamente si también desea eliminar el volumen y todos los datos de prueba.

## Ejecución con .NET instalado

Requisitos: SDK .NET 8 y SQL Server accesible.

```bash
dotnet restore Sgcae.sln
dotnet run --project src/Sgcae.Api
```

La conexión se configura en `src/Sgcae.Api/appsettings.json` o mediante la variable:

```bash
ConnectionStrings__Sgcae="Server=...;Database=SgcaeMineria;..." dotnet run --project src/Sgcae.Api
```

## Endpoints

| Método | Ruta | Uso |
|---|---|---|
| `POST` | `/api/empleados` | Registrar empleado |
| `GET` | `/api/empleados/{id}` | Consultar empleado |
| `GET` | `/api/empleados` | Listar y filtrar empleados |
| `PUT` | `/api/empleados/{id}` | Actualizar empleado |
| `PATCH` | `/api/empleados/{id}/desactivar` | Desactivar empleado |
| `POST` | `/api/credenciales` | Asignar credencial |
| `GET` | `/api/credenciales/{id}` | Consultar una credencial |
| `GET` | `/api/credenciales` | Consultar credenciales |
| `PATCH` | `/api/credenciales/{id}/bloquear` | Bloquear credencial |
| `PATCH` | `/api/credenciales/{id}/revocar` | Revocar credencial |
| `POST` | `/api/accesos` | Registrar entrada o salida |
| `GET` | `/api/accesos/historial` | Consultar historial paginado |
| `GET` | `/api/sedes` | Consultar sedes |
| `POST` | `/api/puntos-acceso` | Registrar punto |
| `GET` | `/api/puntos-acceso/{id}` | Consultar un punto |
| `GET` | `/api/puntos-acceso` | Consultar puntos |
| `PATCH` | `/api/puntos-acceso/{id}/estado` | Habilitar/deshabilitar punto |

Swagger contiene los esquemas y permite ejecutar cada operación. También se incluye una secuencia reproducible en [`docs/SGCAE.http`](docs/SGCAE.http).

### Ejemplo de acceso

```json
POST /api/accesos
{
  "codigoCredencial": "TARJETA-0001",
  "codigoPuntoAcceso": "GYA-PORTERIA",
  "tipo": "Entrada"
}
```

Respuesta posible:

```json
{
  "movimientoId": "d9dbd865-20c9-4f37-a580-faf7fa0bdb28",
  "resultado": "Autorizado",
  "motivo": "Acceso autorizado.",
  "registradoEnUtc": "2026-10-30T02:30:00Z"
}
```

## Pruebas y evidencias

```bash
dotnet test Sgcae.sln
```

Las pruebas unitarias verifican invariantes del dominio. El flujo de GitHub Actions compila la solución, ejecuta las pruebas y publica el archivo TRX como artefacto descargable. Para la sustentación se recomienda conservar como evidencias:

1. ejecución verde del workflow `Integracion continua`;
2. Swagger mostrando los endpoints;
3. respuesta autorizada y respuesta rechazada de `/api/accesos`;
4. historial filtrado que muestre ambos resultados;
5. tablas creadas en SQL Server.

## Estructura

```text
src/
├── Sgcae.Domain
├── Sgcae.Application
├── Sgcae.Infrastructure
└── Sgcae.Api
tests/
└── Sgcae.Domain.Tests
docs/
└── SGCAE.http
```

## Decisiones y límites

- `EnsureCreated` facilita una demostración reproducible sin herramientas locales; para producción se deben generar y versionar migraciones de EF Core.
- Autenticación de operadores, permisos por rol, integración biométrica y reglas de autorización distintas por cargo/sede no forman parte de los requerimientos suministrados.
- Los tiempos se almacenan en UTC; el cliente puede presentarlos en la zona `America/Bogota`.

## Licencia

Proyecto académico. Puede agregarse una licencia institucional según las condiciones de entrega.
