using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebApi.Business.Models;
using WebApi.Business.Utils;
using WebApi.Data;
using System.Data.Entity;

namespace WebApi.Business.Handlers
{
    public class MedicationHandler : BaseHandler
    {
        public MessageResponse ListMedications(FilterDataTableModel model)
        {
            try
            {
                var whereClause = BuildWhereClause(model.SearchBy);
                var sqlQuery = Db.medicamentos.Include(s => s.formasfarmaceuticas);
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
                        data = result.Select(us => us.ToMedicationBusiness()).ToList()
                    }
                };
            }
            catch (Exception ex)
            {

                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible consultar los medicamentos {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse GetMedicationById(int id)
        {
            try
            {
                var data = Db.medicamentos.FirstOrDefault(f => f.idmedicamento == id);
                MedicationModel medication = data.ToMedicationBusiness();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Data = medication
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible obtener el medicamento especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse CreateMedication(MedicationModel medication)
        {
            try
            {
                Db.medicamentos.Add(medication.ToMedicationData());
                Db.SaveChanges();

                return new MessageResponse()
                {
                    Message = "Medicamento registrado exitosamente",
                    ResponseType = ResponseType.OK
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible guardar el medicamento capturado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse ModifyMedication(MedicationModel medication)
        {
            try
            {
                var entry = Db.medicamentos.Find(medication.Id);

                if (entry == null)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Error,
                        Message = $"No fue posible obtener el medicamento a editar"
                    };
                }


                Db.Entry(entry).CurrentValues.SetValues(medication.ToMedicationData());
                Db.SaveChanges();

                return new MessageResponse()
                {
                    Message = "Medicamento actualizado exitosamente",
                    ResponseType = ResponseType.OK
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible actualizar el medicamento especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        public MessageResponse RemoveMedication(int medicationId)
        {
            try
            {
                var entry = Db.medicamentos.Find(medicationId);

                if (entry == null)
                {
                    return new MessageResponse()
                    {
                        ResponseType = ResponseType.Error,
                        Message = $"No fue posible obtener el medicamento a eliminar"
                    };
                }


                Db.medicamentos.Remove(entry);
                Db.SaveChanges();

                return new MessageResponse()
                {
                    Message = "Medicamento eliminado exitosamente",
                    ResponseType = ResponseType.OK
                };
            }
            catch (Exception ex)
            {
                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible eliminar el medicamento especificado {ex.Message} {ex?.InnerException?.Message}"
                };
            }

        }

        public MessageResponse GetPharmaceuticalForms()
        {
            try
            {
                var data = Db.formasfarmaceuticas
                    .Where(s => (s.habilitado.HasValue ? s.habilitado > 0 : false))
                    .AsEnumerable()
                    .Select(s => s.ToPharmaceuticalFormMBusiness()).ToList();

                return new MessageResponse()
                {
                    ResponseType = ResponseType.OK,
                    Data = new
                    {
                        data = data
                    }
                };
            }
            catch (Exception ex)
            {

                return new MessageResponse()
                {
                    ResponseType = ResponseType.Error,
                    Message = $"No fue posible consultar los formas farmaceuticas {ex.Message} {ex?.InnerException?.Message}"
                };
            }
        }

        private Expression<Func<Data.medicamentos, bool>> BuildWhereClause(string searchValue)
        {
            var predicate = PredicateBuilder.New<Data.medicamentos>(true);
            if (string.IsNullOrWhiteSpace(searchValue) == false)
            {
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());
                predicate = predicate.Or(s => searchTerms.Any(srch => s.nombre.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.concentracion.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.presentacion.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.formasfarmaceuticas.nombre.ToLower().Contains(srch)));
            }
            return predicate;
        }

        private Dictionary<string, string> GetColumMapper()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Id", "idmedicamento" },
                { "Name", "nombre" },
                { "Concentration", "concentracion" },
                { "Price", "precio" },
                { "Stock", "stock" },
                { "Presentation", "presentacion" },
                { "PharmaceuticalForm", "idformafarmaceutica"},
                { "StatusName", "bhabilitado" },
            };
        }
    }
}
