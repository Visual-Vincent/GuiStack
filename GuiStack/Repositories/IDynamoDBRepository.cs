/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<string[]> GetTablesAsync()
        {
            using var dynamodb = authenticator.Authenticate();
            var response = await dynamodb.ListTablesAsync();

            response.ThrowIfUnsuccessful("DynamoDB");

            return response.TableNames.ToArray();
        }

        public async Task DeleteTableAsync(string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            using var dynamodb = authenticator.Authenticate();
            var response = await dynamodb.DeleteTableAsync(tableName);

            response.ThrowIfUnsuccessful("DynamoDB");
        }
    }
}
