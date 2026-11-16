using System.Data;
using Microsoft.Data.SqlClient;
using MSP.Entidades;

namespace MSP.Datos
{
    public class UsuarioRepositorio
    {
        public bool ExisteAlgunUsuario()
        {
            const string sql = "SELECT COUNT(1) FROM Usuarios;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
        {
            const string sql = "SELECT * FROM vw_UsuariosDetalle WHERE NombreUsuario = @NombreUsuario;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario.Trim());

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Usuario
            {
                UsuarioId = Convert.ToInt32(r["UsuarioId"]),
                ProvinciaId = r["ProvinciaId"] == DBNull.Value ? null : Convert.ToInt32(r["ProvinciaId"]),
                NombreCompleto = r["NombreCompleto"].ToString()!,
                NombreUsuario = r["NombreUsuario"].ToString()!,
                PasswordHash = (byte[])r["PasswordHash"],
                PasswordSalt = (byte[])r["PasswordSalt"],
                Rol = r["Rol"].ToString()!,
                IntentosFallidos = Convert.ToInt32(r["IntentosFallidos"]),
                Bloqueado = Convert.ToBoolean(r["Bloqueado"]),
                Activo = Convert.ToBoolean(r["Activo"]) && Convert.ToBoolean(r["ProvinciaActiva"]),
                UltimoAcceso = r["UltimoAcceso"] == DBNull.Value ? null : Convert.ToDateTime(r["UltimoAcceso"]),
                FechaCreacion = Convert.ToDateTime(r["FechaCreacion"]),
                NombreProvincia = r["NombreProvincia"].ToString()!
            };
        }

        public List<Usuario> Listar(int? provinciaIdFiltro)
        {
            var lista = new List<Usuario>();
            const string sql = @"
                SELECT UsuarioId, ProvinciaId, NombreCompleto, NombreUsuario, Rol, 
                       IntentosFallidos, Bloqueado, Activo, UltimoAcceso, NombreProvincia
                FROM vw_UsuariosDetalle
                WHERE (@ProvinciaId IS NULL OR ProvinciaId = @ProvinciaId)
                ORDER BY UsuarioId ASC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)provinciaIdFiltro ?? DBNull.Value);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new Usuario
                {
                    UsuarioId = Convert.ToInt32(r["UsuarioId"]),
                    ProvinciaId = r["ProvinciaId"] == DBNull.Value ? null : Convert.ToInt32(r["ProvinciaId"]),
                    NombreCompleto = r["NombreCompleto"].ToString()!,
                    NombreUsuario = r["NombreUsuario"].ToString()!,
                    Rol = r["Rol"].ToString()!,
                    IntentosFallidos = Convert.ToInt32(r["IntentosFallidos"]),
                    Bloqueado = Convert.ToBoolean(r["Bloqueado"]),
                    Activo = Convert.ToBoolean(r["Activo"]),
                    UltimoAcceso = r["UltimoAcceso"] == DBNull.Value ? null : Convert.ToDateTime(r["UltimoAcceso"]),
                    NombreProvincia = r["NombreProvincia"].ToString()!
                });
            }
            return lista;
        }

        public void Insertar(Usuario u)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("sp_Usuarios_Insertar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ProvinciaId", (object?)u.ProvinciaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NombreCompleto", u.NombreCompleto);
            cmd.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
            cmd.Parameters.AddWithValue("@PasswordHash", u.PasswordHash);
            cmd.Parameters.AddWithValue("@PasswordSalt", u.PasswordSalt);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.ExecuteNonQuery();
        }

        public void RegistrarIntentoFallido(int usuarioId, int nuevosIntentos, bool bloquear)
        {
            const string sql = "UPDATE Usuarios SET IntentosFallidos = @Intentos, Bloqueado = @Bloqueado WHERE UsuarioId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Intentos", nuevosIntentos);
            cmd.Parameters.AddWithValue("@Bloqueado", bloquear);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            cmd.ExecuteNonQuery();
        }

        public void RegistrarLoginExitoso(int usuarioId)
        {
            const string sql = "UPDATE Usuarios SET IntentosFallidos = 0, UltimoAcceso = SYSDATETIME() WHERE UsuarioId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstadoBloqueoOActivo(int usuarioId, bool activo, bool desbloquear)
        {
            const string sql = @"
                UPDATE Usuarios 
                SET Activo = @Activo, 
                    Bloqueado = CASE WHEN @Desbloquear = 1 THEN 0 ELSE Bloqueado END,
                    IntentosFallidos = CASE WHEN @Desbloquear = 1 THEN 0 ELSE IntentosFallidos END
                WHERE UsuarioId = @Id;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Activo", activo);
            cmd.Parameters.AddWithValue("@Desbloquear", desbloquear);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarPerfil(int id, string nombre, string rol, int? provinciaId)
        {
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand("UPDATE Usuarios SET NombreCompleto=@Nombre, Rol=@Rol, ProvinciaId=@Provincia WHERE UsuarioId=@Id", conn);
            cmd.Parameters.AddWithValue("@Nombre", nombre.Trim());
            cmd.Parameters.AddWithValue("@Rol", rol);
            cmd.Parameters.AddWithValue("@Provincia", (object?)provinciaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarPassword(int usuarioId, byte[] hash, byte[] salt)
        {
            const string sql = "UPDATE Usuarios SET PasswordHash = @Hash, PasswordSalt = @Salt WHERE UsuarioId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Hash", hash);
            cmd.Parameters.AddWithValue("@Salt", salt);
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            cmd.ExecuteNonQuery();
        }
    }
}
