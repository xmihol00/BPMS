
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Filter;

namespace BPMS_BL.Helpers
{
    public static class FilterHelper
    {
        public static async Task ChnageFilterState(FilterRepository filterRepository, FilterDTO dto, Guid userId)
        {
            try
            {
                FilterEntity entity = new FilterEntity
                {
                    Filter = dto.Filter,
                    UserId = userId
                };

                if (dto.Removed)
                {
                    filterRepository.Remove(entity);
                }
                else
                {
                    await filterRepository.Create(entity);
                }
                await filterRepository.Save();
            }
            catch 
            {

            }
        }
    }
}
