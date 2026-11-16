CREATE DATABASE MSP_CentralizacionMultiTenant;
GO

USE MSP_CentralizacionMultiTenant;
GO

-- ============================================================================
-- 1. TABLA: Provincias (Tenants / Unidades de aislamiento)
-- ============================================================================
CREATE TABLE Provincias (
    ProvinciaId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(10) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    RegionSalud NVARCHAR(80) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO

-- ============================================================================
-- 2. TABLA: Usuarios (Soporta 3 Roles + Control de Bloqueo por Fallos)
-- ============================================================================
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    ProvinciaId INT NULL, -- NULL únicamente para AdminNacional
    NombreCompleto NVARCHAR(150) NOT NULL,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(32) NOT NULL,
    Rol VARCHAR(30) NOT NULL, -- 'AdminNacional' | 'AdminProvincial' | 'Consultor'
    IntentosFallidos INT NOT NULL DEFAULT 0,
    Bloqueado BIT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    UltimoAcceso DATETIME2 NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Usuarios_Provincias FOREIGN KEY (ProvinciaId) 
        REFERENCES Provincias(ProvinciaId),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('AdminNacional', 'AdminProvincial', 'Consultor')),
    CONSTRAINT CK_Usuarios_Tenant CHECK (
        (Rol = 'AdminNacional' AND ProvinciaId IS NULL) OR
        (Rol IN ('AdminProvincial', 'Consultor') AND ProvinciaId IS NOT NULL)
    )
);
GO

