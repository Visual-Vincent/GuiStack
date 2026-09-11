/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2026
 * https://github.com/Visual-Vincent/GuiStack
 */

import type { Queue } from "$lib/models/sqs";
import { ApiClient } from "./apiClient";

export class SQSService
{
    public static async CreateQueueAsync(name: string, isFifo: boolean): Promise<void> {
        await ApiClient.PutAsync("/api/SQS/Queues", JSON.stringify({
            queueName: name,
            isFifo: isFifo
        }), { "Content-Type": "application/json" });
    }

    public static async DeleteQueueAsync(queueUrl: string): Promise<void> {
        await ApiClient.DeleteAsync(`/api/SQS/Queues/${encodeURIComponent(queueUrl)}`);
    }

    public static async GetQueuesAsync(): Promise<Queue[]> {
        return await ApiClient.GetAsync<Queue[]>("/api/SQS/Queues");
    }
}
