using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DTOs.DataSchema;

namespace BPMS_BL.Helpers
{
    public static class ServiceTreeHelper
    {
        public static IEnumerable<T> CreateTree<T>(IEnumerable<T> allNodes, Guid? parentId = null) where T : DataSchema
        {
            IEnumerable<T> nodes = allNodes.Where(x => x.ParentId == parentId);
            foreach (T node in nodes)
            {
                if (node.Type >= DataTypeEnum.Object)
                {
                    node.Children = CreateTree(allNodes, node.Id);
                }
            }

            return nodes;
        }
    }
}
