# 🧩 Practica - API REST .NET con JWT (Autenticación y Autorización)

**Proyecto:** API_CAPITAL_MANAGEMENT  
**Autor:** GieziAdael  
**Rol:** Backend Developer (.NET Junior)  
**Correo:** giezi.tlaxcoapan@gmail.com  
**Fecha:** Noviembre 2025  
**Video del Proyecto** https://www.youtube.com/watch?v=RU0sVjW-b4I

---

## 📘 Descripción General

Este proyecto es una **práctica personal de nivel junior** donde se implementa una **API RESTful básica** con **autenticación y autorización usando JWT (JSON Web Tokens)**.  
El objetivo principal fue reforzar conceptos de **arquitectura en capas**, **buenas prácticas de desarrollo** y **seguridad aplicada en APIs .NET**.

Esta API permite:
- Crear y gestionar usuarios.
- Autenticarse mediante un token JWT.
- Controlar el acceso a los endpoints según roles (`Owner`, `Admin`, `Viewer`).
- Gestionar entidades de ejemplo (`User`, `Employee`, `Movement`, `Organization`).

---

## 🚀 Características principales

1. **Arquitectura modular** con separación de capas:  
   `Controllers`, `Repositories`, `IRepositories`, `Data`, `Dtos`, `Mapper`.
2. **Integración de Entity Framework Core** con `AppDbContext` y SQL Server LocalDB.
3. **Repositorio genérico y patrón de inyección de dependencias (DI)**.
4. **AutoMapper** para transformar entidades y DTOs.
5. **JWT completo**: autenticación, autorización y control de roles.
6. **Swagger UI** configurado para probar endpoints y autenticación por token.
7. **Uso de tareas asíncronas (`async/await`)** para operaciones de base de datos.

---

## 🔒 Lógica de Autenticación y Roles

| Acción | Endpoint | Roles permitidos | Requiere Token |
|--------|-----------|------------------|----------------|
| Crear usuario | `POST /api/User/Register` | Todos | No |
| Iniciar sesión | `POST /api/User/Login` | Todos | No |
| Actualizar password | `PUT /api/User/ModifyMyPassword/{newPassword}` | Todos | Sí |
| Eliminar usuario | `DELETE /api/User/DeleteMyAccount` | Todos | Sí |

### 🏢 Organization

| Acción | Endpoint | Roles permitidos | Requiere Token |
|--------|-----------|------------------|----------------|
| Ver mis organizaciones | `GET /api/Organization/MyOrganizations` | Todos | Sí |
| Ver mis afiliaciones | `GET /api/Organization/Others` | Todos | Sí |
| Crear organización | `POST /api/Organization/Create` | Owner | Sí |
| Actualizar contraseña | `PUT /api/Organization/UpdatePassword/{orgId}/{newPassword}` | Owner | Sí |
| Iniciar sesión en organización | `POST /api/Organization/Login` | Owner, Admin, Viewer | Si |
| Eliminar organización | `DELETE /api/Organization/Delete/{orgId}` | Owner | Sí |

### 👥 Employee

| Acción | Endpoint | Roles permitidos | Requiere Token |
|--------|-----------|------------------|----------------|
| Ver miembros de organización | `GET /api/Employee/Members/{OrgId}` | Owner, Admin, Viewer | Sí |
| Agregar miembro | `POST /api/Employee/AddMember/{OrgId}` | Owner | Sí |
| Actualizar rol de miembro | `PUT /api/Employee/UpdateRoleMember/{OrgId}` | Owner | Sí |
| Eliminar miembro | `DELETE /api/Employee/RemoveMember/{OrgId}` | Owner | Sí |

### 📦 Movement

| Acción | Endpoint | Roles permitidos | Requiere Token |
|--------|-----------|------------------|----------------|
| Ver movimientos por organización | `GET /api/Movement/MyMovements/{OrgId}` | Owner, Admin, Viewer | Sí |
| Crear movimiento | `POST /api/Movement/Create/{OrgId}` | Owner, Admin | Sí |
| Calcular balance | `GET /api/Movement/CalculateBalance/{OrgId}` | Owner, Admin, Viewer | Sí |
| Actualizar movimiento | `PUT /api/Movement/Update/{OrgId}/{NoMov}` | Owner, Admin | Sí |
| Eliminar movimiento | `DELETE /api/Movement/Delete/{OrgId}/{NoMov}` | Owner, Admin | Sí |
| Eliminar todos los movimientos | `DELETE /api/Movement/DeleteAllMovements/{OrgId}` | Owner | Sí |

