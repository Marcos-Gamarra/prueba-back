# API REST .NET 10 — Usuarios, Direcciones y Divisas

Backend REST desarrollado en **.NET 10** utilizando **Minimal API**, **Entity Framework Core con SQLite**, validaciones con **FluentValidation**, arquitectura **CQRS (Commands & Queries)** y documentación interactiva vía **Scalar (OpenAPI)**.

---

## 1. Requisitos y Versión de .NET
- **SDK**: [.NET 10 SDK](https://dotnet.microsoft.com/)
- **Base de Datos**: SQLite (archivo `app.db` local)
- **EF CLI** (para aplicar o crear migraciones):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 2. Configuración y Migraciones de Base de Datos

El proyecto utiliza Entity Framework Core con SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) y `EntityFrameworkCore.Exceptions.Sqlite`.

### Aplicar migraciones:
Para crear la base de datos `app.db` y aplicar todas las migraciones:
```bash
dotnet ef database update --project PruebaTecnicaBack.Api
```

### Crear una nueva migración (si se altera el modelo):
```bash
dotnet ef migrations add <NombreMigracion> --project PruebaTecnicaBack.Api
```

> **Nota sobre Concurrencia**: La aplicación inicializa automáticamente SQLite en modo **WAL** (`PRAGMA journal_mode=WAL;`) en el arranque, permitiendo lecturas y escrituras concurrentes sin bloqueos de archivo.

---

## 3. Cómo Ejecutar el Proyecto

Desde la raíz de la solución:
```bash
dotnet run --project PruebaTecnicaBack.Api
```

La API estará disponible en:
- **HTTP**: `http://localhost:5243`
- **HTTPS**: `https://localhost:7007`
- **Documentación Interactiva (Scalar UI)**: `http://localhost:5243/scalar/v1`
- **Acceso Directo Swagger**: `http://localhost:5243/swagger` (redirige automáticamente a Scalar)

---

## 4. Autenticación por API Key

Todos los endpoints de negocio requieren el header `X-API-KEY`.

- **Header Name**: `X-API-KEY`
- **Clave predeterminada de prueba**: `clave-super-secreta`
- **Configuración**: en `appsettings.json` bajo la sección `ApiKey`:
  ```json
  "ApiKey": {
    "HeaderName": "X-API-KEY",
    "SecretKey": "clave-super-secreta"
  }
  ```

### Ejemplos `curl`:
```bash
# Petición autorizada (200 OK)
curl -H "X-API-KEY: clave-super-secreta" http://localhost:5243/users

# Petición sin header o con clave incorrecta (401 Unauthorized)
curl -i http://localhost:5243/users
```

---

## 5. Endpoints Principales

### Usuarios (`/users`)
- `POST /users`: Crear usuario (`{ "name": "Juan", "email": "juan@test.com", "password": "opcional" }`).
- `GET /users`: Listar usuarios (soporta `?isActive=true|false`).
- `GET /users/{id}`: Obtener usuario por Id.
- `PUT /users/{id}`: Actualizar `name`, `email`, `isActive`.
- `DELETE /users/{id}`: Eliminar usuario (eliminación en cascada de sus direcciones).
- `POST /users/bulk`: Carga masiva de usuarios en paralelo.

### Direcciones (`/users/{userId}/addresses` y `/addresses`)
- `POST /users/{userId}/addresses`: Crear dirección (404 si `userId` no existe).
- `GET /users/{userId}/addresses`: Listar direcciones del usuario (404 si `userId` no existe).
- `PUT /addresses/{id}`: Actualizar dirección (`street`, `city`, `country`, `zipCode`).
- `DELETE /addresses/{id}`: Eliminar dirección por Id.

### Monedas y Conversión (`/currencies` y `/currency`)
- `GET /currencies`: Listar monedas disponibles.
- `POST /currencies`: Crear moneda (`{ "code": "USD", "name": "Dólar", "rateToBase": 7300 }`).
- `POST /currency/convert`: Conversión en paralelo con `Task.WhenAll`.
  ```json
  { "fromCurrencyCode": "USD", "toCurrencyCode": "PYG", "amount": 100 }
  ```

---

## 6. Carga Masiva de Usuarios (Bulk) y Paralelismo

El endpoint `POST /users/bulk` procesa la colección de usuarios en paralelo utilizando `Parallel.ForEachAsync` y un `IDbContextFactory<ApplicationDbContext>` para generar un contexto EF Core independiente por cada operación concurrente:

- **MaxDegreeOfParallelism**: acotado y configurable en `appsettings.json` (por defecto `10`).
- **Archivo de prueba incluido**:
  - `sample-data/bulk_users_test.json`
  - `PruebaTecnicaBack.Api/sample-data/bulk_users_test.json`

### Probar Carga Masiva con `curl`:
```bash
curl -X POST http://localhost:5243/users/bulk \
  -H "Content-Type: application/json" \
  -H "X-API-KEY: clave-super-secreta" \
  -d @sample-data/bulk_users_test.json
```

### Respuesta de ejemplo:
```json
{
  "total": 4,
  "successful": 4,
  "failed": 0,
  "errors": []
}
```

---

## 7. Estado de Cumplimiento de Requerimientos

- CRUD de Users: Implementado (POST, GET con filtro opcional isActive, GET por Id, PUT y DELETE).
- CRUD de Addresses: Implementado (relacion 1:N con Users, respuesta 404 si userId no existe y eliminacion en cascada).
- Modulo Currency y Conversion: Implementado (GET y POST currencies, POST currency/convert con ejecucion paralela via Task.WhenAll).
- Seguridad por API Key: Implementado (header X-API-KEY validado en todos los endpoints de negocio, 401 si falta o es incorrecta).
- EF Core + SQLite: Implementado (migraciones, indices unicos, collation NOCASE y modo WAL habilitado).
- FluentValidation: Implementado (validadores asociados a cada endpoint mutante mediante endpoint filter).
- Patron CQRS: Implementado (separacion de Commands y Queries organizados por carpetas en Application).
- Procesamiento en Paralelo: Implementado (concurrencia acotada en carga masiva con Parallel.ForEachAsync y en conversion con Task.WhenAll).
- IDbContextFactory: Implementado (IDbContextFactory registrado y utilizado en lugar de inyeccion scoped de DbContext).
- Endpoint Carga Masiva y JSON de prueba: Implementado (POST /users/bulk listo para usar con archivo sample-data/bulk_users_test.json).
- Swagger / OpenAPI: Implementado (documentacion interactiva Scalar en /scalar/v1 y redireccion automatica desde /swagger).
- Compilacion y Repositorio: Implementado (compilacion limpia en .NET 10 sin errores ni advertencias).
