using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Business.Handlers
{
    public class BaseHandler : IDisposable
    {
        private bool disposedValue;

        #region Propiedades de conexión
        protected Data.Cepdi_PruebaEntities Db { get; set; }

        #endregion

        public BaseHandler()
        {
            Db = new Data.Cepdi_PruebaEntities();
         
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Db?.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
