using Dapper;
using Manejo_Presupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Manejo_Presupuesto.Servicios
{
    public interface IRepositoriosTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositoriosTiposCuentas : IRepositoriosTiposCuentas
    {
        private readonly string connectionSting;
        public RepositoriosTiposCuentas(IConfiguration configuration)
        {
            connectionSting = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionSting);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", 
                                                            new {usuarioId = tipoCuenta.UsuarioId,
                                                            nombre = tipoCuenta.Nombre},
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre,
                                       int usuarioId)
        {
            using var connection = new SqlConnection(connectionSting);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                            FROM TiposCuentas
                                                                            WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                                                                            new { nombre, usuarioId });

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionSting);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                          FROM TiposCuentas
                                                          WHERE UsuarioId = @UsuarioId
                                                          ORDER BY Orden ", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionSting);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id", tipoCuenta);
            
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionSting);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                            FROM TiposCuentas
                                                                            WHERE Id = @Id AND UsuarioId = @UsuarioId",  new {id, usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionSting);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden where Id = @Id;";
            using var connection = new SqlConnection(connectionSting);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
