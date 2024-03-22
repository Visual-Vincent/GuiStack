/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Threading.Tasks;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.DynamoDB
{
    public class IndexModel : PageModel
    {
        public IDynamoDBRepository DynamoDBRepository { get; }

        [BindProperty(SupportsGet = true)]
        public string Table { get; set; }

        public IndexModel(IDynamoDBRepository dynamodbRepository)
        {
            this.DynamoDBRepository = dynamodbRepository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetRenderTableContentsPartial([FromQuery] int limit = 50)
        {
            var items = await DynamoDBRepository.ScanAsync(Table, limit);
            return Partial("~/Pages/DynamoDB/_TableContentsPartial.cshtml", items);
        }
    }
}
