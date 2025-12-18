using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Back_api.Models;
using Back_api.Services;
using BCrypt.Net;

namespace Back_api.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class AuthController : ControllerBase
    {
        private readonly MySqlService _mySqlService;

        public AuthController(MySqlService mySqlService)
        {
            _mySqlService = mySqlService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using var conn = _mySqlService.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("usuario_login", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("p_email", request.Email);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            string senhaBanco = reader.GetString("senha");

            bool senhaValida = BCrypt.Net.BCrypt.Verify(
                request.Senha,
                senhaBanco
            );

            if (!senhaValida)
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            return Ok(new
            {
                id = reader.GetInt32("id"),
                nome = reader.GetString("nome"),
                email = reader.GetString("email")
            });
        }


        [HttpPost("criar_usuario")]
        public async Task<IActionResult> PostUser([FromBody] UserPostRequest request)
        {
            using var conn = _mySqlService.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("criar_usuario", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            cmd.Parameters.AddWithValue("p_nome", request.Nome);
            cmd.Parameters.AddWithValue("p_email", request.Email);
            cmd.Parameters.AddWithValue("p_senha_hash", senhaHash);

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            return Ok(new { message = "Criado com sucesso" });
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            using var conn = _mySqlService.CreateConnection();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("listar_usuarios", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using var reader = await cmd.ExecuteReaderAsync();

            var usuarios = new List<object>();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new
                {
                    id = reader.GetInt32("id"),
                    nome = reader.GetString("nome"),
                    email = reader.GetString("email"),
                    ativo = reader.GetString("ativo"),
                    dtcreate = reader.GetDateTime("dtcreate")
                });
            }

            return Ok(usuarios);
        }

    }
}
