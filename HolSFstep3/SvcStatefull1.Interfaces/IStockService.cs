using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvcStatefull1.Interfaces
{
    public interface IStockService : IService
    {
        Task<int> GetStockForProduct(string productId);

        Task<bool> UpdateStockForProduct(string productId, int qty);
    }
}
