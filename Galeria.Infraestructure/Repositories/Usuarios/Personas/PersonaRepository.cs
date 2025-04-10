﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using Galeria.Domain.Common.ViewModels.Personas;
using Galeria.Domain.Entities.Usuarios.Personas;
using Galeria.Infraestructure.Interfaces.Usuarios.Personas;
using Galeria.Infraestructure.Repositories.Generic;
using Galeria.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Galeria.Infraestructure.Repositories.Usuarios.Personas
{
    public class PersonaRepository : BaseRepository<Persona>, IPersonaRepository
    {
        private readonly ApplicationDbContext _context;
        public PersonaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> ActualizarPerfilUsuario(string idApplicationUser, ActualizarPerfilVM datos)
        {
            try
            {
                string sqlArtista = @"
                    UPDATE Tbl_Personas 
                    SET Nombres = @nombres, 
                        Apellidos = @apellidos, 
                        Edad = @edad
                    WHERE IdApplicationUser = @idApplicationUser AND IsDeleted = 0";

                var parametros = new
                {
                    idApplicationUser,
                    nombres = datos.Nombres,
                    apellidos = datos.Apellidos,
                    edad = datos.Edad
                };

                await _context.Database.GetDbConnection().ExecuteAsync(sqlArtista, parametros);

                if (!string.IsNullOrEmpty(datos.Contraseña))
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    string passwordHash = passwordHasher.HashPassword(null, datos.Contraseña);

                    string sqlUsuario = @"
                        UPDATE AspNetUsers 
                        SET PasswordHash = @passwordHash
                        WHERE Id = @idApplicationUser";

                    await _context.Database.GetDbConnection().ExecuteAsync(sqlUsuario, new { idApplicationUser, passwordHash });
                }

                return 1;
            }
            catch
            {
                throw;
            }
        }


        public async Task<int> SubirFotoPerfil(string idApplicationUser, string url)
        {
            string sql = @"UPDATE AspNetUsers SET AvatarURL = @url WHERE IsDeleted = 0 AND Id = @idApplicationUser";
            var result = await _context.Database.GetDbConnection().ExecuteAsync(sql, new { idApplicationUser, url });
            return result;
        }

        public async Task<int> ActualizarFotoPerfil(string idApplicationUser, string url)
        {
            string sql = @"UPDATE AspNetUsers SET AvatarURL = @url WHERE IsDeleted = 0 AND Id = @idApplicationUser";
            var result = await _context.Database.GetDbConnection().ExecuteAsync(sql, new { idApplicationUser, url });
            return result;
        }

        public async Task<int> EliminarFotoPerfil(string idApplicationUser)
        {
            string sql = @"UPDATE AspNetUsers SET AvatarURL = NULL WHERE IsDeleted = 0 AND Id = @idApplicationUser";
            var result = await _context.Database.GetDbConnection().ExecuteAsync(sql, new { idApplicationUser });
            return result;
        }

        public async Task<DatosPerfilVM> GetDatosPerfil(string idApplicationUser)
        {
            string sql = @"SELECT per.Nombres, per.Apellidos, per.Edad, usr.AvatarURL FROM Tbl_Personas AS per
                    INNER JOIN AspNetUsers AS usr ON usr.Id = per.IdApplicationUser AND usr.IsDeleted = 0
                    WHERE per.IdApplicationUser = @idApplicationuser AND per.IsDeleted = 0 ";

            var result = await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<DatosPerfilVM>(sql, new { idApplicationUser });
            return result;
        }

        public async Task<List<DatosPerfilVM>> GetTodosLosUsuarios()
        {
            string sql = @"SELECT per.Id, per.Nombres, per.Apellidos, per.Edad, per.idApplicationUser, usr.AvatarURL FROM Tbl_Personas AS per
                    INNER JOIN AspNetUsers AS usr ON usr.Id = per.IdApplicationUser AND usr.IsDeleted = 0
                    WHERE per.IsDeleted = 0 ";

            var result = await _context.Database.GetDbConnection().QueryAsync<DatosPerfilVM>(sql);
            return result.ToList();
        }
    }
}
