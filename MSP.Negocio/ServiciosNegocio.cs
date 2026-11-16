using System.Text;
using MSP.Datos;
using MSP.Entidades;

namespace MSP.Negocio
{
    public class AuthService
    {
        private readonly UsuarioRepositorio _usuarioRepo = new();
        private readonly AuditoriaRepositorio _auditoriaRepo = new();

        public bool SistemaRequiereConfiguracionInicial() => !_usuarioRepo.ExisteAlgunUsuario();

        public void RegistrarPrimerAdminNacional(string nombreCompleto, string nombreUsuario, string password)
        {
            if (_usuarioRepo.ExisteAlgunUsuario())
                throw new InvalidOperationException("El sistema ya cuenta con usuarios configurados.");

            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("El nombre completo y el nombre de usuario son obligatorios.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("La contraseña debe tener mínimo 6 caracteres.");

            var (hash, salt) = SeguridadCrypto.CrearHash(password);

            _usuarioRepo.Insertar(new Usuario
            {
                ProvinciaId = null,
                NombreCompleto = nombreCompleto,
                NombreUsuario = nombreUsuario,
                PasswordHash = hash,
                PasswordSalt = salt,
                Rol = "AdminNacional",
                Activo = true
            });

            GestorEventosSistema.Instancia.Notificar("Seguridad", "BOOTSTRAP_ADMIN", $"Se creó el Administrador Nacional inicial '{nombreUsuario}'.");
        }

        public Usuario IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Debe ingresar usuario y contraseña.");

            var usuario = _usuarioRepo.ObtenerPorNombreUsuario(nombreUsuario);
            if (usuario == null)
            {
                _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant
                {
                    UsuarioOrigen = nombreUsuario,
                    CategoriaAlerta = "USUARIO_INEXISTENTE",
                    NivelRiesgo = "Advertencia",
                    MensajeTecnico = $"Intento de acceso con usuario no registrado: '{nombreUsuario}'."
                });
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }

            if (usuario.Bloqueado)
                throw new UnauthorizedAccessException("Esta cuenta está bloqueada por múltiples intentos fallidos. Contacte al Administrador.");

            if (!usuario.Activo)
                throw new UnauthorizedAccessException("Esta cuenta o su provincia asignada se encuentran desactivadas.");

            if (!SeguridadCrypto.VerificarPassword(password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                int nuevosIntentos = usuario.IntentosFallidos + 1;
                bool bloquear = nuevosIntentos >= 3;
                _usuarioRepo.RegistrarIntentoFallido(usuario.UsuarioId, nuevosIntentos, bloquear);

                _auditoriaRepo.RegistrarAlertaSeguridad(new AlertaSeguridadTenant
                {
                    UsuarioOrigen = usuario.NombreUsuario,
                    ProvinciaInvolucradaId = usuario.ProvinciaId,
                    CategoriaAlerta = bloquear ? "BLOQUEO_CREDENCIALES" : "INTENTO_FALLIDO",
                    NivelRiesgo = bloquear ? "Critico" : "Advertencia",
                    MensajeTecnico = bloquear
                        ? $"Cuenta '{usuario.NombreUsuario}' bloqueada tras 3 intentos fallidos."
                        : $"Contraseña incorrecta para '{usuario.NombreUsuario}' (Intento {nuevosIntentos}/3)."
                });

                throw new UnauthorizedAccessException(bloquear
                    ? "Cuenta bloqueada por alcanzar 3 intentos fallidos."
                    : $"Contraseña incorrecta. Intento {nuevosIntentos} de 3.");
            }

            _usuarioRepo.RegistrarLoginExitoso(usuario.UsuarioId);
            TenantContext.Actual.IniciarSesion(usuario);
            GestorEventosSistema.Instancia.Notificar("Autenticación", "INICIO_SESION", $"Inicio de sesión exitoso ({TenantContext.Actual.DescripcionAlcance}).");

            return usuario;
        }

