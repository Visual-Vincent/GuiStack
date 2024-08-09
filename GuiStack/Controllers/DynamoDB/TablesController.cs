/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace GuiStack.Controllers.DynamoDB
{
    [ApiController]
    [Route("api/" + nameof(DynamoDB))]
    public class TablesController : Controller
    {
        private IDynamoDBRepository dynamodbRepository;

        public TablesController(IDynamoDBRepository dynamodbRepository)
        {
            this.dynamodbRepository = dynamodbRepository;
        }

        private static DynamoDBItemModel DeserializeItem(string data)
        {
            using var resultStream = new MemoryStream();
            using var reader = new StreamReader(resultStream);

            using (var compressedStream = new MemoryStream(WebEncoders.Base64UrlDecode(data)))
            using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(resultStream);
            }

            resultStream.Position = 0;

            var json = reader.ReadToEnd();

            // Utilizing Newtonsoft.Json as it better handles scalar types (string, int, etc.) when the target property is of type System.Object
            // System.Text.Json deserializes these as JsonElement, which is not ideal
            return JsonConvert.DeserializeObject<DynamoDBItemModel>(json);
        }

        private static string SerializeItem(DynamoDBItemModel model)
        {
            using var stream = new MemoryStream();

            using (var compressionStream = new GZipStream(stream, CompressionLevel.Fastest))
            using (var writer = new StreamWriter(compressionStream))
            {
                var json = JsonConvert.SerializeObject(model);
                writer.Write(json);
            }

            return WebEncoders.Base64UrlEncode(stream.ToArray());
        }

        private ActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            if(ex is AmazonDynamoDBException dynamodbEx)
            {
                if(dynamodbEx.StatusCode == HttpStatusCode.NotFound)
                    return StatusCode((int)dynamodbEx.StatusCode, new { error = dynamodbEx.Message });

                if(dynamodbEx.StatusCode == HttpStatusCode.BadRequest && dynamodbEx.ErrorCode == "GuiStack_InvalidField")
                    return StatusCode((int)dynamodbEx.StatusCode, new { error = dynamodbEx.Message });

                Console.Error.WriteLine(dynamodbEx);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> CreateTable([FromBody] DynamoDBCreateTableModel model)
        {
            if(string.IsNullOrWhiteSpace(model.TableName) || model.PartitionKey == null || string.IsNullOrEmpty(model.PartitionKey.Name))
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                await dynamodbRepository.CreateTableAsync(model);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{tableName}")]
        public async Task<ActionResult> DeleteTable([FromRoute] string tableName)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                await dynamodbRepository.DeleteTableAsync(tableName);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{tableName}")]
        [Produces("application/json")]
        public async Task<ActionResult> GetItems([FromRoute] string tableName, [FromQuery] int limit = 50, [FromQuery(Name = "lastEvaluatedKey")] string lastKey = null)
        {
            if(string.IsNullOrWhiteSpace(tableName) || limit < 1 || limit > 500)
                return StatusCode((int)HttpStatusCode.BadRequest);

            tableName = tableName.DecodeRouteParameter();

            try
            {
                var lastKeyItem = !string.IsNullOrWhiteSpace(lastKey)
                    ? DeserializeItem(lastKey)
                    : null;

                var table = await dynamodbRepository.GetTableInfoAsync(tableName);
                var result = await dynamodbRepository.ScanAsync(tableName, limit, lastKeyItem);

                var lastEvaluatedKey = result.LastEvaluatedKey != null
                    ? SerializeItem(result.LastEvaluatedKey)
                    : null;

                var attributeNames = new[] { table.PartitionKey.Name, table.SortKey?.Name } // Ensure PartitionKey and SortKey are always first
                    .Where(name => name != null)
                    .Concat(result.Items // Join with the rest of the fields
                        .SelectMany(item => item.Select(kvp => kvp.Key))
                        .Where(name => name != table.PartitionKey.Name && (table.SortKey == null || name != table.SortKey.Name))
                        .Distinct()
                    )
                    .ToArray();

                return Json(new DynamoDBTableContentsModel() {
                    PartitionKey = table.PartitionKey,
                    SortKey = table.SortKey,
                    AttributeNames = attributeNames,
                    LastEvaluatedKey = lastEvaluatedKey,
                    Items = result.Items
                });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{tableName}")]
        [Consumes("application/json")]
        public async Task<ActionResult> PutItem([FromRoute] string tableName, [FromBody] DynamoDBItemModel model)
        {
            if(string.IsNullOrWhiteSpace(tableName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                await dynamodbRepository.PutItemAsync(tableName, model);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
