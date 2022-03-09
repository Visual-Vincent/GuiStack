using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface ISQSRepository
    {
        Task<IEnumerable<SQSQueue>> GetQueuesAsync();
    }

    public class SQSRepository : ISQSRepository
    {
        private SQSAuthenticator authenticator = new SQSAuthenticator();

        public async Task<IEnumerable<SQSQueue>> GetQueuesAsync()
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.ListQueuesAsync(new ListQueuesRequest());

            response.ThrowIfUnsuccessful("SQS");

            return response.QueueUrls.Select(url => new SQSQueue(url));
        }
    }
}