        public void CambiarPasswordPropia(string passwordActual, string passwordNueva)
        {
            var u = TenantContext.Actual.UsuarioActual ?? throw new InvalidOperationException("No hay sesión activa.");
            var uDb = _usuarioRepo.ObtenerPorNombreUsuario(u.NombreUsuario)!;

            if (!SeguridadCrypto.VerificarPassword(passwordActual, uDb.PasswordHash, uDb.PasswordSalt))
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");

            if (passwordNueva.Length < 6)
                throw new ArgumentException("La nueva contraseña debe tener al menos 6 caracteres.");

            var (hash, salt) = SeguridadCrypto.CrearHash(passwordNueva);
            _usuarioRepo.ActualizarPassword(u.UsuarioId, hash, salt);
            GestorEventosSistema.Instancia.Notificar("Seguridad", "CAMBIO_CLAVE", "El usuario actualizó su contraseña.");
        }

        public void CerrarSesion()
        {
            if (TenantContext.Actual.EstaAutenticado)
            {
                GestorEventosSistema.Instancia.Notificar("Autenticación", "CIERRE_SESION", "Cierre de sesión del usuario.");
                TenantContext.Actual.CerrarSesion();
            }
        }
    }

    public class EstablecimientoService
    {
        private readonly EstablecimientoRepositorio _repo = new();

        public List<EstablecimientoDto> Listar(int? provinciaFiltroUI, int? tipoId, bool? activo, string? texto)
        {
            var estrategia = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            int? provinciaReal = estrategia.ResolverFiltroProvincia(provinciaFiltroUI);
            return _repo.Listar(provinciaReal, tipoId, activo, texto);
        }

        public void Guardar(Establecimiento est)
        {
            var v1 = new ValidadorPermisosTenantHandler();
            var v2 = new ValidadorCamposObligatoriosHandler();
            var v3 = new ValidadorUnicidadCodigoHandler();
            v1.EstablecerSiguiente(v2).EstablecerSiguiente(v3);
            v1.Validar(est);

            if (est.EstablecimientoId == 0)
            {
                _repo.Insertar(est);
                GestorEventosSistema.Instancia.Notificar("Establecimientos", "CREAR", $"Se registró '{est.Nombre}' ({est.Codigo}).");
            }
            else
            {
                _repo.Actualizar(est);
                GestorEventosSistema.Instancia.Notificar("Establecimientos", "EDITAR", $"Se actualizó '{est.Nombre}' ({est.Codigo}).");
            }
        }

        public List<ConsolidadoProvinciaDto> ObtenerConsolidado()
        {
            var estrategia = FabricaEstrategiaRol.ObtenerEstrategiaActual();
            int? provinciaReal = estrategia.ResolverFiltroProvincia(null);
            return _repo.ObtenerConsolidado(provinciaReal);
        }
    }

    public class AdministracionService
    {
        private readonly UsuarioRepositorio _usuarioRepo = new();
        private readonly CatalogoRepositorio _catalogoRepo = new();
        private readonly AuditoriaRepositorio _auditoriaRepo = new();

        public List<Usuario> ListarUsuarios()
        {
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _usuarioRepo.Listar(provFiltro);
        }

        public void CrearUsuario(string nombreCompleto, string nombreUsuario, string password, string rol, int? provinciaId)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios)
                throw new UnauthorizedAccessException("No tiene permisos para crear usuarios.");

            if (TenantContext.Actual.EsAdminProvincial)
            {
                if (rol == "AdminNacional")
                    throw new UnauthorizedAccessException("Un Administrador Provincial no puede crear Administradores Nacionales.");
                provinciaId = TenantContext.Actual.TenantProvinciaId;
            }

            if (rol == "AdminNacional") provinciaId = null;
            else if (!provinciaId.HasValue)
                throw new ArgumentException("Debe asignar una provincia para los roles Provincial o Consultor.");

