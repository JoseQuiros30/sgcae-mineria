# Diseño técnico y dominio

## Contextos y agregados

El alcance se trata como un contexto delimitado de **Control de Accesos**. `Empleado`, `Credencial` y `PuntoAcceso` mantienen sus propias invariantes; `MovimientoAcceso` es el registro auditable e inmutable del resultado de una solicitud.

```mermaid
erDiagram
    SEDE ||--o{ PUNTO_ACCESO : contiene
    EMPLEADO ||--o{ CREDENCIAL : recibe
    EMPLEADO o|--o{ MOVIMIENTO_ACCESO : realiza
    CREDENCIAL o|--o{ MOVIMIENTO_ACCESO : presenta
    PUNTO_ACCESO o|--o{ MOVIMIENTO_ACCESO : registra

    EMPLEADO {
      uniqueidentifier Id PK
      string Identificacion UK
      string Correo UK
      string Estado
    }
    CREDENCIAL {
      uniqueidentifier Id PK
      string Codigo UK
      string Estado
      uniqueidentifier EmpleadoId FK
    }
    SEDE {
      uniqueidentifier Id PK
      string Codigo UK
      string Nombre
    }
    PUNTO_ACCESO {
      uniqueidentifier Id PK
      string Codigo UK
      uniqueidentifier SedeId FK
      bool Habilitado
    }
    MOVIMIENTO_ACCESO {
      uniqueidentifier Id PK
      string CodigoCredencial
      string CodigoPuntoAcceso
      string Resultado
      string Motivo
      datetime RegistradoEnUtc
    }
```

Las relaciones de movimiento son opcionales para poder auditar un código de credencial o punto inexistente sin violar una llave foránea.

## Flujo CQRS de acceso

```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as AccesosController
    participant H as RegistrarAccesoHandler
    participant R as Repositorios
    participant DB as SQL Server
    C->>API: POST /api/accesos
    API->>H: RegistrarAccesoCommand
    H->>R: Buscar credencial + empleado
    H->>R: Buscar punto + sede
    H->>H: Evaluar reglas
    H->>R: Agregar MovimientoAcceso
    R->>DB: INSERT autorizado/rechazado
    H-->>API: ResultadoAccesoDto
    API-->>C: 200 OK
```

La consulta de historial usa otro handler y una ruta de lectura independiente. No muta entidades y ejecuta `AsNoTracking()`.

## Matriz de validación de acceso

| Condición | Resultado |
|---|---|
| Credencial inexistente | Rechazado |
| Punto inexistente | Rechazado |
| Credencial bloqueada o revocada | Rechazado |
| Empleado inactivo | Rechazado |
| Punto deshabilitado | Rechazado |
| Todas las validaciones correctas | Autorizado |

Cada rama genera un `MovimientoAcceso` y guarda un motivo explícito.
