using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace FunctionApp.Infrastructure
{
    internal static class TopologyCreator
    {
        public static async Task Create(IConfiguration configuration, string topicName = "bundle-1", string auditQueue = "audit", string errorQueue = "error")
        {
            var connectionString = configuration.GetValue<string>("AzureWebJobsServiceBus");
            var managementClient = new ManagementClient(connectionString);

            var attribute = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttribute<FunctionNameAttribute>(false) != null)
                .SelectMany(m => m.GetParameters())
                .SelectMany(p => p.GetCustomAttributes<ServiceBusTriggerAttribute>(false))
                .FirstOrDefault();

            if (attribute == null)
            {
                throw new Exception("No endpoint was found");
            }

            // there are endpoints, create a topic
            if (!await managementClient.TopicExistsAsync(topicName))
            {
                await managementClient.CreateTopicAsync(topicName);
            }

            var endpointQueueName = attribute.QueueName;

            if (!await managementClient.QueueExistsAsync(endpointQueueName))
            {
                await managementClient.CreateQueueAsync(new QueueDescription(endpointQueueName)
                {
                    LockDuration = TimeSpan.FromMinutes(5),
                    MaxDeliveryCount = int.MaxValue
                });
            }

            if (!await managementClient.SubscriptionExistsAsync(topicName, endpointQueueName))
            {
                var subscriptionDescription = new SubscriptionDescription(topicName, endpointQueueName)
                {
                    ForwardTo = endpointQueueName,
                    UserMetadata = $"Events {endpointQueueName} subscribed to"
                };
                var ruleDescription = new RuleDescription
                {
                    Filter = new FalseFilter()
                };
                await managementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
            }

            if (!await managementClient.QueueExistsAsync(auditQueue))
            {
                await managementClient.CreateQueueAsync(auditQueue);
            }

            if (!await managementClient.QueueExistsAsync(errorQueue))
            {
                await managementClient.CreateQueueAsync(errorQueue);
            }
        }
    }
}