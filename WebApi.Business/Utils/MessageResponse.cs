﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Business.Utils
{
    public enum ResponseType { OK, Warning, Error }

    public class MessageResponse
    {
        public MessageResponse()
        {
            Message = string.Empty;
        }

        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
