using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Business.Models
{
    public class PharmaceuticalFormModel
    {
        public PharmaceuticalFormModel()
        {
            Id = 0;
            Name = null;
            Habilitado = 1;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public int? Habilitado { get; set; }

    }
}
