/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;

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
