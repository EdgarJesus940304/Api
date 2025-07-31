using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebApi.Business.Models;
using WebApi.Business.Utils;
using WebApi.Data;

namespace WebApi.Business.Handlers
{
    public class UserHandler : BaseHandler
    {
        public MessageResponse ListUsers(FilterDataTableModel model)
        {
            try
            {
                var whereClause = BuildWhereClause(model.SearchBy);
                var sqlQuery = Db.usuarios;
                var result = sqlQuery
                               .AsExpandable()
                               .Where(whereClause)
                               .OrderBy(model.SortBy, model.SortDir, GetColumMapper())
                               .Skip(model.Skip)
                               .Take(model.Take > 0 ? model.Take : sqlQuery.Count())
                               .AsEnumerable();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Data = new
                    {
                        recordsTotal = sqlQuery.Count(),
                        recordsFiltered = sqlQuery.AsExpandable().Where(whereClause).Count(),
                        data = result.Select(us => us.ToUserBusiness()).ToList()
                    }
                };
            }
            catch (Exception ex)
            {

                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible consultar los usuarios {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse GetUserById(int id)
        {
            try
            {
                var data = Db.usuarios.FirstOrDefault(f => f.idusuario == id);
                UserModel user = data.ToUserBusiness();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible obtener el usuario especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }

        }
        public MessageResponse CreateUser(UserModel user)
        {
            try
            {
                Db.usuarios.Add(user.ToUserData());
                Db.SaveChanges();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Message = "Usuario registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible registrar el usuario capturado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse ModifyUser(int id, UserModel user)
        {
            try
            {
                var entry = Db.usuarios.Find(id);

                if (entry == null)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Error,
                        Message = $"No fue posible obtener el usuario a editar"
                    };
                }

                Db.Entry(entry).CurrentValues.SetValues(user.ToUserData());
                Db.Entry(entry).Property(e => e.fechacreacion).IsModified = false;
                Db.SaveChanges();

                return new MessageResponse()
                {
                    Message = "Usuario actualizado exitosamente",
                    ResponseType = ResponseType.OK
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible actualizar el usuario especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse RemoveUser(int userId)
        {
            try
            {
                var entry = Db.usuarios.Find(userId);

                if (entry == null)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Error,
                        Message = $"No fue posible obtener el usuario a eliminar"
                    };
                }

                Db.usuarios.Remove(entry);
                Db.SaveChanges();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Message = "Usuario ekiminado exitosamente",
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible eliminar el usuario especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }
        public MessageResponse Login(UserModel userModel)
        {
            try
            {
                var data = Db.usuarios.FirstOrDefault(f => f.usuario == userModel.UserName);

                if (data is null)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Warning,
                        Message = "El usuario es incorrecto o no se encuentra registrado"
                    };
                }

                if (data?.password != userModel.Password)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Warning,
                        Message = "La contraseña es incorrecta"
                    };
                }

                UserModel user = data.ToUserBusiness();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"Ocurrio un error al intentar inciar de sesion {ex.Message} {ex?.InnerException?.Message}"
                };
            }

        }
        private Expression<Func<Data.usuarios, bool>> BuildWhereClause(string searchValue)
        {
            var predicate = PredicateBuilder.New<Data.usuarios>(true);
            if (string.IsNullOrWhiteSpace(searchValue) == false)
            {
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());
                predicate = predicate.Or(s => searchTerms.Any(srch => s.nombre.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.usuario.ToLower().Contains(srch)));
            }
            return predicate;
        }

        private Dictionary<string, string> GetColumMapper()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", "idusuario" },
                { "Name", "nombre" },
                { "UserName", "usuario" },
                { "Password", "password" },
                { "CreationDate", "fechacreacion" },
                { "StatusName", "estatus" },
            };
        }
    }
}
