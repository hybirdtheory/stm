using Stm.Core.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Mvcdemo
{
    public class OrderInfo
    {
        [Key]
        public long OrderId { get; set; }

        public string OrderContent { get; set; }

    }
}
