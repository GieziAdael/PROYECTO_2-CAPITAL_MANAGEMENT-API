# 🧩 Practica - API REST .NET con JWT (Autenticación y Autorización)

**Proyecto:** API_CAPITAL_MANAGEMENT  
**Autor:** GieziAdael  
**Rol:** Backend Developer (.NET Junior)  
**Correo:** giezi.tlaxcoapan@gmail.com  
**Fecha:** Octubre 2025  
**Curso de referencia:** Implementación de JWT en API REST (Udemy)

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

| Acción | Endpoint | Roles permitidos |
|--------|-----------|------------------|
| Crear usuario | `POST /api/User/Create` | Todos |
| Iniciar sesión | `POST /api/User/Login` | Todos |
| Obtener lista de usuarios | `GET /api/User/GetAll` | admin |
| Obtener usuario por ID | `GET /api/User/Get/{id}` | admin |
| Actualizar email | `PATCH /api/User/ActualizarEmail/{id}/{email}` | Todos |
| Eliminar cuenta | `DELETE /api/User/Delete` | Todos |
| Ver objetos | `GET /api/Objeto/GetAll` | Todos |
| Crear / Eliminar objeto | `POST /api/Objeto/Create`, `DELETE /api/Objeto/Delete` | admin, modd |
| Actualizar objeto | `PATCH /api/Objeto/ActualizarNombre/{id}/{name}` | admin, modd |

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
   https://localhost:5001/swagger
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

---

## 🧑‍💻 Autor

**Giezi Adael**  
📫 **giezi.tlaxcoapan@gmail.com**  
💻 Backend Developer (.NET Junior)  
🌐 Proyecto educativo: Implementación de Autenticación y Autorización JWT en APIs RESTful  
📅 Octubre 2025

---
