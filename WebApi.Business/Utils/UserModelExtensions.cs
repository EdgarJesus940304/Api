using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Business.Utils
{
    public static class UserModelExtensions
    {
        #region De negocio a data
        public static Data.usuarios ToUserData(this Models.UserModel model)
        {
            return model == null ? null : new Data.usuarios()
            {
                idusuario = model.Id,
                nombre = model.Name,
                fechacreacion = model.CreationDate,
                usuario = model.UserName,
                password = model.Password,
                estatus = model.Status,
            };
        }

        #endregion

        #region De data a negocio
        public static Models.UserModel ToUserBusiness(this Data.usuarios data)
        {
            return data == null ? null : new Models.UserModel()
            {
                Id = data.idusuario,
                UserName = data.usuario,
                Password = data.password,
                CreationDate = data.fechacreacion,
                Name = data.nombre,
                Perfil = data.idperfil,
                Status = data.estatus
            };
        }

        #endregion

    }
}
