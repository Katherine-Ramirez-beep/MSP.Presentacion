using Microsoft.Data.SqlClient;
using MSP.Entidades;

namespace MSP.Datos
{
    public class CatalogoRepositorio
    {
        public List<Provincia> ListarProvincias(bool soloActivas = false)
        {
            var lista = new List<Provincia>();
            const string sql = @"
                SELECT ProvinciaId, Codigo, Nombre, RegionSalud, Activo, FechaCreacion
                FROM Provincias
                WHERE (@SoloActivas = 0 OR Activo = 1)
                ORDER BY Nombre ASC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SoloActivas", soloActivas);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new Provincia
                {
                    ProvinciaId = r.GetInt32(0),
                    Codigo = r.GetString(1),
                    Nombre = r.GetString(2),
                    RegionSalud = r.GetString(3),
                    Activo = r.GetBoolean(4),
                    FechaCreacion = r.GetDateTime(5)
                });
            }
            return lista;
        }

        public void CambiarEstadoProvincia(int provinciaId, bool activo)
        {
            const string sql = "UPDATE Provincias SET Activo = @Activo WHERE ProvinciaId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Activo", activo);
            cmd.Parameters.AddWithValue("@Id", provinciaId);
            cmd.ExecuteNonQuery();
        }

        public List<TipoEstablecimiento> ListarTipos(bool soloActivos = false)
        {
            var lista = new List<TipoEstablecimiento>();
            const string sql = @"
                SELECT TipoEstablecimientoId, Codigo, Nombre, Descripcion, Activo, FechaCreacion
                FROM Tipos_Establecimientos
                WHERE (@SoloActivos = 0 OR Activo = 1)
                ORDER BY Nombre ASC;";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@SoloActivos", soloActivos);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new TipoEstablecimiento
                {
                    TipoEstablecimientoId = r.GetInt32(0),
                    Codigo = r.GetString(1),
                    Nombre = r.GetString(2),
                    Descripcion = r.IsDBNull(3) ? string.Empty : r.GetString(3),
                    Activo = r.GetBoolean(4),
                    FechaCreacion = r.GetDateTime(5)
                });
            }
            return lista;
        }

        public void InsertarTipo(TipoEstablecimiento t)
        {
            const string sql = @"
                INSERT INTO Tipos_Establecimientos (Codigo, Nombre, Descripcion, Activo)
                VALUES (@Codigo, @Nombre, @Descripcion, 1);";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Codigo", t.Codigo.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@Nombre", t.Nombre.Trim());
            cmd.Parameters.AddWithValue("@Descripcion", (object?)t.Descripcion ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstadoTipo(int tipoId, bool activo)
        {
            const string sql = "UPDATE Tipos_Establecimientos SET Activo = @Activo WHERE TipoEstablecimientoId = @Id;";
            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Activo", activo);
            cmd.Parameters.AddWithValue("@Id", tipoId);
            cmd.ExecuteNonQuery();
        }

        public void InsertarProvincia(Provincia p)
        {
            const string sql = @"
                INSERT INTO Provincias (Codigo, Nombre, RegionSalud, Activo)
                VALUES (@Codigo, @Nombre, @RegionSalud, 1);";

            using var conn = ConexionFactory.Instancia.CrearConexion();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Codigo", p.Codigo.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre.Trim());
            cmd.Parameters.AddWithValue("@RegionSalud", p.RegionSalud.Trim());
            cmd.ExecuteNonQuery();
        }
    }
}
