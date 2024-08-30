/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface IDynamoDBRepository
    {
        Task CreateTableAsync(DynamoDBCreateTableModel model);
        Task DeleteTableAsync(string tableName);
        Task<string[]> GetTablesAsync();
        Task<DynamoDBTableModel> GetTableInfoAsync(string tableName);
        Task DeleteItemAsync(string tableName, DynamoDBAttributeValue partitionKey, DynamoDBAttributeValue sortKey);
        Task<DynamoDBItemModel> GetItemAsync(string tableName, DynamoDBAttributeValue partitionKey, DynamoDBAttributeValue sortKey);
        Task PutItemAsync(string tableName, IDictionary<string, DynamoDBFieldModel> itemData);
        Task<DynamoDBTableScanResults> ScanAsync(string tableName, int limit, DynamoDBItemModel lastEvaluatedKey = null);
    }

    public class DynamoDBRepository : IDynamoDBRepository
    {
        private DynamoDBAuthenticator authenticator = new DynamoDBAuthenticator();

        public async Task CreateTableAsync(DynamoDBCreateTableModel model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            using var dynamodb = authenticator.Authenticate();

            var keySchema = new List<KeySchemaElement>() {
                new KeySchemaElement(model.PartitionKey.Name, KeyType.HASH)
            };

            var attributeDefinitions = new List<AttributeDefinition>() {
                new AttributeDefinition(model.PartitionKey.Name, model.PartitionKey.Type.ToScalarAttributeType())
            };

            if(model.SortKey != null && !string.IsNullOrEmpty(model.SortKey.Name))
            {
                keySchema.Add(new KeySchemaElement(model.SortKey.Name, KeyType.RANGE));
                attributeDefinitions.Add(new AttributeDefinition(model.SortKey.Name, model.SortKey.Type.ToScalarAttributeType()));
            }

            var response = await dynamodb.CreateTableAsync(new CreateTableRequest() {
                TableName = model.TableName,
                KeySchema = keySchema,
                AttributeDefinitions = attributeDefinitions,
                TableClass = TableClass.STANDARD, // AWS defaults
                BillingMode = BillingMode.PROVISIONED, // AWS defaults
                ProvisionedThroughput = new ProvisionedThroughput(5, 5), // AWS defaults
                StreamSpecification = new StreamSpecification() { StreamEnabled = false },
                DeletionProtectionEnabled = false // AWS defaults
            });

            response.ThrowIfUnsuccessful("DynamoDB");
        }

        public async Task DeleteTableAsync(string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            using var dynamodb = authenticator.Authenticate();
            var response = await dynamodb.DeleteTableAsync(tableName);

            response.ThrowIfUnsuccessful("DynamoDB");
        }

        public async Task<string[]> GetTablesAsync()
        {
            using var dynamodb = authenticator.Authenticate();
            var response = await dynamodb.ListTablesAsync();

            response.ThrowIfUnsuccessful("DynamoDB");

            return response.TableNames.ToArray();
        }

        public async Task<DynamoDBTableModel> GetTableInfoAsync(string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            using var dynamodb = authenticator.Authenticate();
            var response = await dynamodb.DescribeTableAsync(tableName);

            response.ThrowIfUnsuccessful("DynamoDB");

            var table = response.Table;
            var partitionKey = table.KeySchema.Find(k => k.KeyType == KeyType.HASH);
            var sortKey      = table.KeySchema.Find(k => k.KeyType == KeyType.RANGE);

            var partitionKeyAttribute = table.AttributeDefinitions.Find(k => k.AttributeName == partitionKey.AttributeName);
            var sortKeyAttribute      = sortKey != null ? table.AttributeDefinitions.Find(k => k.AttributeName == sortKey.AttributeName) : null;

            var status = table.TableStatus?.Value;

            status = !string.IsNullOrEmpty(status)
                ? char.ToUpper(status[0]) + status?.ToLower()[1..]
                : "(Unknown)";

            return new DynamoDBTableModel() {
                Name = table.TableName,
                Arn = Arn.Parse(table.TableArn),
                ItemCount = table.ItemCount,
                TableSizeBytes = table.TableSizeBytes,
                TableClass = (table.TableClassSummary?.TableClass ?? null).ToHumanReadableString(),
                BillingMode = (table.BillingModeSummary?.BillingMode ?? null).ToHumanReadableString(),
                ReadCapacityUnits = table.ProvisionedThroughput?.ReadCapacityUnits ?? 0,
                WriteCapacityUnits = table.ProvisionedThroughput?.WriteCapacityUnits ?? 0,
                DeletionProtectionEnabled = table.DeletionProtectionEnabled,
                Status = status,

                PartitionKey = new DynamoDBKeyAttribute(partitionKeyAttribute.AttributeName, partitionKeyAttribute.AttributeType.ToDynamoDBAttributeType()),
                SortKey = sortKeyAttribute != null ? new DynamoDBKeyAttribute(sortKeyAttribute.AttributeName, sortKeyAttribute.AttributeType.ToDynamoDBAttributeType()) : null
            };
        }

        public async Task DeleteItemAsync(string tableName, DynamoDBAttributeValue partitionKey, DynamoDBAttributeValue sortKey)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            if(partitionKey == null)
                throw new ArgumentNullException(nameof(partitionKey));

            if(partitionKey.Value == null)
                throw new ArgumentException("partitionKey.Value cannot be null", nameof(partitionKey));

            if(sortKey != null && sortKey.Value == null)
                throw new ArgumentException("sortKey.Value cannot be null", nameof(sortKey));

            if(partitionKey.Value.Type != DynamoDBFieldType.String 
                && partitionKey.Value.Type != DynamoDBFieldType.Number 
                && partitionKey.Value.Type != DynamoDBFieldType.Binary)
                throw new ArgumentException($"'{partitionKey.Value.Type}' is not a valid data type for the partition key", nameof(partitionKey));

            if(sortKey != null
                && sortKey.Value.Type != DynamoDBFieldType.String
                && sortKey.Value.Type != DynamoDBFieldType.Number
                && sortKey.Value.Type != DynamoDBFieldType.Binary)
                throw new ArgumentException($"'{sortKey.Value.Type}' is not a valid data type for the sort key", nameof(sortKey));

            using var dynamodb = authenticator.Authenticate();

            var key = new Dictionary<string, AttributeValue>() {
                { partitionKey.Name, partitionKey.ToDynamoDBAttributeValue() }
            };

            if (sortKey != null)
                key.Add(sortKey.Name, sortKey.ToDynamoDBAttributeValue());

            var response = await dynamodb.DeleteItemAsync(new DeleteItemRequest() {
                TableName = tableName,
                Key = key
            });
            
            response.ThrowIfUnsuccessful("DynamoDB");
        }

        public async Task<DynamoDBItemModel> GetItemAsync(string tableName, DynamoDBAttributeValue partitionKey, DynamoDBAttributeValue sortKey)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            if(partitionKey == null)
                throw new ArgumentNullException(nameof(partitionKey));

            if(partitionKey.Value == null)
                throw new ArgumentException("partitionKey.Value cannot be null", nameof(partitionKey));

            if(sortKey != null && sortKey.Value == null)
                throw new ArgumentException("sortKey.Value cannot be null", nameof(sortKey));

            if(partitionKey.Value.Type != DynamoDBFieldType.String 
                && partitionKey.Value.Type != DynamoDBFieldType.Number 
                && partitionKey.Value.Type != DynamoDBFieldType.Binary)
                throw new ArgumentException($"'{partitionKey.Value.Type}' is not a valid data type for the partition key", nameof(partitionKey));

            if(sortKey != null
                && sortKey.Value.Type != DynamoDBFieldType.String
                && sortKey.Value.Type != DynamoDBFieldType.Number
                && sortKey.Value.Type != DynamoDBFieldType.Binary)
                throw new ArgumentException($"'{sortKey.Value.Type}' is not a valid data type for the sort key", nameof(sortKey));

            using var dynamodb = authenticator.Authenticate();

            var key = new Dictionary<string, AttributeValue>() {
                { partitionKey.Name, partitionKey.ToDynamoDBAttributeValue() }
            };

            if (sortKey != null)
                key.Add(sortKey.Name, sortKey.ToDynamoDBAttributeValue());
            
            var response = await dynamodb.GetItemAsync(new GetItemRequest() {
                TableName = tableName,
                Key = key,
                ConsistentRead = true
            });

            response.ThrowIfUnsuccessful("DynamoDB");

            var item = response.Item?.ToDynamoDBItemModel();

            if(response.Item != null)
                DynamoDBItem.FromAttributes(response.Item).Dispose();

            return item;
        }

        public async Task PutItemAsync(string tableName, IDictionary<string, DynamoDBFieldModel> itemData)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            if(itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            using var dynamodb = authenticator.Authenticate();
            using var item = itemData.ToDynamoDBItem();

            var response = await dynamodb.PutItemAsync(new PutItemRequest() {
                TableName = tableName,
                Item = item.Attributes
            });

            response.ThrowIfUnsuccessful("DynamoDB");
        }

        public async Task<DynamoDBTableScanResults> ScanAsync(string tableName, int limit, DynamoDBItemModel lastEvaluatedKey = null)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name cannot be empty", nameof(tableName));

            if(limit < 1 || limit > 500)
                throw new ArgumentOutOfRangeException(nameof(limit), limit, "Item limit must be between 1 and 500");

            using var dynamodb = authenticator.Authenticate();

            List<Dictionary<string, AttributeValue>> items = new();
            DynamoDBItem lastEvalKey = lastEvaluatedKey?.ToDynamoDBItem();

            try
            {
                while(items.Count < limit)
                {
                    var result = await dynamodb.ScanAsync(new ScanRequest() {
                        TableName = tableName,
                        ConsistentRead = true,
                        Select = Select.ALL_ATTRIBUTES,
                        ReturnConsumedCapacity = ReturnConsumedCapacity.NONE,
                        ExclusiveStartKey = lastEvalKey?.Attributes,
                        Limit = limit
                    });

                    result.ThrowIfUnsuccessful("DynamoDB");
                    result.Items.ForEach(item => items.Add(item));

                    lastEvalKey?.Dispose();
                    lastEvalKey = result.LastEvaluatedKey != null
                        ? DynamoDBItem.FromAttributes(result.LastEvaluatedKey)
                        : null;

                    if(lastEvalKey == null || lastEvalKey.Attributes.Count <= 0)
                        break; // No more items in result
                }

                return new DynamoDBTableScanResults() {
                    Items = items.Select(item => item.ToDynamoDBItemModel()).ToArray(),
                    LastEvaluatedKey = lastEvalKey != null && lastEvalKey.Attributes.Count > 0
                        ? lastEvalKey.Attributes.ToDynamoDBItemModel()
                        : null
                };
            }
            finally
            {
                lastEvalKey?.Dispose();

                foreach (var item in items)
                {
                    DynamoDBItem.FromAttributes(item).Dispose();
                }
            }
        }
    }
}
