using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BaseFacade
    {
        protected Guid _userId { get; set; }
        protected readonly FilterRepository _filterRepository;

        public BaseFacade(FilterRepository filterRepository)
        {
            _filterRepository = filterRepository;
        }
    }
}