-- ============================================================================
-- 3. TABLA: Tipos_Establecimientos (Catálogo Global Estandarizado)
-- ============================================================================
CREATE TABLE Tipos_Establecimientos (
    TipoEstablecimientoId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(15) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO

-- ============================================================================
-- 4. TABLA: Establecimientos (Aislada por Provincia / Tenant)
-- ============================================================================
CREATE TABLE Establecimientos (
    EstablecimientoId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(25) NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    ProvinciaId INT NOT NULL,
    TipoEstablecimientoId INT NOT NULL,
    Direccion NVARCHAR(250) NOT NULL,
    Municipio NVARCHAR(100) NOT NULL,
    Telefono VARCHAR(25) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UsuarioCreacionId INT NOT NULL,
    FechaModificacion DATETIME2 NULL,
    UsuarioModificacionId INT NULL,

    CONSTRAINT FK_Establecimientos_Provincias FOREIGN KEY (ProvinciaId) 
        REFERENCES Provincias(ProvinciaId),
    CONSTRAINT FK_Establecimientos_Tipos FOREIGN KEY (TipoEstablecimientoId) 
        REFERENCES Tipos_Establecimientos(TipoEstablecimientoId),
    CONSTRAINT FK_Establecimientos_UsuarioCreacion FOREIGN KEY (UsuarioCreacionId) 
        REFERENCES Usuarios(UsuarioId)
);
GO

CREATE UNIQUE INDEX UX_Establecimientos_Codigo ON Establecimientos(Codigo) WHERE Activo = 1;
CREATE INDEX IX_Establecimientos_Provincia ON Establecimientos(ProvinciaId, Activo);
GO

-- ============================================================================
-- 5. TABLA INHERENTE: Bitacora (Auditoría de operaciones del sistema)
-- ============================================================================
CREATE TABLE Bitacora (
    BitacoraId BIGINT IDENTITY(1,1) PRIMARY KEY,
    FechaHora DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UsuarioId INT NULL,
    NombreUsuario VARCHAR(50) NOT NULL,
    Rol VARCHAR(30) NULL,
    ProvinciaId INT NULL,
    Modulo VARCHAR(50) NOT NULL,
    Accion VARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL
);
GO

-- ============================================================================
-- 6. TABLA INHERENTE: AlertasSeguridadTenant (Monitor de Seguridad y Aislamiento)
-- ============================================================================
CREATE TABLE AlertasSeguridadTenant (
    AlertaId BIGINT IDENTITY(1,1) PRIMARY KEY,
    FechaHora DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UsuarioOrigen VARCHAR(50) NOT NULL,
    ProvinciaInvolucradaId INT NULL,
    CategoriaAlerta VARCHAR(60) NOT NULL, -- 'VIOLACION_AISLAMIENTO', 'BLOQUEO_CREDENCIALES', 'INTENTO_FALLIDO'
    NivelRiesgo VARCHAR(20) NOT NULL,     -- 'Informativo', 'Advertencia', 'Critico'
    MensajeTecnico NVARCHAR(500) NOT NULL,
    Resuelta BIT NOT NULL DEFAULT 0
);
GO

-- ============================================================================
-- 7. SEGURIDAD MULTI-TENANT NATIVA EN SQL SERVER (ROW-LEVEL SECURITY)
-- ============================================================================
CREATE SCHEMA Security;
GO

CREATE FUNCTION Security.fn_TenantFilter(@ProvinciaId INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_result
WHERE 
    CAST(SESSION_CONTEXT(N'IsAdminNacional') AS BIT) = 1
    OR @ProvinciaId = CAST(SESSION_CONTEXT(N'TenantProvinciaId') AS INT)
    OR (SESSION_CONTEXT(N'TenantProvinciaId') IS NULL AND SESSION_CONTEXT(N'IsAdminNacional') IS NULL);
GO

CREATE SECURITY POLICY Security.EstablecimientosPolicy
ADD FILTER PREDICATE Security.fn_TenantFilter(ProvinciaId) ON dbo.Establecimientos,
ADD BLOCK PREDICATE Security.fn_TenantFilter(ProvinciaId) ON dbo.Establecimientos AFTER INSERT,
ADD BLOCK PREDICATE Security.fn_TenantFilter(ProvinciaId) ON dbo.Establecimientos AFTER UPDATE
WITH (STATE = ON);
GO

-- ============================================================================
-- 1. VISTAS (Encapsulan los JOINs complejos para la Capa de Datos)
-- ============================================================================
CREATE OR ALTER VIEW vw_EstablecimientosDetalle AS
SELECT 
    e.EstablecimientoId,
    e.Codigo,
    e.Nombre,
    e.ProvinciaId,
    p.Nombre AS Provincia,
    p.RegionSalud,
    e.TipoEstablecimientoId,
    t.Nombre AS TipoEstablecimiento,
    e.Direccion,
    e.Municipio,
    ISNULL(e.Telefono, 'N/A') AS Telefono,
    e.Activo
FROM Establecimientos e
INNER JOIN Provincias p ON e.ProvinciaId = p.ProvinciaId
INNER JOIN Tipos_Establecimientos t ON e.TipoEstablecimientoId = t.TipoEstablecimientoId;
GO

CREATE OR ALTER VIEW vw_ConsolidadoNacional AS
SELECT 
    p.ProvinciaId,
    p.Codigo AS CodigoProvincia,
    p.Nombre AS Provincia,
    p.RegionSalud,
    COUNT(e.EstablecimientoId) AS TotalEstablecimientos,
    ISNULL(SUM(CASE WHEN e.Activo = 1 THEN 1 ELSE 0 END), 0) AS Activos,
    ISNULL(SUM(CASE WHEN e.Activo = 0 THEN 1 ELSE 0 END), 0) AS Inactivos
FROM Provincias p
LEFT JOIN Establecimientos e ON p.ProvinciaId = e.ProvinciaId
GROUP BY p.ProvinciaId, p.Codigo, p.Nombre, p.RegionSalud;
GO

CREATE OR ALTER VIEW vw_UsuariosDetalle AS
SELECT 
    u.UsuarioId,
    u.ProvinciaId,
    u.NombreCompleto,
    u.NombreUsuario,
    u.PasswordHash,
    u.PasswordSalt,
    u.Rol,
    u.IntentosFallidos,
    u.Bloqueado,
    u.Activo,
    u.UltimoAcceso,
    u.FechaCreacion,
    ISNULL(p.Nombre, N'Nivel Nacional') AS NombreProvincia,
    ISNULL(p.Activo, 1) AS ProvinciaActiva
FROM Usuarios u
LEFT JOIN Provincias p ON u.ProvinciaId = p.ProvinciaId;
GO

-- ============================================================================
-- 2. PROCEDIMIENTOS ALMACENADOS (Operaciones Transaccionales Seguras)
-- ============================================================================

-- SP 1: Listar Establecimientos con filtros dinámicos + RLS automático
CREATE OR ALTER PROCEDURE sp_Establecimientos_Listar
    @ProvinciaId INT = NULL,
    @TipoId INT = NULL,
    @Activo BIT = NULL,
    @Texto NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM vw_EstablecimientosDetalle
    WHERE (@ProvinciaId IS NULL OR ProvinciaId = @ProvinciaId)
      AND (@TipoId IS NULL OR TipoEstablecimientoId = @TipoId)
      AND (@Activo IS NULL OR Activo = @Activo)
      AND (@Texto IS NULL OR Nombre LIKE '%' + @Texto + '%' OR Codigo LIKE '%' + @Texto + '%' OR Municipio LIKE '%' + @Texto + '%')
    ORDER BY EstablecimientoId ASC;
END;
GO

-- SP 2: Insertar Establecimiento
CREATE OR ALTER PROCEDURE sp_Establecimientos_Insertar
    @Codigo VARCHAR(25),
    @Nombre NVARCHAR(150),
    @ProvinciaId INT,
    @TipoId INT,
    @Direccion NVARCHAR(250),
    @Municipio NVARCHAR(100),
    @Telefono VARCHAR(25) = NULL,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Establecimientos (Codigo, Nombre, ProvinciaId, TipoEstablecimientoId, Direccion, Municipio, Telefono, Activo, UsuarioCreacionId)
    VALUES (UPPER(TRIM(@Codigo)), TRIM(@Nombre), @ProvinciaId, @TipoId, TRIM(@Direccion), TRIM(@Municipio), NULLIF(TRIM(@Telefono), ''), 1, @UsuarioId);
END;
GO

-- SP 3: Actualizar Establecimiento
CREATE OR ALTER PROCEDURE sp_Establecimientos_Actualizar
    @Id INT,
    @Codigo VARCHAR(25),
    @Nombre NVARCHAR(150),
    @ProvinciaId INT,
    @TipoId INT,
    @Direccion NVARCHAR(250),
    @Municipio NVARCHAR(100),
    @Telefono VARCHAR(25) = NULL,
    @Activo BIT,
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Establecimientos
    SET Codigo = UPPER(TRIM(@Codigo)),
        Nombre = TRIM(@Nombre),
        ProvinciaId = @ProvinciaId,
        TipoEstablecimientoId = @TipoId,
        Direccion = TRIM(@Direccion),
        Municipio = TRIM(@Municipio),
        Telefono = NULLIF(TRIM(@Telefono), ''),
        Activo = @Activo,
        FechaModificacion = SYSDATETIME(),
        UsuarioModificacionId = @UsuarioId
    WHERE EstablecimientoId = @Id;
END;
GO

-- SP 4: Registrar Usuario (Usado desde el Login inicial y desde el Módulo Usuarios)
CREATE OR ALTER PROCEDURE sp_Usuarios_Insertar
    @ProvinciaId INT = NULL,
    @NombreCompleto NVARCHAR(150),
    @NombreUsuario VARCHAR(50),
    @PasswordHash VARBINARY(64),
    @PasswordSalt VARBINARY(32),
    @Rol VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Usuarios (ProvinciaId, NombreCompleto, NombreUsuario, PasswordHash, PasswordSalt, Rol, Activo)
    VALUES (@ProvinciaId, TRIM(@NombreCompleto), TRIM(@NombreUsuario), @PasswordHash, @PasswordSalt, @Rol, 1);
END;
GO

-- SP 5: Registrar en Bitácora
CREATE OR ALTER PROCEDURE sp_Bitacora_Insertar
    @UsuarioId INT = NULL,
    @NombreUsuario VARCHAR(50),
    @Rol VARCHAR(30) = NULL,
    @ProvinciaId INT = NULL,
    @Modulo VARCHAR(50),
    @Accion VARCHAR(50),
    @Descripcion NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Bitacora (UsuarioId, NombreUsuario, Rol, ProvinciaId, Modulo, Accion, Descripcion)
    VALUES (@UsuarioId, @NombreUsuario, @Rol, @ProvinciaId, @Modulo, @Accion, @Descripcion);
END;
GO

-- SP 6: Registrar Alerta de Seguridad Tenant
CREATE OR ALTER PROCEDURE sp_Alertas_Insertar
    @UsuarioOrigen VARCHAR(50),
    @ProvinciaInvolucradaId INT = NULL,
    @CategoriaAlerta VARCHAR(60),
    @NivelRiesgo VARCHAR(20),
    @MensajeTecnico NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AlertasSeguridadTenant (UsuarioOrigen, ProvinciaInvolucradaId, CategoriaAlerta, NivelRiesgo, MensajeTecnico)
    VALUES (@UsuarioOrigen, @ProvinciaInvolucradaId, @CategoriaAlerta, @NivelRiesgo, @MensajeTecnico);
END;
GO