            if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(nombreUsuario) || password.Length < 6)
                throw new ArgumentException("Complete todos los campos y use una contraseña de al menos 6 caracteres.");

            var (hash, salt) = SeguridadCrypto.CrearHash(password);
            _usuarioRepo.Insertar(new Usuario
            {
                ProvinciaId = provinciaId,
                NombreCompleto = nombreCompleto,
                NombreUsuario = nombreUsuario,
                PasswordHash = hash,
                PasswordSalt = salt,
                Rol = rol
            });

            GestorEventosSistema.Instancia.Notificar("Usuarios", "CREAR", $"Se creó el usuario '{nombreUsuario}' con rol '{rol}'.");
        }

        public void ActualizarEstadoUsuario(int usuarioId, bool activo, bool desbloquear)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarUsuarios)
                throw new UnauthorizedAccessException("Sin permisos para gestionar usuarios.");

            _usuarioRepo.CambiarEstadoBloqueoOActivo(usuarioId, activo, desbloquear);
            GestorEventosSistema.Instancia.Notificar("Usuarios", "ESTADO", $"Se actualizó estado/bloqueo del usuario ID={usuarioId}.");
        }

        public List<Provincia> ListarProvincias(bool soloActivas = false) => _catalogoRepo.ListarProvincias(soloActivas);

        public void CrearProvincia(string codigo, string nombre, string regionSalud)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede registrar provincias.");

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(regionSalud))
                throw new ArgumentException("Todos los campos de la provincia son obligatorios.");

            _catalogoRepo.InsertarProvincia(new Provincia { Codigo = codigo, Nombre = nombre, RegionSalud = regionSalud });
            GestorEventosSistema.Instancia.Notificar("Provincias", "CREAR", $"Se registró la provincia '{nombre}' ({codigo}).");
        }

        public void CambiarEstadoProvincia(int provinciaId, bool activo)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede activar o desactivar provincias.");

            _catalogoRepo.CambiarEstadoProvincia(provinciaId, activo);
            GestorEventosSistema.Instancia.Notificar("Provincias", activo ? "ACTIVAR" : "DESACTIVAR", $"Cambio de estado en Provincia ID={provinciaId}.");
        }

        public List<TipoEstablecimiento> ListarTipos(bool soloActivos = false) => _catalogoRepo.ListarTipos(soloActivos);

        public void CrearTipo(string codigo, string nombre, string descripcion)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede crear tipos de establecimientos.");

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Código y Nombre son obligatorios.");

            _catalogoRepo.InsertarTipo(new TipoEstablecimiento { Codigo = codigo, Nombre = nombre, Descripcion = descripcion });
            GestorEventosSistema.Instancia.Notificar("TiposEstablecimientos", "CREAR", $"Se creó el tipo '{nombre}'.");
        }

        public void CambiarEstadoTipo(int tipoId, bool activo)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeGestionarCatalogosGlobales)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede modificar tipos.");

            _catalogoRepo.CambiarEstadoTipo(tipoId, activo);
            GestorEventosSistema.Instancia.Notificar("TiposEstablecimientos", "ESTADO", $"Cambio de estado en Tipo ID={tipoId}.");
        }

        public List<BitacoraRegistro> ListarBitacora()
        {
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _auditoriaRepo.ListarBitacora(provFiltro);
        }

        public List<AlertaSeguridadTenant> ListarAlertas()
        {
            int? provFiltro = FabricaEstrategiaRol.ObtenerEstrategiaActual().ResolverFiltroProvincia(null);
            return _auditoriaRepo.ListarAlertas(provFiltro);
        }

        public void ResolverAlerta(long alertaId)
        {
            if (!FabricaEstrategiaRol.ObtenerEstrategiaActual().PuedeResolverAlertasSeguridad)
                throw new UnauthorizedAccessException("Solo el Administrador Nacional puede marcar alertas como resueltas.");

            _auditoriaRepo.MarcarAlertaResuelta(alertaId);
            GestorEventosSistema.Instancia.Notificar("SeguridadTenant", "RESOLVER_ALERTA", $"Alerta de seguridad ID={alertaId} marcada como resuelta.");
        }
    }

    public static class ExportadorBitacoraService
    {
        public static void Exportar(List<BitacoraRegistro> registros, string rutaArchivo, string formato)
        {
            var sb = new StringBuilder();
            string alcance = TenantContext.Actual.DescripcionAlcance;

            switch (formato.ToUpper())
            {
                case "EXCEL":
                    sb.AppendLine("ID;Fecha y Hora;Usuario;Rol;Modulo;Accion;Descripcion");
                    foreach (var r in registros)
                        sb.AppendLine($"{r.BitacoraId};{r.FechaHora:yyyy-MM-dd HH:mm:ss};{r.NombreUsuario};{r.Rol};{r.Modulo};{r.Accion};\"{r.Descripcion.Replace("\"", "'")}\"");
                    File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(true));
                    break;

                case "WORD":
                    sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word'>");
                    sb.AppendLine("<head><meta charset='utf-8'><title>Bitácora MSP</title></head><body style='font-family:Segoe UI, Arial;'>");
                    sb.AppendLine("<h2 style='color:#0D5C63;'>MINISTERIO DE SALUD PÚBLICA - BITÁCORA DE AUDITORÍA</h2>");
                    sb.AppendLine($"<p><b>{alcance}</b> | Generado: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
                    sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse; width:100%; font-size:10pt;'>");
                    sb.AppendLine("<tr style='background:#0D5C63; color:white;'><th>ID</th><th>Fecha/Hora</th><th>Usuario</th><th>Rol</th><th>Módulo</th><th>Acción</th><th>Descripción</th></tr>");
                    foreach (var r in registros)
                        sb.AppendLine($"<tr><td>{r.BitacoraId}</td><td>{r.FechaHora:dd/MM/yyyy HH:mm}</td><td>{r.NombreUsuario}</td><td>{r.Rol}</td><td>{r.Modulo}</td><td>{r.Accion}</td><td>{r.Descripcion}</td></tr>");
                    sb.AppendLine("</table></body></html>");
                    File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
                    break;

                case "PDF":
                    GenerarPdfNativo(registros, rutaArchivo, alcance);
                    break;

                default:
                    sb.AppendLine("==========================================================================");
                    sb.AppendLine(" MINISTERIO DE SALUD - REPORTE OFICIAL DE BITÁCORA DE AUDITORÍA");
                    sb.AppendLine($" {alcance} | Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    sb.AppendLine("==========================================================================");
                    foreach (var r in registros)
                        sb.AppendLine($"[{r.FechaHora:yyyy-MM-dd HH:mm:ss}] [{r.Modulo} - {r.Accion}] Usuario: {r.NombreUsuario} ({r.Rol}) -> {r.Descripcion}");
                    File.WriteAllText(rutaArchivo, sb.ToString(), Encoding.UTF8);
                    break;
            }

            GestorEventosSistema.Instancia.Notificar("Bitácora", "EXPORTAR", $"Exportación de bitácora en formato {formato}.");
        }

        private static void GenerarPdfNativo(List<BitacoraRegistro> registros, string rutaArchivo, string alcance)
        {
            var lineas = new List<string>
            {
                "MINISTERIO DE SALUD - BITACORA DE AUDITORIA MULTI-TENANT",
                alcance,
                $"Fecha de emision: {DateTime.Now:dd/MM/yyyy HH:mm}",
                "--------------------------------------------------------------------------------"
            };

            foreach (var r in registros.Take(40))
            {
                string texto = $"{r.FechaHora:dd/MM HH:mm} | {r.NombreUsuario} | {r.Modulo}/{r.Accion} | {r.Descripcion}";
                if (texto.Length > 92) texto = texto[..92] + "...";
                lineas.Add(LimpiarTextoPdf(texto));
            }

            var contentStream = new StringBuilder();
            contentStream.AppendLine("BT /F1 11 Tf 40 750 Td 14 TL");
            for (int i = 0; i < lineas.Count; i++)
            {
                if (i == 1) contentStream.AppendLine("/F1 9 Tf");
                contentStream.AppendLine($"({lineas[i]}) Tj T*");
            }
            contentStream.AppendLine("ET");

            byte[] streamBytes = Encoding.ASCII.GetBytes(contentStream.ToString());
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.ASCII);

            var offsets = new List<long>();
            void WriteStr(string s) => bw.Write(Encoding.ASCII.GetBytes(s));

            WriteStr("%PDF-1.4\n");
            offsets.Add(ms.Position); WriteStr("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n");
            offsets.Add(ms.Position); WriteStr("2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n");
            offsets.Add(ms.Position); WriteStr("3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj\n");
            offsets.Add(ms.Position); WriteStr($"4 0 obj << /Length {streamBytes.Length} >> stream\n");
            bw.Write(streamBytes);
            WriteStr("\nendstream endobj\n");
            offsets.Add(ms.Position); WriteStr("5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Courier >> endobj\n");

            long xrefPos = ms.Position;
            WriteStr($"xref\n0 {offsets.Count + 1}\n0000000000 65535 f \n");
            foreach (var off in offsets) WriteStr($"{off:D10} 00000 n \n");
            WriteStr($"trailer << /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF");

            File.WriteAllBytes(rutaArchivo, ms.ToArray());
        }

        private static string LimpiarTextoPdf(string s)
        {
            return s.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                    .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                    .Replace("ñ", "n").Replace("Ñ", "N").Replace("(", "[").Replace(")", "]").Replace("\\", "/");
        }
    }
}