**Roles soportados:**
- `Owner`
- `Admin`
- `Viewer`

---

## 🧰 Tecnologías utilizadas

- **.NET 8.0 / ASP.NET Core Web API**  
- **C# 12**
- **Entity Framework Core**
- **SQL Server LocalDB**
- **AutoMapper**
- **JWT Bearer Authentication**
- **Swagger / Postman**
- **BCrypt**

---

## 🏗️ Arquitectura del Proyecto

```
📦 API_CAPITAL_MANAGEMENT
 ┣ 📂 Controllers
 ┃ ┣ UserController.cs
 ┃ ┣ ObjetoController.cs
 ┃ ┣ EmployeeController.cs
 ┃ ┗ OrganizationController.cs
 ┣ 📂 Data
 ┃ ┗ AppDbContext.cs
 ┣ 📂 Repositories
 ┃ ┣ UserRepo.cs
 ┃ ┣ MovementRepo.cs
 ┃ ┣ EmployeeRepo.cs
 ┃ ┗ OrganizationRepo.cs
 ┣ 📂 Repositories/IRepositories
 ┃ ┣ IUserRepo.cs
 ┃ ┣ IEmployeeRepo.cs
 ┃ ┣ IMovementRepo.cs
 ┃ ┗ IOrganizationRepo.cs
 ┣ 📂 Dtos
 ┣ 📂 Mapper
 ┣ appsettings.json
 ┗ Program.cs
```

---

## ⚙️ Ejecución del Proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/GieziAdael/Practica1_API-JWT.git
   ```
2. Restaura dependencias:
   ```bash
   dotnet restore
   ```
3. Configura la base de datos local (SQL Server LocalDB).
4. **Configura la Secret Key JWT** (no debe ir en el repositorio):
   ```bash
   dotnet user-secrets set "ApiSettings:SecretKey" "TU_SECRETO_SUPER_SEGURO"
   ```
5. Ejecuta el proyecto:
   ```bash
   dotnet run
   ```
6. Abre Swagger:
   ```
   https://localhost:5001/swagger/index.html
   ```
   Desde aquí puedes probar endpoints, generar tokens y autenticarlos.

---


---

## 🧠 Aprendizaje y conclusiones personales

📅 **Septiembre, 2025:**  
Inicio del estudio sobre autenticación JWT en APIs REST mediante un curso de Udemy.  

📅 **Noviembre, 2025:**  
Desarrollo de esta práctica desde cero para reforzar conceptos de:
- Arquitectura limpia y modular.
- Buenas prácticas de inyección de dependencias.
- Separación lógica entre controladores, repositorios y modelos.
- Implementación manual de JWT (creación, firma, validación y autorización).
- Gestión básica de roles.

🧩 A partir de este ejercicio comprendí cómo una API profesional gestiona:
1. La estructura y orden de los componentes.
2. La responsabilidad de cada capa.
3. La forma segura en que se autentica y autoriza el acceso.

📅 **Historial de novedades:** 
1. Se desarrollo la API REST con 33 repositorios y 18 endpoints, subiendolo a GitHub el día 06/11/2025
2. Se implemento una enpoint faltante, quedando en total 19 endpoints, se implemento caché con middleware y documentacion el día 07/11/2025
3. Se implemento dos endpoints (Actualizar Password para el USUARIO y Obtener un listado de Organizaciones en las que el USUARIO se encuentra afilidado) el día 13/11/2025
4. Se agrego documentacion visual y grafica, un archivo .excalidraw el día 25/11/25

📣**Ultimas novedades:** 
1. Se agrego una capa de Servicios, quedando un proyecto de API REST mas profesional y mejor organizada 09/12/2025

📅**Proximament:** 
1. Agregar un Hub para WebSockets, chat en tiempo real para empleados de la organizacion - Fecha prevista de lanzamiento (a mediadios - finales Diciembre)
---

## 🧑‍💻 Autor

**Giezi Adael**  
📫 **giezi.tlaxcoapan@gmail.com**  
💻 Backend Developer (.NET Junior)  
🌐 Proyecto educativo: Implementación de Autenticación y Autorización JWT en APIs RESTful  
📅 Noviembre 2025
⏯️ https://www.youtube.com/watch?v=RU0sVjW-b4I

---
