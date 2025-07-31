using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Business.Models;

namespace WebApi.Business.Utils
{
    public static class MedicationExtensions
    {
        #region De negocio a data
        public static Data.medicamentos ToMedicationData(this Models.MedicationModel model)
        {
            return model == null ? null : new Data.medicamentos()
            {
                idmedicamento = model.Id,
                nombre = model.Name,
                concentracion = model.Concentration,
                idformafarmaceutica = model?.PharmaceuticalForm?.Id,
                precio = model.Price,
                stock = model.Stock,
                presentacion = model.Presentation,
                bhabilitado = model.Enable,
            };
        }

        #endregion

        #region De data a negocio
        public static Models.MedicationModel ToMedicationBusiness(this Data.medicamentos data)
        {
            return data == null ? null : new Models.MedicationModel()
            {
                Id = data.idmedicamento,
                Name = data.nombre,
                Concentration = data.concentracion,
                Presentation = data.presentacion,
                Price = data.precio,
                Stock = data.stock,
                Enable = data.bhabilitado,
                PharmaceuticalForm = new PharmaceuticalFormModel()
                {
                    Id = data.formasfarmaceuticas?.idformafarmaceutica,
                    Name = data.formasfarmaceuticas?.nombre
                }
            };
        }
        public static Models.PharmaceuticalFormModel ToPharmaceuticalFormMBusiness(this Data.formasfarmaceuticas data)
        {
            return data == null ? null : new Models.PharmaceuticalFormModel()
            {
                Id = data.idformafarmaceutica,
                Name = data.nombre
            };
        }


        #endregion
    }
}
