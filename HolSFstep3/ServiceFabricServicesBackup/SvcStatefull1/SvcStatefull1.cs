using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SvcStatefull1.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace SvcStatefull1
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SvcStatefull1 : StatefulService, IStockService
    {
        public SvcStatefull1(StatefulServiceContext context)
            : base(context)
        {
            //leStock.Add("SurfaceBook", 5);
        }

        //Dictionary<string,int> leStock = new Dictionary<string,int>();

        public async Task<int> GetStockForProduct(string productId)
        {
            //if (leStock.ContainsKey(productId))
            //    return leStock[productId];
            //return 0;

            var leStock = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("leStock");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var getStock = await leStock.TryGetValueAsync(tx,productId);
                return getStock.HasValue ? getStock.Value : 0;
            }

        }

        public async Task<bool> UpdateStockForProduct(string productId, int qty)
        {
            //if (leStock.ContainsKey(productId))
            //{
            //    leStock[productId]= leStock[productId]+qty;
            //}
            //else
            //{
            //    leStock.Add(productId, qty);
            //}
            //return true;

            var leStock = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("leStock");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var getStock = await leStock.TryGetValueAsync(tx, productId);

                await leStock.AddOrUpdateAsync(tx, productId,qty, (key,value)=> value+qty );

                await tx.CommitAsync();
            }
            return true;

        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //we return a service remoting listener 
            return new List<ServiceReplicaListener>()
            {
                new ServiceReplicaListener( (context) => this.CreateServiceRemotingListener(context) )
            };

            //return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
